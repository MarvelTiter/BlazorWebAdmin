using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

#pragma warning disable RS2008 // 启用分析器发布跟踪

namespace BlazorTemplate.ClientCore.Generators;

/// <summary>
///     为 <c>ModelPage&lt;TModel, TQuery&gt;</c> 的派生类生成两个 partial 重写：
///     <list type="bullet">
///         <item><c>ApplyCapabilities</c>：把「派生类是否重写了某个可选钩子」从运行时反射判定改为编译期常量。</item>
///         <item>
///             <c>CollectPageButtons</c>：扫描派生类继承链上所有 <c>[TableButton]</c>（含 EditButton/DeleteButton 等派生特性）
///             方法，在编译期静态构建按钮列表，取代运行时的特性反射扫描与按名字解析表达式方法。
///         </item>
///     </list>
/// </summary>
/// <remarks>
///     生成器无法在基类一侧求解：编译 ClientCore 时看不到任何派生类。
///     但在派生类所在的程序集里，它自己的 override 列表是完全可见的，
///     因此把「基类运行时查询」反转为「派生类编译期申报」即可 100% 静态判定。
///     <para>
///         安全边界：只有当派生类里<strong>确实找到</strong> [TableButton] 方法时才生成 CollectPageButtons 重写。
///         一个按钮都找不到时不生成，让基类的反射兜底继续生效——这保护了把按钮方法写在
///         razor <c>@code</c> 块里的页面（生成器在分析阶段看不到 Razor 生成的语法树）。
///     </para>
/// </remarks>
[Generator(LanguageNames.CSharp)]
public class ModelPageCapabilityGenerator : IIncrementalGenerator
{
    private const string BaseTypeFullName = "BlazorTemplate.ClientCore.Basic.ModelPage";
    private const string HookMethodName = "ApplyCapabilities";
    private const string ButtonsMethodName = "CollectPageButtons";
    private const string TableButtonTypeFullName = "BlazorTemplate.ClientCore.UI.Table.TableButton";

    // 已知按钮特性及其构造器默认值（与 TableButton.cs 中的定义保持一致）。
    // FullyQualifiedFormat 自带 global:: 前缀；生成器项目不引用宿主程序集，
    // 无法静态执行未知派生特性的构造器，因此只支持框架内置的这三个特性。
    private const string TableButtonAttributeFullName = "global::BlazorTemplate.ClientCore.UI.Table.TableButtonAttribute";
    private const string EditButtonAttributeFullName = "global::BlazorTemplate.ClientCore.UI.Table.EditButton";
    private const string DeleteButtonAttributeFullName = "global::BlazorTemplate.ClientCore.UI.Table.DeleteButton";

    /// <summary>可被派生类重写的可选钩子；数组顺序即生成代码的书写顺序。</summary>
    private static readonly HookDefinition[] Hooks =
    [
        new("OnExportAsync", "Options.ShowExportButton = {0};", null),
        new("OnAddItemAsync", "Options.ShowAddButton = {0};", null),
        new("HandleImportedDataAsync", "Options.ShowImportButton = {0};", null),
        new("OnSelectedChangedAsync", null, "Options.OnSelectedChangedAsync = OnSelectedChangedAsync;"),
        new("OnCellUpdateAsync", null, "Options.OnCellUpdateAsync = OnCellUpdateAsync;"),
        new("OnRowUpdateAsync", null, "Options.OnRowUpdateAsync = OnRowUpdateAsync;")
    ];

    private static readonly DiagnosticDescriptor NotPartialDescriptor = new(
        id: "MPCG001",
        title: "ModelPage 派生类应声明为 partial",
        messageFormat: "类型 \"{0}\" 派生自 ModelPage，请添加 partial 关键字，以便生成编译期的钩子能力申报代码",
        category: "ModelPageCapabilityGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor ButtonMissingLabelDescriptor = new(
        id: "MPCG002",
        title: "TableButton 缺少 Label 与 LabelExpression",
        messageFormat: "方法 \"{0}\" 的按钮特性既没有 Label 也没有 LabelExpression，运行时会抛出 ArgumentNullException；已跳过该页按钮的编译期生成，回退到反射路径",
        category: "ModelPageCapabilityGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor ButtonUnsupportedAttributeDescriptor = new(
        id: "MPCG003",
        title: "未知的 TableButton 派生特性无法在编译期求值",
        messageFormat: "方法 \"{0}\" 使用的特性 \"{1}\" 派生自 TableButtonAttribute，但其构造器设置的默认值无法在编译期求解；已跳过该页按钮的编译期生成，回退到反射路径",
        category: "ModelPageCapabilityGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var pages = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsCandidate(node),
                transform: static (ctx, ct) => Transform(ctx, ct))
            .Where(static page => page is not null);

        context.RegisterSourceOutput(pages, static (spc, page) => Emit(spc, page!));
    }

    private static bool IsCandidate(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax { BaseList: not null } decl)
        {
            return false;
        }

        // file 局部类型无法跨文件追加 partial 声明，直接放弃。
        return !decl.Modifiers.Any(SyntaxKind.FileKeyword);
    }

    private static PageModel? Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.Node is not ClassDeclarationSyntax decl)
        {
            return null;
        }

        if (context.SemanticModel.GetDeclaredSymbol(decl, cancellationToken) is not INamedTypeSymbol symbol)
        {
            return null;
        }

        // 暂不支持嵌套类型：生成嵌套 partial 需要重建外层声明。
        if (symbol.ContainingType is not null)
        {
            return null;
        }

        var modelPageBase = FindModelPageBase(symbol);
        if (modelPageBase is null)
        {
            return null;
        }

        var hooked = new bool[Hooks.Length];
        // 沿继承链向上，直到（不含）ModelPage 本身：中间基类重写过也算，
        // 这与原反射实现里 GetMethod 返回最派生版本的语义保持一致。
        for (var type = symbol; type is not null && !IsModelPage(type); type = type.BaseType)
        {
            foreach (var member in type.GetMembers())
            {
                if (member is not IMethodSymbol method || !method.IsOverride)
                {
                    continue;
                }

                var overridden = method.OverriddenMethod;
                if (overridden?.ContainingType is null || !IsModelPage(overridden.ContainingType))
                {
                    continue;
                }

                for (var i = 0; i < Hooks.Length; i++)
                {
                    if (Hooks[i].MethodName == method.Name)
                    {
                        hooked[i] = true;
                    }
                }
            }
        }

        // 按钮扫描：GetMethods(BINDING_ATTRS) 的静态等价——沿完整继承链收集所有
        // 带有 [TableButton]（或其派生特性）的普通方法。
        var buttons = ImmutableArray.CreateBuilder<ButtonModel>();
        for (var type = (INamedTypeSymbol?)symbol; type is not null; type = type.BaseType)
        {
            foreach (var member in type.GetMembers())
            {
                if (member is not IMethodSymbol
                    {
                        MethodKind: MethodKind.Ordinary,
                        IsGenericMethod: false,
                        Parameters.Length: 1
                    } method)
                {
                    continue;
                }

                var attr = method.GetAttributes()
                    .FirstOrDefault(static a => IsTableButtonAttribute(a.AttributeClass));
                if (attr is null)
                {
                    continue;
                }

                buttons.Add(BuildButton(method, attr));
            }
        }

        return new PageModel(
            ToHintName(symbol),
            symbol.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : null,
            BuildDeclaration(decl, symbol),
            symbol.Name,
            ImmutableArray.Create(hooked),
            symbol.GetMembers(HookMethodName).Any(static m => m is IMethodSymbol { IsOverride: true }),
            symbol.GetMembers(ButtonsMethodName).Any(static m => m is IMethodSymbol { IsOverride: true }),
            ExtractModelTypeFullName(modelPageBase),
            buttons.ToImmutable(),
            decl.Modifiers.Any(SyntaxKind.PartialKeyword),
            decl.Identifier.GetLocation());
    }

    private static void Emit(SourceProductionContext context, PageModel page)
    {
        if (!page.IsPartial)
        {
            context.ReportDiagnostic(Diagnostic.Create(NotPartialDescriptor, page.Location, page.TypeName));
            return;
        }

        var sections = new StringBuilder();

        // —— ApplyCapabilities：用户已自行重写该钩子时不做生成，避免重复 override。
        if (!page.HasUserHook)
        {
            var hookBody = new StringBuilder();
            for (var i = 0; i < Hooks.Length; i++)
            {
                var hook = Hooks[i];
                var overridden = page.Hooked[i];

                // 布尔开关无论真假都显式写出，以覆盖基类默认值；回调只在重写时挂接。
                var line = hook.ButtonAssignment is not null
                    ? string.Format(hook.ButtonAssignment, overridden ? "true" : "false")
                    : overridden ? hook.CallbackAssignment : null;

                if (line is not null)
                {
                    hookBody.Append("        ").Append(line).Append('\n');
                }
            }

            var body = hookBody.ToString();
            if (body.Length == 0)
            {
                body = "        // 该页未重写任何可选钩子，全部开关保持默认。\n";
            }

            sections.Append($$"""
                /// <summary>由 ModelPageCapabilityGenerator 生成：编译期判定的钩子能力申报。</summary>
                protected override void ApplyCapabilities()
                {
                {{body}}
                }

                """);
        }

        // —— CollectPageButtons：只在确实找到按钮方法时生成（见类型 remarks 中的安全边界）。
        if (!page.HasUserButtonsOverride && !page.Buttons.IsEmpty)
        {
            var unsupported = page.Buttons.FirstOrDefault(static b => b.UnsupportedAttribute is not null);
            var invalid = page.Buttons.FirstOrDefault(static b => !b.IsValid);
            if (unsupported is not null)
            {
                // 未知派生特性的构造器默认值无法静态求解，回退反射兜底以保持行为一致。
                context.ReportDiagnostic(Diagnostic.Create(
                    ButtonUnsupportedAttributeDescriptor, page.Location,
                    unsupported.MethodName, unsupported.UnsupportedAttribute));
            }
            else if (invalid is not null)
            {
                // 与原反射路径的 ArgumentNullException.ThrowIfNull(btnOptions.Label ?? btnOptions.LabelExpression) 对齐：
                // 反射兜底继续生效，行为保持不变，同时在编译期提前报错。
                context.ReportDiagnostic(Diagnostic.Create(
                    ButtonMissingLabelDescriptor, page.Location, invalid.MethodName));
            }
            else
            {
                var buttonBody = new StringBuilder();
                foreach (var button in page.Buttons)
                {
                    buttonBody.Append("        new global::").Append(TableButtonTypeFullName)
                        .Append('<').Append(page.ModelTypeFullName).Append(">\n")
                        .Append("        {\n");
                    foreach (var line in button.PropertyLines)
                    {
                        buttonBody.Append("            ").Append(line).Append('\n');
                    }
                    buttonBody.Append("        },\n");
                }

                sections.Append($$"""
                    /// <summary>由 ModelPageCapabilityGenerator 生成：编译期构建 [TableButton] 按钮（对象初始化器），替代运行时反射扫描。</summary>
                    protected override global::System.Collections.Generic.List<global::{{TableButtonTypeFullName}}<{{page.ModelTypeFullName}}>> CollectPageButtons()
                    {
                        return
                        [
                    {{buttonBody}}
                        ];
                    }

                    """);
            }
        }

        if (sections.Length == 0)
        {
            return;
        }

        var ns = page.NamespaceName is { Length: > 0 } name ? $"namespace {name};\n\n" : "";

        var source = $$"""
            // <auto-generated/>
            #nullable enable

            {{ns}}{{page.Declaration}}
            {
            {{sections}}
            }
            """;

        context.AddSource(page.HintName, SourceText.From(source, Encoding.UTF8));
    }

    /// <summary>
    ///     把按钮特性在编译期求解为 <c>TableButton&lt;TModel&gt;</c> 的对象初始化器属性行。
    ///     等价于运行时 <c>new TableButton&lt;TModel&gt;(attr)</c> 构造器的逐属性拷贝语义。
    /// </summary>
    private static ButtonModel BuildButton(IMethodSymbol method, AttributeData attr)
    {
        var attributeType = attr.AttributeClass!;
        var attributeTypeFullName = attributeType.ConstructedFrom.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat);

        // 已知特性的构造器默认值；未知派生特性无法静态执行其构造器，标记为不支持。
        string? defaultLabel;
        string? defaultIcon = null;
        var defaultDanger = false;
        switch (attributeTypeFullName)
        {
            case TableButtonAttributeFullName:
                defaultLabel = null;
                break;
            case EditButtonAttributeFullName:
                // EditButton 构造器只设置 Label/Icon，不设置 Danger（默认 false）。
                defaultLabel = "TableButtons.Edit";
                defaultIcon = "edit";
                break;
            case DeleteButtonAttributeFullName:
                defaultLabel = "TableButtons.Delete";
                defaultIcon = "delete";
                defaultDanger = true;
                break;
            default:
                return new ButtonModel(method.Name, [], isValid: false, unsupportedAttribute: attributeTypeFullName);
        }

        // 命名参数覆盖构造器默认值，与运行时 attribute 实例上的属性值一一对应。
        // 注意：生成器项目不引用宿主程序集，属性名只能用字符串字面量。
        var label = defaultLabel;
        var icon = defaultIcon;
        var buttonType = "primary";
        var danger = defaultDanger;
        string? group = null, confirmContent = null, confirmTitle = null, additionalParameter = null;
        string? labelExpression = null;
        var expressionLines = new List<string>();

        foreach (var namedArg in attr.NamedArguments)
        {
            switch (namedArg.Key)
            {
                case "Label": label = namedArg.Value.Value as string; break;
                case "Icon": icon = namedArg.Value.Value as string; break;
                case "Type": buttonType = namedArg.Value.Value as string ?? "primary"; break;
                case "Danger": danger = namedArg.Value.Value is true; break;
                case "Group": group = namedArg.Value.Value as string; break;
                case "ConfirmContent": confirmContent = namedArg.Value.Value as string; break;
                case "ConfirmTitle": confirmTitle = namedArg.Value.Value as string; break;
                case "AdditionalParameter": additionalParameter = namedArg.Value.Value as string; break;
                case "LabelExpression": labelExpression = namedArg.Value.Value as string; break;
                case "VisibleExpression":
                    if (namedArg.Value.Value is string visibleMethod)
                    {
                        expressionLines.Add($"VisibleExpression = {visibleMethod},");
                    }
                    break;
            }
        }

        // 与原反射路径对齐：Label 与 LabelExpression 皆缺省时运行时抛 ArgumentNullException。
        var isValid = label is not null || labelExpression is not null;

        // 对象初始化器只写非默认值：省略项与 TableButton 自身的属性默认值一致
        // （Label 默认 "Button"、Group 默认 "TableTips.ActionColumn"、Danger 默认 false、引用属性默认 null）。
        // ButtonType 例外：运行时总是拷贝 options.Type（缺省 "primary"），而 TableButton.ButtonType 默认 null，必须显式写出。
        var lines = new List<string>();
        if (label is not null)
        {
            lines.Add($"Label = {SymbolDisplay.FormatLiteral(label, quote: true)},");
        }
        if (icon is not null)
        {
            lines.Add($"Icon = {SymbolDisplay.FormatLiteral(icon, quote: true)},");
        }
        lines.Add($"ButtonType = {SymbolDisplay.FormatLiteral(buttonType, quote: true)},");
        if (danger)
        {
            lines.Add("Danger = true,");
        }
        if (confirmContent is not null)
        {
            lines.Add($"ConfirmContent = {SymbolDisplay.FormatLiteral(confirmContent, quote: true)},");
        }
        if (confirmTitle is not null)
        {
            lines.Add($"ConfirmTitle = {SymbolDisplay.FormatLiteral(confirmTitle, quote: true)},");
        }
        if (additionalParameter is not null)
        {
            lines.Add($"AdditionalParameter = {SymbolDisplay.FormatLiteral(additionalParameter, quote: true)},");
        }
        if (group is not null)
        {
            lines.Add($"Group = {SymbolDisplay.FormatLiteral(group, quote: true)},");
        }
        lines.Add($"Callback = {method.Name},");
        if (labelExpression is not null)
        {
            lines.Add($"LabelExpression = {labelExpression},");
        }
        lines.AddRange(expressionLines);

        return new ButtonModel(method.Name, lines, isValid, unsupportedAttribute: null);
    }

    private static bool IsTableButtonAttribute(INamedTypeSymbol? type)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.ConstructedFrom.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                == TableButtonAttributeFullName)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>从构造好的 ModelPage 基类中提取 TModel 实参的全名，供生成的泛型类型引用。</summary>
    private static string ExtractModelTypeFullName(INamedTypeSymbol modelPageBase)
    {
        return modelPageBase.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    private static bool IsModelPage(INamedTypeSymbol type)
    {
        var tn = type.ConstructedFrom?.ToDisplayString();
        return tn?.StartsWith(BaseTypeFullName) == true;
    }

    private static INamedTypeSymbol? FindModelPageBase(INamedTypeSymbol type)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (IsModelPage(current))
            {
                return current;
            }
        }

        return null;
    }

    private static string BuildDeclaration(ClassDeclarationSyntax decl, INamedTypeSymbol symbol)
    {
        var builder = new StringBuilder();

        // partial 的各个声明必须保持可访问性一致，因此只复制访问修饰符。
        foreach (var modifier in decl.Modifiers)
        {
            var text = modifier.ValueText;
            if (text is "public" or "internal" or "protected" or "private")
            {
                builder.Append(text).Append(' ');
            }
        }

        builder.Append("partial class ").Append(symbol.Name);

        if (decl.TypeParameterList is { Parameters.Count: > 0 } typeParameters)
        {
            builder.Append('<');
            builder.Append(string.Join(", ", typeParameters.Parameters.Select(static p => p.Identifier.ValueText)));
            builder.Append('>');
        }

        return builder.ToString();
    }

    private static string ToHintName(INamedTypeSymbol symbol)
    {
        var display = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
            .WithGenericsOptions(SymbolDisplayGenericsOptions.None));

        var builder = new StringBuilder(display.Length);
        foreach (var c in display)
        {
            builder.Append(char.IsLetterOrDigit(c) || c is '_' or '.' ? c : '_');
        }

        return builder.Append(".ApplyCapabilities.g.cs").ToString();
    }

    private sealed class HookDefinition
    {
        public readonly string MethodName;
        public readonly string? ButtonAssignment;
        public readonly string? CallbackAssignment;

        public HookDefinition(string methodName, string? buttonAssignment, string? callbackAssignment)
        {
            MethodName = methodName;
            ButtonAssignment = buttonAssignment;
            CallbackAssignment = callbackAssignment;
        }
    }

    private sealed class ButtonModel
    {
        public readonly string MethodName;
        public readonly List<string> PropertyLines;
        public readonly bool IsValid;
        public readonly string? UnsupportedAttribute;

        public ButtonModel(string methodName, List<string> propertyLines, bool isValid, string? unsupportedAttribute)
        {
            MethodName = methodName;
            PropertyLines = propertyLines;
            IsValid = isValid;
            UnsupportedAttribute = unsupportedAttribute;
        }
    }

    private sealed class PageModel : IEquatable<PageModel>
    {
        public readonly string HintName;
        public readonly string? NamespaceName;
        public readonly string Declaration;
        public readonly string TypeName;
        public readonly ImmutableArray<bool> Hooked;
        public readonly bool HasUserHook;
        public readonly bool HasUserButtonsOverride;
        public readonly string ModelTypeFullName;
        public readonly ImmutableArray<ButtonModel> Buttons;
        public readonly bool IsPartial;
        public readonly Location Location;

        public PageModel(
            string hintName,
            string? namespaceName,
            string declaration,
            string typeName,
            ImmutableArray<bool> hooked,
            bool hasUserHook,
            bool hasUserButtonsOverride,
            string modelTypeFullName,
            ImmutableArray<ButtonModel> buttons,
            bool isPartial,
            Location location)
        {
            HintName = hintName;
            NamespaceName = namespaceName;
            Declaration = declaration;
            TypeName = typeName;
            Hooked = hooked;
            HasUserHook = hasUserHook;
            HasUserButtonsOverride = hasUserButtonsOverride;
            ModelTypeFullName = modelTypeFullName;
            Buttons = buttons;
            IsPartial = isPartial;
            Location = location;
        }

        public bool Equals(PageModel? other) =>
            other is not null
            && HintName == other.HintName
            && NamespaceName == other.NamespaceName
            && Declaration == other.Declaration
            && TypeName == other.TypeName
            && HasUserHook == other.HasUserHook
            && HasUserButtonsOverride == other.HasUserButtonsOverride
            && ModelTypeFullName == other.ModelTypeFullName
            && IsPartial == other.IsPartial
            && Hooked.SequenceEqual(other.Hooked)
            && ButtonsEqual(Buttons, other.Buttons);

        private static bool ButtonsEqual(ImmutableArray<ButtonModel> left, ImmutableArray<ButtonModel> right)
        {
            if (left.Length != right.Length)
            {
                return false;
            }

            for (var i = 0; i < left.Length; i++)
            {
                if (left[i].MethodName != right[i].MethodName
                    || left[i].IsValid != right[i].IsValid
                    || left[i].UnsupportedAttribute != right[i].UnsupportedAttribute
                    || !left[i].PropertyLines.SequenceEqual(right[i].PropertyLines))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object? obj) => Equals(obj as PageModel);

        public override int GetHashCode() => HintName.GetHashCode();
    }
}

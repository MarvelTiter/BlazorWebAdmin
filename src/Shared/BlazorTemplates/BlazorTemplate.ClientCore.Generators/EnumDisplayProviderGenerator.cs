using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BlazorTemplate.ClientCore.Generators;

/// <summary>
///     为枚举生成编译期元数据，产出一个 <c>{Name}EnumMeta : IEnumMeta</c>，覆盖该枚举的<strong>全部</strong>成员。
///     <para>
///         它是枚举信息的唯一底层来源，上层两类消费方都从它派生，本生成器不再为每个枚举另发 provider：
///         <list type="bullet">
///             <item>
///                 <c>EnumHelper&lt;TEnum&gt;</c>——"枚举字典"语义（全部成员，缺 <c>[Display]</c> 回退成员名）；
///             </item>
///             <item>
///                 <c>EnumLookupProvider&lt;TEnum&gt;</c>——"可选项"语义（只含带 <c>[Display]</c> 的成员），
///                 由生成代码在模块初始化时用元数据构造并交给 <c>LookupService</c>；
///                 该 provider 是通用实现，不再逐枚举生成。
///             </item>
///         </list>
///         两类语义刻意留在同一份数据上、由消费方各自取用，避免"全成员"与"仅 <c>[Display]</c> 成员"两个集合漂移。
///     </para>
///     <remarks>
///         覆盖范围只有当前编译单元的语法树；引用程序集里的枚举需要消费方调用一次
///         <c>AddEnumLookupProvider&lt;TEnum&gt;()</c> 作为编译期登记标记才会被生成。
///     </remarks>
/// </summary>
[Generator(LanguageNames.CSharp)]
public class EnumDisplayProviderGenerator : IIncrementalGenerator
{
    private const string DisplayAttributeFullName = "System.ComponentModel.DataAnnotations.DisplayAttribute";

    private static readonly SymbolDisplayFormat FullyQualified = SymbolDisplayFormat.FullyQualifiedFormat;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enums = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is EnumDeclarationSyntax,
            static (c, _) =>
            {
                var si = c.SemanticModel.GetDeclaredSymbol(c.Node) as INamedTypeSymbol;
                return si!;
            }
        ).Collect();

        var manual = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is InvocationExpressionSyntax
            {
                Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "AddEnumLookupProvider" }
            },
            static (c, _) =>
            {
                var invocation = (InvocationExpressionSyntax)c.Node;
                if (c.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol symbol)
                {
                    return null!;
                }
                return (symbol.TypeArguments.FirstOrDefault() as INamedTypeSymbol)!;
            }
            ).Where(t => t is not null).Collect();

        var cc = context.CompilationProvider.Combine(enums.Combine(manual));
        context.RegisterSourceOutput(cc, static (context, c) =>
        {
            var (compilation, allEnums) = c;
            Emit(context, allEnums.Left, allEnums.Right);
        });
    }

    private static void Emit(
        SourceProductionContext context,
        ImmutableArray<INamedTypeSymbol> enums,
        ImmutableArray<INamedTypeSymbol> manual)
    {
        // 按全名去重：AddEnumLookupProvider<本编译单元内的枚举>() 会让同一个枚举命中两条分支，
        // 原先会导致同名类与同名 hintName 重复提交（AddSource 直接抛异常）。
        var seen = new HashSet<string>(StringComparer.Ordinal);
        // 类名唯一化：不同命名空间下同名枚举会撞类名。
        var usedNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var item in enums.Concat(manual))
        {
            if (item is null)
            {
                continue;
            }

            var fullName = item.ToDisplayString(FullyQualified);
            if (!seen.Add(fullName))
            {
                continue;
            }

            // 只要能在生成代码里写出该枚举的 typeof 引用与成员引用就生成；否则静默跳过。
            if (!CanReference(item))
            {
                continue;
            }

            var uniqueName = MakeUniqueName(item.Name, fullName, usedNames);
            var members = GetEnumMembers(item);

            context.AddSource($"{uniqueName}EnumMeta.g.cs",
                BuildEnumMeta(uniqueName, fullName, members));
        }
    }

    /// <summary>
    ///     枚举成员按底层值升序，与 <c>Enum.GetValues(Type)</c> 的顺序一致。
    ///     <c>value__</c> 是编译器为枚举合成的实例字段，必须排除。
    /// </summary>
    private static EnumMember[] GetEnumMembers(INamedTypeSymbol enumType)
    {
        return enumType.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(static f => f.HasConstantValue && f.Name != "value__")
            .OrderBy(static f => f.ConstantValue!, Comparer<object>.Default)
            .Select(static f => new EnumMember(f.Name, GetDisplayLabel(f)))
            .ToArray();
    }

    private static string? GetDisplayLabel(IFieldSymbol field)
    {
        var display = field.GetAttributes()
            .FirstOrDefault(static a => a.AttributeClass?.ToDisplayString() == DisplayAttributeFullName);
        return display?.NamedArguments.FirstOrDefault(static n => n.Key == "Name").Value.Value as string;
    }

    /// <summary>
    ///     能否在生成代码里安全写出 <c>typeof(枚举全名)</c> 与成员引用：
    ///     要求整个外层类型链对外可见，且不含开放泛型（带类型参数的外层类型无法直接引用）。
    /// </summary>
    private static bool CanReference(INamedTypeSymbol type)
    {
        for (var current = (INamedTypeSymbol?)type; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility != Accessibility.Public || current.TypeParameters.Length > 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     生成 <c>{Name}EnumMeta : IEnumMeta</c>——枚举信息的唯一产物。
    ///     <para>
    ///         值以 <c>object</c> 装箱写进 <c>EnumField.Value</c>，消费侧靠拆箱取用，
    ///         因此不需要 <c>MakeGenericMethod</c> 类动态代码生成；泛型只出现在
    ///         <c>EnumLookupProvider&lt;具体枚举&gt;</c> 这种实参可静态写死的位置。
    ///     </para>
    ///     <para>
    ///         这里的语法一律取保守形式（显式 <c>new EnumField(...)</c>、不用集合表达式与目标类型 <c>new</c>），
    ///         因为生成产物要在<strong>消费方项目</strong>里编译，而本仓库是模板仓库、消费方的 LangVersion 不可控。
    ///     </para>
    /// </summary>
    private static string BuildEnumMeta(string className, string fullName, EnumMember[] members)
    {
        var hasAnyLabel = members.Any(static m => m.Label is not null);
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated />");
        // 显式声明可空上下文：CS8669 要求自动生成代码用显式指令，不继承项目级配置。
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using BlazorTemplate.ClientCore.Lookup;");
        sb.AppendLine("namespace EnumDisplayProviderGenerator;");
        sb.AppendLine($"public sealed class {className}EnumMeta : IEnumMeta");
        sb.AppendLine("{");

        // —— 成员名 → 显示名（全成员，缺 [Display] 回退成员名），ParseDictionary 用
        sb.AppendLine("    private static readonly global::System.Collections.Generic.Dictionary<string, string> displayNames = new()");
        sb.AppendLine("    {");
        foreach (var member in members)
        {
            sb.AppendLine($"        [{Literal(member.Name)}] = {Literal(member.Label ?? member.Name)},");
        }
        sb.AppendLine("    };");
        sb.AppendLine();

        // —— 全成员清单（带值），是其余一切的来源
        sb.AppendLine("    private static readonly EnumField[] fields = new EnumField[]");
        sb.AppendLine("    {");
        foreach (var member in members)
        {
            sb.AppendLine($"        new EnumField({fullName}.{member.Name}, {Literal(member.Name)}, {LiteralOrNull(member.Label)}),");
        }
        sb.AppendLine("    };");
        sb.AppendLine();

        // —— 模块初始化：注册元数据；有 [Display] 成员时顺带注册通用 provider
        sb.AppendLine("    [global::System.Runtime.CompilerServices.ModuleInitializer]");
        sb.AppendLine("    public static void Init()");
        sb.AppendLine("    {");
        sb.AppendLine($"        var meta = new {className}EnumMeta();");
        sb.AppendLine($"        EnumMetaRegistry.Register(typeof({fullName}), meta);");
        if (hasAnyLabel)
        {
            // 一个带 [Display] 的成员都没有时不注册，等价于原来"不生成 provider"的语义。
            sb.AppendLine($"        LookupService.AddEnumProvider(new EnumLookupProvider<{fullName}>(meta));");
        }
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    public global::System.Collections.Generic.IReadOnlyDictionary<string, string> DisplayNames => displayNames;");
        sb.AppendLine();
        sb.AppendLine("    public global::System.Collections.Generic.IReadOnlyList<EnumField> Fields => fields;");
        sb.AppendLine();
        sb.AppendLine("    public string? GetDisplayName(object value)");
        sb.AppendLine($"        => value is {fullName} v ? GetDisplayName(v) : null;");
        sb.AppendLine();

        if (hasAnyLabel)
        {
            // 与原反射语义对齐：没有 [Display] 的成员返回 null（ParseDictionary 才回退成成员名）。
            sb.AppendLine($"    private static string? GetDisplayName({fullName} value) => value switch");
            sb.AppendLine("    {");
            foreach (var member in members.Where(static m => m.Label is not null))
            {
                sb.AppendLine($"        {fullName}.{member.Name} => {Literal(member.Label!)},");
            }
            sb.AppendLine("        _ => null,");
            sb.AppendLine("    };");
        }
        else
        {
            sb.AppendLine($"    private static string? GetDisplayName({fullName} value) => null;");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>
    ///     类名唯一化。同名枚举（不同命名空间）时给后者加一个由全名算出的稳定短哈希，
    ///     避免生成的类与 hintName 冲突。不使用 <c>string.GetHashCode</c>——它在不同进程间不稳定。
    /// </summary>
    private static string MakeUniqueName(string enumName, string fullName, HashSet<string> usedNames)
    {
        if (usedNames.Add(enumName))
        {
            return enumName;
        }

        var candidate = enumName + "_" + StableSuffix(fullName);
        usedNames.Add(candidate);
        return candidate;
    }

    private static string StableSuffix(string text)
    {
        unchecked
        {
            var hash = 2166136261u;
            foreach (var c in text)
            {
                hash ^= c;
                hash *= 16777619u;
            }
            return hash.ToString("x8", CultureInfo.InvariantCulture);
        }
    }

    private static string Literal(string value) => SymbolDisplay.FormatLiteral(value, quote: true);

    private static string LiteralOrNull(string? value) => value is null ? "null" : Literal(value);

    private sealed class EnumMember
    {
        public readonly string Name;
        public readonly string? Label;

        public EnumMember(string name, string? label)
        {
            Name = name;
            Label = label;
        }
    }
}

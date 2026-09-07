using System;
using System.Collections.Generic;
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
///     把 <c>PagesService</c> 构造时基于反射的路由元数据采集（遍历
///     <c>AppConst.AllAssemblies</c> → <c>ExportedTypes</c> → 逐个读 Route / PageInfo / PageGroup /
///     Authorize / AllowAnonymous / Layout 特性）替换为编译期生成。
///     <para>
///         同时取代 <c>ProjectInit.ScanRazorLibraryAssembly</c> 的程序集发现：生成的代码会把
///         路由类型所在程序集静态登记到 <c>AppConst</c>：入口程序集写入
///     <c>AppConst.AppAssembly</c>，其余程序集写入 <c>AppConst.AdditionalAssemblies</c>，
///         运行时不再需要 <c>Assembly.Load</c> 遍历引用程序集去判断"哪个程序集里有页面"。
///     </para>
/// </summary>
/// <remarks>
///     覆盖范围：只有当前编译单元（语法树）里显式写了 <c>[Route]</c> 的类型可被扫描到。
///     razor 文件中的 <c>@page</c> 由 Razor 源生成器在编译期注入特性，而源生成器之间互相不可见，
///     因此这类页面无法覆盖——它们继续走 <c>PagesService</c> 的反射兜底，
///     并通过 <c>PageRouteContext.IsGenerated</c> 做类型级去重，不会重复登记。
///     <para>
///         与反射语义对齐的几点：
///         <list type="bullet">
///             <item>只处理 <c>ExportedTypes</c> 等价的 public（含其外层类型）类型；</item>
///             <item>开放泛型类型跳过（反射路径里同样无法实例化路由），交回兜底；</item>
///             <item><c>Authorize</c> 按原代码 <c>inherit: false</c> 只查自身，其余特性按 <c>inherit: true</c> 沿基类链查找。</item>
///         </list>
///     </para>
/// </remarks>
[Generator(LanguageNames.CSharp)]
public class PageRouteRegistryGenerator : IIncrementalGenerator
{
    // 生成器工程不引用宿主程序集，特性全名一律用字符串常量。
    // SymbolDisplayFormat.FullyQualifiedFormat 自带 global:: 前缀，常量必须与之完全一致。
    private const string RouteAttributeFullName = "global::Microsoft.AspNetCore.Components.RouteAttribute";
    private const string PageInfoAttributeFullName = "global::BlazorTemplate.ClientCore.Common.Attributes.PageInfoAttribute";
    private const string PageGroupAttributeFullName = "global::BlazorTemplate.ClientCore.Common.Attributes.PageGroupAttribute";
    private const string AuthorizeAttributeFullName = "global::Microsoft.AspNetCore.Authorization.AuthorizeAttribute";
    private const string AllowAnonymousAttributeFullName = "global::Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute";
    private const string LayoutAttributeFullName = "global::Microsoft.AspNetCore.Components.LayoutAttribute";

    private const string RouteMetaTypeFullName = "global::BlazorTemplate.ClientCore.Store.Models.RouteMeta";
    private const string PageRouteContextTypeFullName = "global::BlazorTemplate.ClientCore.Routers.PageRouteContext";

    private static readonly SymbolDisplayFormat FullyQualified = SymbolDisplayFormat.FullyQualifiedFormat;

    private static readonly DiagnosticDescriptor GenericPageSkippedDescriptor = new(
        id: "PRRG001",
        title: "开放泛型路由类型无法在编译期登记",
        messageFormat: "类型 \"{0}\" 标注了 [Route] 但带有类型参数，无法生成 typeof 表达式；已跳过编译期登记，由 PagesService 的反射兜底处理",
        category: "PageRouteRegistryGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (ctx, ct) => Transform(ctx, ct))
            .Where(static model => model is not null)
            .Collect();

        var compilationAndTypes = candidates.Combine(context.CompilationProvider);

        context.RegisterSourceOutput(compilationAndTypes, static (spc, source) =>
        {
            Emit(spc, source.Left, source.Right);
        });
    }

    private static RouteModel? Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.Node is not ClassDeclarationSyntax decl)
        {
            return null;
        }

        if (context.SemanticModel.GetDeclaredSymbol(decl, cancellationToken) is not INamedTypeSymbol symbol)
        {
            return null;
        }

        // 与原反射路径的 assembly.ExportedTypes 对齐：只登记对外可见的类型。
        if (!IsExported(symbol))
        {
            return null;
        }

        // 与原反射路径一致：Authorize 用 GetCustomAttribute<T>(false)，只查自身声明。
        var routeAttr = FindAttribute(symbol, RouteAttributeFullName, inherit: true);
        if (routeAttr is null)
        {
            return null;
        }

        if (symbol.TypeParameters.Length > 0)
        {
            return new RouteModel(
                symbol.ToDisplayString(FullyQualified),
                null,
                null,
                null,
                null,
                false,
                0,
                false,
                null,
                null,
                false,
                false,
                isGeneric: true,
                decl.Identifier.GetLocation());
        }

        var template = routeAttr.ConstructorArguments.Length > 0
            ? routeAttr.ConstructorArguments[0].Value as string
            : null;
        if (string.IsNullOrEmpty(template))
        {
            return null;
        }

        var infoAttr = FindAttribute(symbol, PageInfoAttributeFullName, inherit: true);
        var groupAttr = FindAttribute(symbol, PageGroupAttributeFullName, inherit: true);
        var layoutAttr = FindAttribute(symbol, LayoutAttributeFullName, inherit: true);
        var authorizeAttr = FindAttribute(symbol, AuthorizeAttributeFullName, inherit: false);
        // 原代码 AllowAnonymous 用的是单参数重载，默认 inherit: true。
        var allowAnonymousAttr = FindAttribute(symbol, AllowAnonymousAttributeFullName, inherit: true);

        string? infoId = null, infoTitle = null, infoIcon = null, infoGroupId = null;
        var infoSort = 0;
        var infoPin = false;
        var hasPageInfo = infoAttr is not null;

        if (infoAttr is not null)
        {
            // PageInfoAttribute 有两个构造器：(string id) 与 ()。
            if (infoAttr.ConstructorArguments.Length > 0)
            {
                infoId = infoAttr.ConstructorArguments[0].Value as string;
            }

            foreach (var named in infoAttr.NamedArguments)
            {
                switch (named.Key)
                {
                    case "Id": infoId = named.Value.Value as string; break;
                    case "Title": infoTitle = named.Value.Value as string; break;
                    case "Icon": infoIcon = named.Value.Value as string; break;
                    case "GroupId": infoGroupId = named.Value.Value as string; break;
                    case "Sort": infoSort = named.Value.Value is int s ? s : 0; break;
                    case "Pin": infoPin = named.Value.Value is true; break;
                }
            }
        }

        string? groupId = null;
        GroupModel? groupModel = null;
        if (groupAttr is not null && groupAttr.ConstructorArguments.Length >= 3)
        {
            groupId = groupAttr.ConstructorArguments[0].Value as string;
            var groupName = groupAttr.ConstructorArguments[1].Value as string ?? groupId ?? string.Empty;
            var groupSort = groupAttr.ConstructorArguments[2].Value is int gs ? gs : 0;
            string? groupIcon = null;
            foreach (var named in groupAttr.NamedArguments)
            {
                if (named.Key == "Icon")
                {
                    groupIcon = named.Value.Value as string;
                }
            }

            if (!string.IsNullOrEmpty(groupId))
            {
                groupModel = new GroupModel(groupId!, groupName, groupIcon, groupSort);
            }
        }

        string? layoutType = null;
        if (layoutAttr is not null
            && layoutAttr.ConstructorArguments.Length > 0
            && layoutAttr.ConstructorArguments[0].Value is ITypeSymbol layoutSymbol)
        {
            layoutType = layoutSymbol.ToDisplayString(FullyQualified);
        }

        var typeName = symbol.Name;
        var typeFullName = symbol.ToDisplayString(FullyQualified);

        return new RouteModel(
            typeFullName,
            routeId: string.IsNullOrEmpty(infoId) ? typeName : infoId,
            routeTitle: string.IsNullOrEmpty(infoTitle) ? typeName : infoTitle,
            routeUrl: template,
            icon: infoIcon,
            pin: infoPin,
            sort: infoSort,
            hasPageInfo: hasPageInfo,
            group: string.IsNullOrEmpty(infoGroupId) ? (string.IsNullOrEmpty(groupId) ? "ROOT" : groupId) : infoGroupId,
            layoutTypeFullName: layoutType,
            isStaticPath: !HasRouteParameter(template!),
            isAllowAnonymous: allowAnonymousAttr is not null || authorizeAttr is null,
            isGeneric: false,
            decl.Identifier.GetLocation(),
            groupModel,
            symbol.ContainingAssembly);
    }

    private static void Emit(
        SourceProductionContext context,
        System.Collections.Immutable.ImmutableArray<RouteModel?> candidates,
        Compilation compilation)
    {
        var pages = new List<RouteModel>();
        var groups = new Dictionary<string, GroupModel>(StringComparer.Ordinal);
        var groupOrder = new List<string>();

        foreach (var candidate in candidates)
        {
            if (candidate is null)
            {
                continue;
            }

            if (candidate.IsGeneric)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    GenericPageSkippedDescriptor, candidate.Location, candidate.TypeFullName));
                continue;
            }

            // 只登记当前编译产出的程序集里的类型：引用的程序集由它们自己的生成器负责。
            if (!SymbolEqualityComparer.Default.Equals(candidate.Assembly, compilation.Assembly))
            {
                continue;
            }

            pages.Add(candidate);
            CollectGroup(candidate, groups, groupOrder);
        }

        if (pages.Count == 0)
        {
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine($"namespace {compilation.Assembly.Name};");
        sb.AppendLine();
        sb.AppendLine("/// <summary>由 PageRouteRegistryGenerator 生成：编译期登记的路由与分组元数据。</summary>");
        sb.AppendLine("public static class PageRouteRegistry");
        sb.AppendLine("{");
        sb.AppendLine("    [global::System.Runtime.CompilerServices.ModuleInitializer]");
        sb.AppendLine("    public static void Initialize()");
        sb.AppendLine("    {");
        sb.AppendLine($"        {PageRouteContextTypeFullName}.RegisterPages(static () => BuildPages());");
        sb.AppendLine($"        {PageRouteContextTypeFullName}.RegisterGroups(static () => BuildGroups());");

        // 当前编译产出的是入口程序集时登记为 AppConst.AppAssembly，否则作为附加页面程序集登记。
        // 等价于运行时 Assembly.GetEntryAssembly() 的判断，但在编译期就已经确定。
        var registerMethod = IsEntryAssembly(compilation) ? "RegisterAppAssembly" : "RegisterAssembly";
        sb.AppendLine($"        {PageRouteContextTypeFullName}.{registerMethod}(typeof({pages[0].TypeFullName}));");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine($"    private static {RouteMetaTypeFullName}[] BuildPages()");
        sb.AppendLine("    {");
        sb.AppendLine("        return");
        sb.AppendLine("        [");
        foreach (var page in pages)
        {
            EmitRouteMeta(sb, page);
        }

        sb.AppendLine("        ];");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine($"    private static {RouteMetaTypeFullName}[] BuildGroups()");
        sb.AppendLine("    {");
        sb.AppendLine("        return");
        sb.AppendLine("        [");
        foreach (var id in groupOrder)
        {
            var group = groups[id];
            sb.AppendLine($"            new {RouteMetaTypeFullName}");
            sb.AppendLine("            {");
            sb.AppendLine($"                RouteId = {Literal(group.Id)},");
            sb.AppendLine($"                RouteTitle = {Literal(group.Name)},");
            if (group.Icon is not null)
            {
                sb.AppendLine($"                Icon = {Literal(group.Icon)},");
            }

            sb.AppendLine($"                Sort = {group.Sort.ToString(CultureInfo.InvariantCulture)},");
            sb.AppendLine("                Group = \"ROOT\",");
            sb.AppendLine("                HasPageInfo = true,");
            sb.AppendLine("                IsAllowAnonymous = true,");
            sb.AppendLine("                IsGroupHeader = true,");
            sb.AppendLine("            },");
        }

        sb.AppendLine("        ];");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource("PageRouteRegistry.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    /// <summary>
    ///     判断当前编译产出的程序集是否为应用入口程序集，即运行时
    ///     <c>Assembly.GetEntryAssembly()</c> 会返回的那一个。
    ///     <para>
    ///         入口程序集登记到 <c>AppConst.AppAssembly</c>；其余程序集（Razor 类库等）
    ///         登记到 <c>AppConst.AdditionalAssemblies</c>。
    ///     </para>
    /// </summary>
    private static bool IsEntryAssembly(Compilation compilation)
    {
        if (compilation.GetEntryPoint(CancellationToken.None) is not null)
        {
            return true;
        }

        // 兜底：入口点由编译器后期注入、或尚未绑定到符号时，按输出类型判断。
        return compilation.Options.OutputKind is OutputKind.ConsoleApplication or OutputKind.WindowsApplication;
    }

    private static void EmitRouteMeta(StringBuilder sb, RouteModel page)
    {
        sb.AppendLine($"            new {RouteMetaTypeFullName}");
        sb.AppendLine("            {");
        sb.AppendLine($"                RouteId = {Literal(page.RouteId!)},");
        sb.AppendLine($"                RouteTitle = {Literal(page.RouteTitle!)},");
        sb.AppendLine($"                RouteUrl = {Literal(page.RouteUrl!)},");
        sb.AppendLine($"                RouteType = typeof({page.TypeFullName}),");
        if (page.Icon is not null)
        {
            sb.AppendLine($"                Icon = {Literal(page.Icon)},");
        }

        if (page.Pin)
        {
            sb.AppendLine("                Pin = true,");
        }

        sb.AppendLine($"                Group = {Literal(page.Group!)},");
        sb.AppendLine($"                Sort = {page.Sort.ToString(CultureInfo.InvariantCulture)},");
        if (page.HasPageInfo)
        {
            sb.AppendLine("                HasPageInfo = true,");
        }

        if (page.LayoutTypeFullName is not null)
        {
            sb.AppendLine($"                Layout = typeof({page.LayoutTypeFullName}),");
        }

        if (page.IsStaticPath)
        {
            sb.AppendLine("                IsStaticPath = true,");
        }

        if (page.IsAllowAnonymous)
        {
            sb.AppendLine("                IsAllowAnonymous = true,");
        }

        sb.AppendLine("            },");
    }

    /// <summary>
    ///     收集该类型上的 <c>[PageGroup]</c>。与 PagesService.TryAddGroup 的语义一致：
    ///     按 Id 去重，已存在时仅用后来者的 Icon 补空。
    /// </summary>
    private static void CollectGroup(
        RouteModel page,
        Dictionary<string, GroupModel> groups,
        List<string> groupOrder)
    {
        if (page.GroupAttribute is null)
        {
            return;
        }

        var group = page.GroupAttribute;
        if (groups.TryGetValue(group.Id, out var existed))
        {
            if (string.IsNullOrEmpty(existed.Icon) && group.Icon is not null)
            {
                groups[group.Id] = new GroupModel(existed.Id, existed.Name, group.Icon, existed.Sort);
            }

            return;
        }

        groups[group.Id] = group;
        groupOrder.Add(group.Id);
    }

    private static AttributeData? FindAttribute(INamedTypeSymbol type, string attributeFullName, bool inherit)
    {
        for (var current = (INamedTypeSymbol?)type; current is not null; current = current.BaseType)
        {
            foreach (var attribute in current.GetAttributes())
            {
                if (InheritsFrom(attribute.AttributeClass, attributeFullName))
                {
                    return attribute;
                }
            }

            if (!inherit)
            {
                return null;
            }
        }

        return null;
    }

    private static bool InheritsFrom(INamedTypeSymbol? attributeClass, string targetFullName)
    {
        for (var current = attributeClass; current is not null; current = current.BaseType)
        {
            if (current.ToDisplayString(FullyQualified) == targetFullName)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsExported(INamedTypeSymbol type)
    {
        for (var current = (INamedTypeSymbol?)type; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility != Accessibility.Public)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>等价于原正则 <c>\{[^{}]+\}</c>：判断路由模板里是否含有路径参数。</summary>
    private static bool HasRouteParameter(string template)
    {
        var index = template.IndexOf('{');
        while (index >= 0)
        {
            var end = template.IndexOf('}', index + 1);
            if (end < 0)
            {
                return false;
            }

            var inner = template.Substring(index + 1, end - index - 1);
            if (inner.Length > 0 && !inner.Contains('{') && !inner.Contains('}'))
            {
                return true;
            }

            index = template.IndexOf('{', index + 1);
        }

        return false;
    }

    private static string Literal(string? value)
    {
        return value is null ? "null!" : SymbolDisplay.FormatLiteral(value, quote: true);
    }

    private sealed class GroupModel
    {
        public readonly string Id;
        public readonly string Name;
        public readonly string? Icon;
        public readonly int Sort;

        public GroupModel(string id, string name, string? icon, int sort)
        {
            Id = id;
            Name = name;
            Icon = icon;
            Sort = sort;
        }
    }

    private sealed class RouteModel
    {
        public readonly string TypeFullName;
        public readonly IAssemblySymbol? Assembly;
        public readonly string? RouteId;
        public readonly string? RouteTitle;
        public readonly string? RouteUrl;
        public readonly string? Icon;
        public readonly bool Pin;
        public readonly int Sort;
        public readonly bool HasPageInfo;
        public readonly string? Group;
        public readonly string? LayoutTypeFullName;
        public readonly bool IsStaticPath;
        public readonly bool IsAllowAnonymous;
        public readonly bool IsGeneric;
        public readonly GroupModel? GroupAttribute;
        public readonly Location Location;

        public RouteModel(
            string typeFullName,
            string? routeId,
            string? routeTitle,
            string? routeUrl,
            string? icon,
            bool pin,
            int sort,
            bool hasPageInfo,
            string? group,
            string? layoutTypeFullName,
            bool isStaticPath,
            bool isAllowAnonymous,
            bool isGeneric,
            Location location,
            GroupModel? groupAttribute = null,
            IAssemblySymbol? assembly = null)
        {
            TypeFullName = typeFullName;
            RouteId = routeId;
            RouteTitle = routeTitle;
            RouteUrl = routeUrl;
            Icon = icon;
            Pin = pin;
            Sort = sort;
            HasPageInfo = hasPageInfo;
            Group = group;
            LayoutTypeFullName = layoutTypeFullName;
            IsStaticPath = isStaticPath;
            IsAllowAnonymous = isAllowAnonymous;
            IsGeneric = isGeneric;
            Location = location;
            GroupAttribute = groupAttribute;
            Assembly = assembly;
        }
    }
}

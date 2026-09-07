using System.Reflection;
using BlazorTemplate.ClientCore.Store.Models;

namespace BlazorTemplate.ClientCore.Routers;

/// <summary>
///     编译期路由元数据的注册表。
///     <para>
///         源生成器（<c>PageRouteRegistryGenerator</c>）会为当前编译单元中显式标注了
///         <c>[Route]</c> 的类型生成 <see cref="RouteMeta" /> 构造代码，并通过 ModuleInitializer
///         注册到这里；<see cref="PagesService" /> 直接读取，不再对这些类型做运行时特性反射。
///     </para>
///     <para>
///         覆盖范围限制：源生成器之间互相不可见，因此写在 razor 文件里的 <c>@page</c> / <c>@attribute</c>
///         无法被扫描到。这类页面仍由 <see cref="PagesService" /> 的反射兜底路径补充，
///         并用 <see cref="IsGenerated" /> 做类型级去重，保证同一类型不会被登记两次。
///     </para>
/// </summary>
public static class PageRouteContext
{
    private static readonly List<RouteMeta> pages = [];
    private static readonly List<RouteMeta> groups = [];
    private static readonly HashSet<Type> pageTypes = [];
    private static readonly HashSet<string> groupIds = new(StringComparer.Ordinal);
    private static readonly HashSet<Assembly> registeredAssemblies = [];

    /// <summary>是否已经有过编译期注册。为 false 时运行时的程序集扫描兜底不可跳过。</summary>
    public static bool HasGeneratedPages => pages.Count > 0;

    /// <summary>编译期登记到的页面元数据。</summary>
    public static IReadOnlyList<RouteMeta> GeneratedPages => pages;

    /// <summary>编译期登记到的分组元数据（已按 RouteId 去重）。</summary>
    public static IReadOnlyList<RouteMeta> GeneratedGroups => groups;

    /// <summary>注册页面元数据；按 <see cref="RouteMeta.RouteType" /> 去重，重复注册会被忽略。</summary>
    public static void RegisterPages(Func<RouteMeta[]> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        foreach (var meta in factory())
        {
            if (meta?.RouteType is null)
            {
                continue;
            }

            if (!pageTypes.Add(meta.RouteType))
            {
                continue;
            }

            pages.Add(meta);
        }
    }

    /// <summary>
    ///     注册分组元数据；按 <see cref="RouteMeta.RouteId" /> 去重。
    ///     重复分组会用后来者的 Icon 补空，与 <see cref="PagesService" /> 中 TryAddGroup 的合并语义保持一致。
    /// </summary>
    public static void RegisterGroups(Func<RouteMeta[]> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        foreach (var meta in factory())
        {
            if (meta is null || string.IsNullOrEmpty(meta.RouteId))
            {
                continue;
            }

            var existed = groups.Find(g => g.RouteId == meta.RouteId);
            if (existed is not null)
            {
                if (string.IsNullOrEmpty(existed.Icon) && meta.Icon is not null)
                {
                    existed.Icon = meta.Icon;
                }

                continue;
            }

            groupIds.Add(meta.RouteId);
            groups.Add(meta);
        }
    }

    /// <summary>该页面类型是否已经由生成器登记过；反射兜底路径据此跳过，避免重复。</summary>
    public static bool IsGenerated(Type pageType) => pageTypes.Contains(pageType);

    /// <summary>已经由生成器静态登记过页面、因此无需再做程序集发现的程序集。</summary>
    public static ICollection<Assembly> RegisteredAssemblies => registeredAssemblies;

    /// <summary>
    ///     静态登记一个包含页面的<strong>非入口</strong>程序集（取代 <c>ProjectInit.ScanRazorLibraryAssembly</c>
    ///     的程序集发现），进入 <see cref="AppConst.AdditionalAssemblies" />。
    /// </summary>
    public static void RegisterAssembly(Type? routeType)
    {
        if (routeType is null)
        {
            return;
        }

        AppConst.AddAssembly(routeType);
        registeredAssemblies.Add(routeType.Assembly);
    }

    /// <summary>
    ///     静态登记<strong>应用入口</strong>程序集（等价于运行时 <c>Assembly.GetEntryAssembly()</c>）。
    ///     <para>
    ///         入口程序集设置 <see cref="AppConst.AppAssembly" />，不进
    ///         <see cref="AppConst.AdditionalAssemblies" />——后者是 <c>AllAssemblies</c> 里排在
    ///         <c>AppAssembly</c> 之后的补充项，把入口程序集塞进去会重复登记。
    ///     </para>
    ///     <para>
    ///         <c>Program.cs</c> 里通常仍会写一遍 <c>AppConst.AppAssembly = ...</c>；
    ///         模块初始化器先执行、其后被覆盖为同一程序集，语义一致，不会冲突。
    ///     </para>
    /// </summary>
    public static void RegisterAppAssembly(Type? routeType)
    {
        if (routeType is null)
        {
            return;
        }

        AppConst.AppAssembly = routeType.Assembly;
        registeredAssemblies.Add(routeType.Assembly);
    }

    /// <summary>该程序集是否已由生成器登记过；已登记时反射兜底可以跳过它的扫描。</summary>
    public static bool IsAssemblyRegistered(Assembly assembly) => registeredAssemblies.Contains(assembly);
}

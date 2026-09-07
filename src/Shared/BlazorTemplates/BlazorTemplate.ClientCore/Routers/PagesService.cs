using BlazorTemplate.ClientCore.Store.Models;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BlazorTemplate.ClientCore.Routers;

[AutoInject(LifeTime = InjectLifeTime.Singleton)]
public partial class PagesService
{
    public List<RouteMeta> Pages { get; } = [];
    public List<RouteMeta> Groups { get; } = [];
    public PagesService()
    {
        // 编译期登记优先：源生成器已为显式标注 [Route] 的类型生成好 RouteMeta，直接取用。
        // Groups 先灌入编译期分组，反射补充时 TryAddGroup 会命中它们并只补 Icon，语义不变。
        List<RouteMeta> routes = [.. PageRouteContext.GeneratedPages];
        Groups.AddRange(PageRouteContext.GeneratedGroups);

        // 反射兜底：只补充生成器没覆盖到的类型（razor 的 @page、外部程序集、开放泛型等）。
        foreach (var assembly in AppConst.AllAssemblies)
        {
            var missed = assembly.ExportedTypes
                .Where(t => !PageRouteContext.IsGenerated(t) && t.GetCustomAttribute<RouteAttribute>() != null);
            routes.AddRange(missed.SelectMany(CollectRouteMeta));
        }

        Pages = [.. Groups.Concat(routes).OrderBy(m => m.Sort)];
    }

    private IEnumerable<RouteMeta> CollectRouteMeta(Type t)
    {
        var routerAttr = t.GetCustomAttribute<RouteAttribute>()!;
        var info = t.GetCustomAttribute<PageInfoAttribute>();
        var groupInfo = t.GetCustomAttribute<PageGroupAttribute>();
        var authorizeAttr = t.GetCustomAttribute<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>(false);
        var allowAnonymousAttr = t.GetCustomAttribute<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>();
        var layout = t.GetCustomAttribute<LayoutAttribute>();
        if (groupInfo != null)
        {
            ArgumentNullException.ThrowIfNull(info, $"{nameof(PageGroupAttribute)} should used with {nameof(PageInfoAttribute)}");
            TryAddGroup(groupInfo);
        }

        yield return new()
        {
            RouteId = info?.Id ?? t.Name,
            RouteTitle = info?.Title ?? t.Name,
            RouteUrl = routerAttr.Template,
            Icon = info?.Icon,
            Pin = info?.Pin ?? false,
            Group = info?.GroupId ?? groupInfo?.Id ?? "ROOT",
            Sort = info?.Sort ?? 0,
            HasPageInfo = info != null,
            RouteType = t,
            Layout = layout?.LayoutType,
            IsStaticPath = !CheckPathTemplate(routerAttr.Template),
            IsAllowAnonymous = allowAnonymousAttr != null || authorizeAttr is null,
        };
    }

    private static bool CheckPathTemplate(string template)
    {
        return MatchPathParameter().IsMatch(template);
    }

    private void TryAddGroup(PageGroupAttribute groupInfo)
    {
        var g = Groups.Find(g => g.RouteId == groupInfo.Id);
        if (g == default)
        {
            g = new RouteMeta
            {
                RouteId = groupInfo.Id,
                RouteTitle = groupInfo.Name
            };
            if (groupInfo.Icon != null)
                g.Icon = groupInfo.Icon;
            g.Sort = groupInfo.Sort;
            g.Group = "ROOT";
            g.HasPageInfo = true;
            g.IsAllowAnonymous = true;
            g.IsGroupHeader = true;
            Groups.Add(g);
        }
        else
        {
            if (string.IsNullOrEmpty(g.Icon) && groupInfo.Icon != null)
            {
                g.Icon = groupInfo.Icon;
            }
        }
    }

    [GeneratedRegex(@"\{[^{}]+\}")]
    private static partial Regex MatchPathParameter();
}

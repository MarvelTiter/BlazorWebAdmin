using Project.Constraints.Common.Attributes;
using Project.Constraints.Store.Models;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Project.Web.Shared.Routers;

[AutoInject(LifeTime = InjectLifeTime.Singleton)]
public partial class PagesService
{
    public List<RouteMeta> Pages { get; } = [];
    public List<RouteMeta> Groups { get; } = [];
    public PagesService()
    {
        List<RouteMeta> routes = [];
        foreach (var assembly in AppConst.AllAssemblies)
        {
            routes.AddRange(assembly.ExportedTypes.Where(t => t.GetCustomAttribute<RouteAttribute>() != null).SelectMany(CollectRouteMeta));
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
        if (groupInfo != null)
        {
            ArgumentNullException.ThrowIfNull(info, $"{nameof(PageGroupAttribute)} should used with {nameof(PageInfoAttribute)}");
            TryAddGroup(groupInfo);
        }
        //ArgumentOutOfRangeException.ThrowIfEqual(false, HasGroup(info), $"invalid groupId ({info!.GroupId})");
        //TryForceShowGroup(info);
        if (CheckPathTemplate(routerAttr.Template))
        {
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
                //ForceShowOnNavMenu = info?.ForceShowOnNavMenu ?? false,
                IsAllowAnonymous = allowAnonymousAttr != null || authorizeAttr is null,
            };
        }
    }

    private static bool CheckPathTemplate(string template)
    {
        return !MatchPathParameter().Match(template).Success;
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

    //private static void TryForceShowGroup(PageInfoAttribute? pageInfo)
    //{
    //    var g = Groups.Find(g => g.RouteId == pageInfo?.GroupId);
    //    if (g is not null && !g.ForceShowOnNavMenu)
    //    {
    //        g.ForceShowOnNavMenu = pageInfo?.ForceShowOnNavMenu ?? false;
    //    }
    //}

    [GeneratedRegex(@"\{[^{}]+\}")]
    private static partial Regex MatchPathParameter();
}

using Project.Constraints.Common.Attributes;
using Project.Constraints.Store.Models;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Project.Web.Shared.Routers;

internal static partial class AllPages
{
    public static IList<RouterMeta> Pages { get; } = [];
    public static List<RouterMeta> Groups { get; } = [];
    static AllPages()
    {
        List<RouterMeta> routes = [];
        foreach (var assembly in AppConst.AllAssemblies)
        {
            routes.AddRange(assembly.ExportedTypes.Where(t => t.GetCustomAttribute<RouteAttribute>() != null).SelectMany(GetRouterMeta));
        }
        Pages = [.. Groups.Concat(routes).OrderBy(m => m.Sort)];
    }

    private static IEnumerable<RouterMeta> GetRouterMeta(Type t)
    {
        var routerAttr = t.GetCustomAttribute<RouteAttribute>()!;
        var info = t.GetCustomAttribute<PageInfoAttribute>();
        var groupInfo = t.GetCustomAttribute<PageGroupAttribute>();

        if (groupInfo != null)
        {
            ArgumentNullException.ThrowIfNull(info, $"{nameof(PageGroupAttribute)} should used with {nameof(PageInfoAttribute)}");
            TryAddGroup(groupInfo);
        }
        //ArgumentOutOfRangeException.ThrowIfEqual(false, HasGroup(info), $"invalid groupId ({info!.GroupId})");
        TryForceShowGroup(info);
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
                ForceShowOnNavMenu = info?.ForceShowOnNavMenu ?? false
            };
        }

    }

    private static bool CheckPathTemplate(string template)
    {
        return !MatchPathParameter().Match(template).Success;
    }

    //static bool HasGroup(PageInfoAttribute? pageInfo)
    //{
    //    if (pageInfo == null || pageInfo.GroupId == null)
    //    {
    //        return true;
    //    }
    //    return Groups.Any(g => g.RouteId == pageInfo.GroupId);
    //}
    private static void TryAddGroup(PageGroupAttribute groupInfo)
    {
        var g = Groups.Find(g => g.RouteId == groupInfo.Id);
        if (g == null)
        {
            g = new RouterMeta
            {
                RouteId = groupInfo.Id,
                RouteTitle = groupInfo.Name
            };
            if (groupInfo.Icon != null)
                g.Icon = groupInfo.Icon;
            g.Sort = groupInfo.Sort;
            g.Group = "ROOT";
            g.HasPageInfo = true;
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

    private static void TryForceShowGroup(PageInfoAttribute? pageInfo)
    {
        var g = Groups.Find(g => g.RouteId == pageInfo?.GroupId);
        if (g is not null && !g.ForceShowOnNavMenu)
        {
            g.ForceShowOnNavMenu = pageInfo?.ForceShowOnNavMenu ?? false;
        }
    }

    [GeneratedRegex(@"\{[^{}]+\}")]
    private static partial Regex MatchPathParameter();
}

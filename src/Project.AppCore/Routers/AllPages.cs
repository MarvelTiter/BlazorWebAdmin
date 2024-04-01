using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Constraints.Store.Models;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Project.AppCore.Routers
{
    public static class AllPages
    {
        public static IList<Assembly> Assemblies { get; }
        public static IList<RouterMeta> AllRoutes { get; }
        public static IEnumerable<RouteMenu> RouteMenu { get; } = new List<RouteMenu>();
        static IList<RouterMeta> Groups { get; }
        static AllPages()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                Assemblies = new List<Assembly>();
                return;
            }
            var referencedAssemblies = entryAssembly.GetReferencedAssemblies().Select(Assembly.Load);
            Assemblies = new List<Assembly> { entryAssembly }.Union(referencedAssemblies).ToList();
            List<RouterMeta> routes = new();
            Groups = new List<RouterMeta>();
            foreach (var assembly in Assemblies)
            {
                routes.AddRange(assembly.ExportedTypes.Where(t => t.GetCustomAttribute<RouteAttribute>() != null).SelectMany(GetRouterMeta));
            }
            AllRoutes = routes.Concat(Groups).OrderBy(r => r.Sort).ToList();
        }

        private static IEnumerable<RouterMeta> GetRouterMeta(Type t)
        {
            var routerAttr = t.GetCustomAttribute<RouteAttribute>();
            var info = t.GetCustomAttribute<PageInfoAttribute>();
            var groupInfo = t.GetCustomAttribute<PageGroupAttribute>();

            if (groupInfo != null)
            {
                ArgumentNullException.ThrowIfNull(info, $"{nameof(PageGroupAttribute)} should used with {nameof(PageInfoAttribute)}");
                TryAddGroup(groupInfo);
            }
            //ArgumentOutOfRangeException.ThrowIfEqual(false, HasGroup(info), $"invalid groupId ({info!.GroupId})");

            yield return new()
            {
                RouteId = info?.Id ?? t.FullName!,
                RouteTitle = info?.Title ?? t.Name,
                RouteUrl = routerAttr!.Template,
                Icon = info?.Icon ?? "",
                Pin = info?.Pin ?? false,
                Group = info?.GroupId ?? groupInfo?.Id ?? "ROOT",
                Sort = info?.Sort ?? 0,
                HasPageInfo = info != null,
            };
        }
        static bool HasGroup(PageInfoAttribute? pageInfo)
        {
            if (pageInfo == null || pageInfo.GroupId == null)
            {
                return true;
            }
            return Groups.Any(g => g.RouteId == pageInfo.GroupId);
        }
        private static void TryAddGroup(PageGroupAttribute groupInfo)
        {
            var g = Groups.FirstOrDefault(g => g.RouteId == groupInfo.Id);
            if (g == null)
            {
                g = new RouterMeta();
                g.RouteId = groupInfo.Id;
                g.RouteTitle = groupInfo.Name;
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
    }
}

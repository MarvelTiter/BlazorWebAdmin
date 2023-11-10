using Microsoft.AspNetCore.Components;
using Project.Common.Attributes;
using System.Reflection;

namespace Project.AppCore.Routers
{
    public static class AllPages
    {
        public static IList<Assembly> Assemblies { get; }
        public static IList<RouterMeta> AllRoutes { get; }
        public static IEnumerable<RouteMenu> RouteMenu { get; } = new List<RouteMenu>();

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
            foreach (var assembly in Assemblies)
            {
                routes.AddRange(assembly.ExportedTypes.Where(t => t.GetCustomAttribute<RouteAttribute>() != null).Select(t => GetRouterMeta(t)));
            }
            AllRoutes = routes.OrderBy(r => r.Sort).ToList();

            InitRouteMenu();
        }

        private static RouterMeta GetRouterMeta(Type t)
        {
            var routerAttr = t.GetCustomAttribute<RouteAttribute>();
            var info = t.GetCustomAttribute<PageInfoAttribute>();
            var groupInfo = t.GetCustomAttribute<PageGroupAttribute>();
            //var template = routerAttr!.Template;
            RouterMeta meta = new()
            {
                RouteId = info?.Id ?? t.FullName!,
                RouteUrl = routerAttr!.Template,
                RouteTitle = info?.Title ?? t.Name,
                Icon = groupInfo?.Icon ?? info?.Icon ?? "",
                Pin = info?.Pin ?? false,
                Group = groupInfo?.Name ?? info?.Group,
                Sort = groupInfo?.Sort ?? info?.Sort ?? 0,
                Ignore = info == null,
            };
            return meta;
        }

        private static void InitRouteMenu()
        {
            //var roots = AllRoutes.Where(meta => meta.Group == null);
            //var groups = AllRoutes.Where(meta => meta.Group != null).GroupBy(meta => meta.Group).Select(g =>
            // {
            //     return new RouterMeta
            //     {
            //         RouteLink = g.Key!,
            //         RouteName = 
            //     };
            // });
        }
    }
}

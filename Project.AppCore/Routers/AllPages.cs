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
            //var template = routerAttr!.Template;
            RouterMeta meta = new()
            {
                Id = info?.PageId ?? t.Name,
                RouteLink = routerAttr!.Template,
                RouteName = info?.PageTitle ?? t.Name,
                IconName = info?.Icon ?? "",
                Pin = info?.Pin ?? false,
                Group = info?.Group,
                Sort = info?.Sort ?? 0,
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

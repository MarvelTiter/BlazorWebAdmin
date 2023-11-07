using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Project.AppCore.Options;
using Project.AppCore.Services;
using Project.Common;
using Project.Common.Attributes;
using Project.Models.Permissions;
using System.Reflection;

namespace Project.AppCore.Store
{
    public class RouterMeta : ICloneable
    {
        public bool IsActive { get; set; }
        public string RouteLink { get; set; }
        public string IconName { get; set; }
        public string RouteName { get; set; }
        public bool HasChildren => Children != null && Children.Any();
        public string Redirect { get; set; } = "NoRedirect";
        public bool Pin { get; set; }
        public string? Parent { get; set; }
        public bool Menu { get; set; }
        public IEnumerable<RouterMeta> Children { get; set; }

        public object Clone()
        {
            return new RouterMeta
            {
                IsActive = false,
                RouteLink = RouteLink,
                IconName = IconName,
                RouteName = RouteName,
                Redirect = Redirect,
                Pin = Pin,
                Parent = Parent,
            };
        }
    }

    public class TagRoute : RouterMeta
    {
        public bool Closable { get; set; } = true;
        //public CacheItem Content { get; set; } = new CacheItem();
        public DateTime StartTime { get; set; }
        public DateTime ActiveTime { get; set; }
        public TimeSpan LifeTime { get; set; }
        public RenderFragment? Body { get; set; }
        public RenderFragment? Title { get; set; }
        public string ItemClass => ClassHelper.Default
            .AddClass("main_content")
            .AddClass("active", () => IsActive).Class;

        public void SetActive(bool active)
        {
            IsActive = active;
            //if (active && Content != null) Content.ActiveTime = DateTime.Now;
            if (active) ActiveTime = DateTime.Now;
        }
    }


    public static class AllPages
    {
        public static IList<Assembly> Assemblies { get; }
        public static IList<RouterMeta> AllRoutes { get; }
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
            AllRoutes = routes;
        }

        private static RouterMeta GetRouterMeta(Type t)
        {
            var routerAttr = t.GetCustomAttribute<RouteAttribute>();
            var info = t.GetCustomAttribute<PageInfoAttribute>();
            var template = routerAttr!.Template;
            RouterMeta meta = new()
            {
                RouteLink = template.Length > 1 ? template[1..] : template,
                RouteName = info?.Title ?? t.Name,
                IconName = info?.Icon ?? "",
                Pin = info?.Pin ?? false,
                Parent = info?.Parent,
                Menu = info?.IsMenu ?? false,
            };
            return meta;
        }
    }

    public class RouterStore : StoreBase
    {
        private readonly IPermissionService permissionService;
        private readonly IStringLocalizer<RouterStore> localizer;
        private readonly IOptionsMonitor<CultureOptions> options;
        private readonly IOptionsMonitor<Token> token;

        public RouterStore(IPermissionService permissionService
            , IStringLocalizer<RouterStore> localizer
            , IOptionsMonitor<CultureOptions> options
            , IOptionsMonitor<Token> token)
        {
            this.permissionService = permissionService;
            this.localizer = localizer;
            this.options = options;
            this.token = token;
        }
        public List<TagRoute> TopLink { get; set; } = new List<TagRoute>();

        public List<RouterMeta> Routers { get; set; } = new List<RouterMeta>();

        public int Count { get; set; }

        public TagRoute? Current => TopLink.FirstOrDefault(r => r.IsActive);

        protected override void Release()
        {
            TopLink.Clear();
            Routers.Clear();
        }

        public void SetActive(string link)
        {
            if (link == "") link = "/";
            TopLink.ForEach(a => a.SetActive(a.IsActive = link == a.RouteLink));
            NotifyChanged();
            //return Task.CompletedTask;
        }
        private RouterMeta FindRecursive(IEnumerable<RouterMeta> groups, string link)
        {
            if (!groups.Any()) return null;

            foreach (var item in groups)
            {
                if (!string.IsNullOrEmpty(item.RouteLink) && link == item.RouteLink)
                {
                    return item;
                }
                if (item.HasChildren)
                {
                    var result = FindRecursive(item.Children, link);
                    if (result != null)
                        return result;
                    else
                        continue;
                }
            }
            return null;
        }
        public void TryAdd(string link)
        {
            if (link == "") link = "/";
            var route = FindRecursive(Routers, link) ?? AllPages.AllRoutes.FirstOrDefault(r => r.RouteLink == link);
            if (route != null && !TopLink.Any(x => link == x.RouteLink))
            {
                TopLink.Add(new TagRoute
                {
                    RouteLink = route.RouteLink,
                    RouteName = route.RouteName,
                    Closable = !route.Pin
                });
            }
            SetActive(link);
        }

        public void Remove(string link)
        {
            var index = TopLink.FindIndex(rs => rs.RouteLink == link);
            TopLink.RemoveAt(index);
            if (index < TopLink.Count)
            {
                // 跳到后一个标签
                SetActive(TopLink[index].RouteLink);
            }
            else
            {
                if (TopLink.Count > 0)
                    SetActive(TopLink[index - 1].RouteLink);
            }
            NotifyChanged();
        }

        string GetLocalizerString(Power power)
        {
            if (options.CurrentValue.Enabled) return localizer[power.PowerId];
            else return power.PowerName;
        }

        string GetLocalizerString(string key, string defaultValue)
        {
            if (options.CurrentValue.Enabled) return localizer[key];
            else return defaultValue;
        }

        string GetHomeLocalizer()
        {
            if (options.CurrentValue.Enabled) return localizer["Home"];
            else return "主页";
        }

        public Task RemoveOther(string link)
        {
            var removes = TopLink.Where(r => r.RouteLink != link).ToArray();
            foreach (var item in removes)
            {
                if (item.Closable)
                    TopLink.Remove(item);
            }
            NotifyChanged();
            return Task.CompletedTask;
        }

        public Task Reset()
        {
            TopLink.Clear();
            TopLink.Add(new TagRoute
            {
                RouteLink = "/",
                RouteName = GetHomeLocalizer(),
                Closable = false,
            });
            return Task.CompletedTask;
        }
        //TODO 获取权限列表
        public Task InitRoutersAsync(UserInfo? userInfo)
        {
            if (!token.CurrentValue.NeedAuthentication)
            {
                InitRoutersByDefault();
                return Task.CompletedTask;
            }
            else
            {
                return InitRoutersAsyncByUser(userInfo);
            }
        }

        private void InitRoutersByDefault()
        {
            Routers = new List<RouterMeta>();
            var roots = AllPages.AllRoutes.Where(meta => meta.Menu && meta.Parent == null);
            var groups = AllPages.AllRoutes.Where(meta => meta.Menu && meta.Parent != null).GroupBy(meta => meta.Parent).Select(g =>
             {
                 return new RouterMeta
                 {
                     RouteLink = g.Key!,
                     RouteName = GetLocalizerString(g.Key!, g.Key!)
                 };
             });
            roots = roots.Concat(groups);
            foreach (var item in roots)
            {
                var route = (RouterMeta)item.Clone();
                route.Children = FindChildren(item);
                Routers.Add(route);
            }

            List<RouterMeta> FindChildren(RouterMeta parent)
            {
                var children = AllPages.AllRoutes.Where(p => p.Parent == parent.RouteLink);
                List<RouterMeta> childNodes = new();
                foreach (var child in children)
                {
                    var n1 = (RouterMeta)child.Clone();
                    n1.Children = FindChildren(child);
                    childNodes.Add(n1);
                }
                return childNodes;
            }
        }

        private async Task InitRoutersAsyncByUser(UserInfo? userInfo)
        {
            if (userInfo == null) return;
            await Reset();
            var result = await permissionService.GetPowerListByUserIdAsync(userInfo.UserId);
            var powers = result.Payload.Where(p => p.PowerType == PowerType.Page);
            var roots = powers.Where(p => p.ParentId == "ROOT");
            Routers = new List<RouterMeta>
            {
                new RouterMeta
                {
                    RouteLink = "/",
                    IconName = "home",
                    RouteName =GetHomeLocalizer(),
                    Children = new List<RouterMeta>()
                }
            };
            foreach (var item in roots)
            {
                var n = new RouterMeta();
                n.RouteName = GetLocalizerString(item);
                n.RouteLink = item.Path;
                n.IconName = item.Icon;
                n.Children = FindChildren(powers, item);
                Routers.Add(n);
            }

            List<RouterMeta> FindChildren(IEnumerable<Power> all, Power parent)
            {
                var children = all.Where(p => p.ParentId == parent.PowerId);
                List<RouterMeta> childNodes = new();
                foreach (var child in children)
                {
                    var n1 = new RouterMeta();
                    n1.RouteName = GetLocalizerString(child);
                    n1.RouteLink = child.Path;
                    n1.IconName = child.Icon;
                    n1.Children = FindChildren(all, child);
                    childNodes.Add(n1);
                }
                return childNodes;
            }
        }
    }
}

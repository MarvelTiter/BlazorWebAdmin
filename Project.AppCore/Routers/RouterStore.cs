using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Project.AppCore.Options;
using Project.AppCore.Services;
using Project.AppCore.Store;
using Project.Models.Permissions;
using System.Text.RegularExpressions;

namespace Project.AppCore.Routers
{
    public class RouterStore : StoreBase
    {
        private readonly IPermissionService permissionService;
        private readonly NavigationManager navigationManager;
        private readonly IStringLocalizer<RouterStore> localizer;
        private readonly IOptionsMonitor<CultureOptions> options;
        private readonly IOptionsMonitor<Token> token;

        public RouterStore(IPermissionService permissionService
            , NavigationManager navigationManager
            , IStringLocalizer<RouterStore> localizer
            , IOptionsMonitor<CultureOptions> options
            , IOptionsMonitor<Token> token)
        {
            this.permissionService = permissionService;
            this.navigationManager = navigationManager;
            this.localizer = localizer;
            this.options = options;
            this.token = token;
        }


        readonly Dictionary<string, TagRoute> pages = new();

        public List<TagRoute> TopLinks => pages.Values.ToList();

        public List<RouteMenu> Menus { get; set; } = new List<RouteMenu>();

        public int Count { get; set; }

        public TagRoute? Current => pages.ContainsKey(CurrentUrl) ? pages[CurrentUrl] : null;

        protected override void Release()
        {
            pages.Clear();
            Menus.Clear();
        }

        public string CurrentUrl
        {
            get => "/" + navigationManager.ToBaseRelativePath(navigationManager.Uri);
        }

        public Task RouteDataChangedHandleAsync(Microsoft.AspNetCore.Components.RouteData routeData)
        {
            if (!pages.TryGetValue(CurrentUrl, out var tag))
            {
                var meta = AllPages.AllRoutes.First(r => r.RouteLink == CurrentUrl);
                tag = new TagRoute
                {
                    RouteLink = meta.RouteLink,
                    RouteName = meta.RouteName,
                    Pin = meta.Pin,
                };
                pages[CurrentUrl] = tag;
            }
            tag.Body ??= CreateBody(tag, routeData);
            return Task.CompletedTask;
        }

        private RenderFragment CreateBody(TagRoute? route, Microsoft.AspNetCore.Components.RouteData routeData)
        {
            var pagetype = routeData.PageType;
            var routeValues = routeData.RouteValues;
            void RenderForLastValue(RenderTreeBuilder builder)
            {                //dont reference RouteData again
                var seq = 0;
                builder.OpenComponent(seq++, pagetype);
                foreach (KeyValuePair<string, object> routeValue in routeValues)
                {
                    builder.AddAttribute(seq++, routeValue.Key, routeValue.Value);
                }
                builder.AddComponentReferenceCapture(seq++, obj =>
                {
                    if (route != null)
                        route.PageRef = obj;
                });
                builder.CloseComponent();
            }
            return RenderForLastValue;
        }

        public void TryAddTopLink(string url, string title)
        {
            if (!pages.TryGetValue(CurrentUrl, out var tag))
            {
                var meta = AllPages.AllRoutes.First(r => r.RouteLink == CurrentUrl);
                tag = new TagRoute
                {
                    RouteLink = url,
                    RouteName = title,
                    Pin = meta.Pin,
                };
                pages[CurrentUrl] = tag;
            }
            NotifyChanged();
        }

        public void Remove(string link)
        {
            pages.Remove(link);
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
            var removeKeys = pages.Keys.Where(k => k != link);
            foreach (var key in removeKeys)
            {
                if (pages[key].Pin) continue;
                pages.Remove(key);
            }
            NotifyChanged();
            return Task.CompletedTask;
        }

        public Task Reset()
        {
            pages.Clear();
            pages.Add("/", new TagRoute
            {
                RouteLink = "/",
                RouteName = GetHomeLocalizer(),
                Pin = true
            });
            return Task.CompletedTask;
        }
        //TODO 获取权限列表
        public async Task InitRoutersAsync(UserInfo? userInfo)
        {
            if (userInfo != null)
            {
                await InitRoutersAsyncByUser(userInfo);
            }
            InitRoutersByDefault();
        }

        private void InitRoutersByDefault()
        {
            foreach (var item in AllPages.AllRoutes.Where(m => !m.Ignore))
            {
                if (Menus.Any(m => m.RouteLink == item.RouteLink))
                {
                    continue;
                }
                item.RouteName = GetLocalizerString(item.Id ?? item.RouteName, item.RouteName);
                item.Group ??= "ROOT";
                Menus.Add(new RouteMenu(item));
            }
        }

        private async Task InitRoutersAsyncByUser(UserInfo? userInfo)
        {
            if (userInfo == null) return;
            await Reset();
            var result = await permissionService.GetPowerListByUserIdAsync(userInfo.UserId);
            var powers = result.Payload.Where(p => p.PowerType == PowerType.Page);
            Menus.Clear();
            Menus.Add(new()
            {
                Id = "Dashboard",
                RouteLink = "/",
                IconName = "home",
                Group = "ROOT",
                RouteName = GetHomeLocalizer(),
            });
            foreach (var pow in powers)
            {
                var meta = AllPages.AllRoutes.First(m => m.RouteLink == "/" + pow.Path);
                if (meta == null) continue;
                meta.RouteName = GetLocalizerString(pow);
                meta.Id = pow.PowerId;
                meta.IconName = pow.Icon;
                meta.Group = pow.ParentId;
                meta.Sort = pow.Sort;
                Menus.Add(new(meta));
            }
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Project.AppCore.Options;
using Project.AppCore.PageHelper;
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
        private readonly ILogger<RouterStore> logger;
        private readonly IOptionsMonitor<AppSetting> setting;

        public RouterStore(IPermissionService permissionService
            , NavigationManager navigationManager
            , IStringLocalizer<RouterStore> localizer
            , IOptionsMonitor<CultureOptions> options
            , ILogger<RouterStore> logger
            , IOptionsMonitor<AppSetting> setting)
        {
            this.permissionService = permissionService;
            this.navigationManager = navigationManager;
            this.localizer = localizer;
            this.options = options;
            this.logger = logger;
            this.setting = setting;
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
        TagRoute? preview;
        public Task RouteDataChangedHandleAsync(Microsoft.AspNetCore.Components.RouteData routeData)
        {
            if (!pages.TryGetValue(CurrentUrl, out var tag))
            {
                var meta = Menus.FirstOrDefault(r => r.RouteUrl == CurrentUrl)
                    ?? AllPages.AllRoutes.First(r => r.RouteUrl == CurrentUrl);
                tag = new TagRoute
                {
                    RouteUrl = meta.RouteUrl,
                    RouteTitle = meta.RouteTitle,
                    Icon = meta.Icon,
                    Pin = meta.Pin,
                };
                pages[CurrentUrl] = tag;
            }
            tag.Body ??= CreateBody(tag, routeData);
            preview?.SetActive(false);
            tag.SetActive(true);
            preview = tag;
            NotifyChanged();
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

        public void Remove(string link)
        {
            pages.Remove(link);
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
                RouteUrl = "/",
                RouteTitle = GetHomeLocalizer(),
                Icon = "home",
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
            if (setting.CurrentValue.LoadUnregisteredPage)
            {
                InitRoutersByDefault();
            }
        }

        private void InitRoutersByDefault()
        {
            foreach (var item in AllPages.AllRoutes.Where(m => m.HasPageInfo).OrderBy(m => m.Sort))
            {
                if (Menus.Any(m => m.RouteUrl == item.RouteUrl && m.RouteId == item.RouteId))
                {
                    continue;
                }
                var title = GetLocalizerString(item.RouteId, item.RouteTitle);
                if (title == item.RouteId)
                    title = item.RouteTitle;
                item.RouteTitle = title;
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
                RouteId = "Dashboard",
                RouteUrl = "/",
                Icon = "home",
                Group = "ROOT",
                RouteTitle = GetHomeLocalizer(),
            });
            foreach (var pow in powers)
            {
                var meta = AllPages.AllRoutes.FirstOrDefault(m => m.RouteUrl == "/" + pow.Path);
                if (meta == null)
                    continue;
                meta.RouteTitle = GetLocalizerString(pow.PowerId, pow.PowerName);
                meta.RouteId = pow.PowerId;
                meta.Icon = pow.Icon;
                meta.Group = pow.ParentId;
                meta.Sort = pow.Sort;
                Menus.Add(new(meta));
            }
        }
    }
}

using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using Project.Constraints.Common.Attributes;
using Project.Constraints.Options;
using Project.Constraints.PageHelper;
using Project.Constraints.Store.Models;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Components;
using Project.Web.Shared.Store;
using System.Reflection;
using static Project.Web.Shared.Routers.RouterStoreExtensions;
namespace Project.Web.Shared.Routers;

public static class RouterStoreExtensions
{
    public static bool CompareUrl(this IRouterStore store, string? url)
    {
        return CompareUrl(store.CurrentUrl, url);
    }

    public static bool Compare(this IRouterStore store, TagRoute other)
    {
        return store.Current == other;
    }

    public static bool CompareUrl(string? url1, string? url2)
    {
        if (string.IsNullOrEmpty(url1) || string.IsNullOrEmpty(url2)) return false;
        if (!url1.StartsWith('/')) url1 = '/' + url1;
        if (!url2.StartsWith('/')) url2 = '/' + url2;
        return url1 == url2;
    }
}

[AutoInject(ServiceType = typeof(IRouterStore))]
public class RouterStore(IProjectSettingService settingService
        , NavigationManager navigationManager
        , IUserStore userStore
        , IStringLocalizer<RouterStore> localizer
        , IOptionsMonitor<CultureOptions> options
        , ILogger<RouterStore> logger
        , IOptionsMonitor<AppSetting> setting) : StoreBase, IRouterStore
{
    readonly Dictionary<string, TagRoute> pages = [];
    //private FrozenDictionary<string, RouteMenu>? frozenMenus;
    private List<RouteMenu> menus = [];
    public List<TagRoute> TopLinks => [.. pages.Values];

    public IEnumerable<RouteMenu> Menus => menus;// frozenMenus?.Values ?? [];

    public int Count { get; set; }

    public TagRoute? Current => pages.TryGetValue(CurrentUrl, out TagRoute? value) ? value : null;

    protected override void Release()
    {
        pages.Clear();
    }

    public string CurrentUrl
    {
        get
        {
            var url = navigationManager.ToBaseRelativePath(navigationManager.Uri);
            if (url.IndexOf('?') > 0)
            {
                return '/' + url[0..url.IndexOf('?')];
            }
            return '/' + url;
        }
    }
    private static string? AttachFirstSlash(string? url)
    {
        if (string.IsNullOrEmpty(url)) return null;
        if (url.StartsWith('/')) return url;
        return "/" + url;
    }

    TagRoute? preview;
    public async Task RouteDataChangedHandleAsync(RouteData routeData)
    {
        if (!pages.TryGetValue(CurrentUrl, out var tag))
        {
            // TODO 可能有BUG，先观察观察
            if (menus.Count == 0) return;
            RouterMeta? meta = menus.FirstOrDefault(r => CompareUrl(r.RouteUrl, CurrentUrl));
            if (meta == null)
            {
                meta = AllPages.Pages.FirstOrDefault(r => CompareUrl(r.RouteUrl, CurrentUrl));
                if (meta != null)
                    meta.Cache = false;
            }
            tag = new TagRoute
            {
                RouteId = meta?.RouteId ?? CurrentUrl,
                RouteUrl = AttachFirstSlash(meta?.RouteUrl ?? CurrentUrl),
                RouteTitle = meta?.RouteTitle,
                Icon = meta?.Icon ?? "",
                Pin = meta?.Pin ?? false,
                Cache = meta?.Cache ?? false,
            };
            pages[CurrentUrl] = tag;
        }

        var enable = await OnRouterChangingAsync(tag);
        if (enable)
        {
            tag.Body ??= CreateBody(tag, routeData);
            tag.Panic = false;
        }
        else
        {
            // 不允许导航到此页面
            tag.Body ??= b => b.Component<ForbiddenPage>().Build();
        }
        if (preview != null)
        {
            if (preview.Panic || !preview.Cache)
            {
                preview.Body = null;
            }
            if (!preview.Rendered && preview.RouteUrl != CurrentUrl)
            {
                preview.Drop();
            }
            else
            {
                preview.SetActive(false);
            }
        }
        //preview?.SetActive(false);
        tag.SetActive(true);
        preview = tag;
        NotifyChanged();
    }

    private RenderFragment CreateBody(TagRoute? route, RouteData routeData)
    {
        var pagetype = routeData.PageType;
        var routeValues = routeData.RouteValues;
        void RenderForLastValue(RenderTreeBuilder builder)
        {   //dont reference RouteData again
            builder.OpenComponent(0, pagetype);
            foreach (KeyValuePair<string, object?> routeValue in routeValues)
            {
                builder.AddAttribute(1, routeValue.Key, routeValue.Value);
            }
            builder.AddComponentReferenceCapture(2, obj =>
            {
                CollectPageAdditionalInfo(route, obj);
            });
            builder.CloseComponent();
        }
        return RenderForLastValue;
    }

    private void CollectPageAdditionalInfo(TagRoute? route, object obj)
    {
        if (route != null)
        {
            route.PageRef = obj;
            route.Rendered = true;
            if (obj is IRoutePageTitle page)
            {
                route.Title = page.GetTitle();
            }
            if (route.RouteTitle is null)
            {
                var tta = obj.GetType().GetCustomAttribute<TagTitleAttribute>();
                if (tta != null)
                {
                    route.Title = tta.Title.AsContent();
                }
                else
                {
                    route.Title = CurrentUrl.AsContent();
                }
            }
            NotifyChanged();
        }
    }

    public void Remove(string link)
    {
        if (pages.TryGetValue(link, out var p))
        {
            p.Drop();
        }
        pages.Remove(link);
    }

    public string GetLocalizerString(RouterMeta meta)
    {
        if (options.CurrentValue.Enabled)
        {
            var l = localizer[meta.RouteId];
            if (!string.Equals(l, meta.RouteId))
            {
                return l;
            }
            //return localizer[meta.RouteId];
        }
        return meta.RouteTitle;
    }

    public Task RemoveOther(string link)
    {
        var removeKeys = pages.Keys.Where(k => k != link);
        foreach (var key in removeKeys)
        {
            if (pages[key].Pin) continue;
            if (pages.TryGetValue(key, out var p))
            {
                p.Drop();
            }
            pages.Remove(key);
        }
        NotifyChanged();
        return Task.CompletedTask;
    }

    public Task Reload()
    {
        if (Current is null)
        {
            return Task.CompletedTask;
        }
        Current.Drop();
        navigationManager.NavigateTo(CurrentUrl);
        return Task.CompletedTask;
    }

    public Task Reset()
    {
        pages.Clear();
        var homeTag = new TagRoute
        {
            RouteUrl = "/",
            RouteId = "Home",
            RouteTitle = "主页",
            Icon = "svg-home",
            Pin = true,
            IsActive = true
        };
        pages.Add("/", homeTag);
        preview = homeTag;
        return Task.CompletedTask;
    }
    public async Task InitRoutersAsync(UserInfo? userInfo)
    {
        try
        {
            await Reset();
            List<RouteMenu> menus = [];
            //menus.Clear();
            menus.Add(new()
            {
                RouteId = "Home",
                RouteUrl = "/",
                Icon = "svg-home",
                Group = "ROOT",
                Cache = true,
                RouteTitle = "主页",
            });
            //if (userInfo != null && setting.CurrentValue.LoadPageFromDatabase)
            //{
            //    await InitRoutersAsyncByUser(userInfo);
            //}
            //if (setting.CurrentValue.LoadUnregisteredPage)
            //{
            //    await InitRoutersByDefault();
            //}

            MinimalPower[] savedInfos = [];
            if (userInfo is not null)
            {
                savedInfos = [.. await settingService.GetUserPowersAsync(userInfo)];
            }
            foreach (var meta in AllPages.Pages.Where(m => m.HasPageInfo).OrderBy(m => m.Sort))
            {
                if (!menus.Any(m => m.RouteId == meta.RouteId))
                {
                    var enable = await OnRouteMetaFilterAsync(meta);
                    if (!enable)
                        continue;
                    if (userInfo?.UserPages.Contains(meta.RouteId) == true
                        || meta.ForceShowOnNavMenu
                        || setting.CurrentValue.LoadUnregisteredPage)
                    {
                        if (savedInfos.Length > 0 && savedInfos.Any(p => p.PowerId == meta.RouteId))
                        {
                            var savedMeta = savedInfos.First(p => p.PowerId == meta.RouteId);
                            meta.Icon = savedMeta.Icon;
                            meta.RouteTitle = savedMeta.PowerName;
                            meta.Sort = savedMeta.Sort;
                        }
                        menus.Add(new RouteMenu(meta));
                    }
                }
            }
            //menus.Sort((a, b) => a.Sort - b.Sort);
            this.menus = [.. menus.OrderBy(m => m.Sort)];
            //frozenMenus = menus.OrderByDescending(a => a.Sort).ToDictionary(m => m.RouteId, m => m).ToFrozenDictionary();
        }
        catch (Exception ex)
        {
            logger.LogError("{Message}", ex.Message);
        }
    }

    //private async Task InitRoutersByDefault(List<RouteMenu> menus)
    //{
    //    foreach (var meta in AllPages.AllRoutes.Where(m => m.HasPageInfo).OrderBy(m => m.Sort))
    //    {
    //        if (!menus.Any(m => m.RouteId == meta.RouteId))
    //        {
    //            var enable = await OnRouteMetaFilterAsync(meta);
    //            if (!enable)
    //                continue;
    //            //var title = GetLocalizerString(meta);
    //            //if (title == meta.RouteId)
    //            //    title = meta.RouteTitle;
    //            //meta.RouteTitle = title;
    //            menus.Add(new RouteMenu(meta));
    //        }
    //    }
    //}

    //private async Task InitRoutersAsyncByUser(UserInfo? userInfo, List<RouteMenu> menus)
    //{
    //    if (userInfo == null) return;
    //    var result = await settingService.GetUserPowersAsync(userInfo);
    //    userInfo.UserPowers = result.Where(p => p.PowerType == PowerType.Button).Select(p => p.PowerId).ToArray();
    //    var powers = result.Where(p => p.PowerType == PowerType.Page);

    //    foreach (var pow in powers)
    //    {
    //        var meta = AllPages.AllRoutes.FirstOrDefault(m => m.RouteId == pow.PowerId);
    //        meta ??= new RouterMeta()
    //        {
    //            RouteUrl = pow.Path,
    //            RouteId = pow.PowerId
    //        };
    //        if (menus.Any(m => m.RouteId == meta.RouteId))
    //        {
    //            continue;
    //        }
    //        var enable = await OnRouteMetaFilterAsync(meta);
    //        if (!enable)
    //            continue;
    //        meta.RouteTitle = pow.PowerName;
    //        meta.Icon = pow.Icon;
    //        meta.Group = pow.ParentId;
    //        meta.Sort = pow.Sort;
    //        meta.Cache = true;
    //        menus.Add(new(meta));
    //    }
    //}

    public event Func<TagRoute, Task<bool>>? RouterChangingEvent;

    private async Task<bool> OnRouterChangingAsync(TagRoute tag)
    {
        bool enable = true;
        if (IsUserDashboard(tag))
        {
            enable = EnableShowUserDashboard(userStore, setting.CurrentValue);
        }
        if (RouterChangingEvent != null)
        {
            //return await RouterChangingEvent.Invoke(tag);
            foreach (Func<TagRoute, Task<bool>> item in RouterChangingEvent.GetInvocationList().Cast<Func<TagRoute, Task<bool>>>())
            {
                enable = enable && await item.Invoke(tag);
            }
        }
        return enable;
    }

    public event Func<RouterMeta, Task<bool>>? RouteMetaFilterEvent;
    private async Task<bool> OnRouteMetaFilterAsync(RouterMeta meta)
    {
        if (IsUserDashboard(meta))
        {
            return EnableShowUserDashboard(userStore, setting.CurrentValue);
        }
        var used = meta.RouteType == null
            || AppConst.AllAssemblies.IndexOf(meta.RouteType.Assembly) > -1
            || meta.RouteType.Assembly == Assembly.GetEntryAssembly()
            || (meta.RouteType.Assembly.GetName().Name?.EndsWith(".Client") ?? false);
        if (RouteMetaFilterEvent != null)
        {
            var enable = await RouteMetaFilterEvent.Invoke(meta);
            return used && enable;
        }
        return used;
    }

    private static bool IsUserDashboard(RouterMeta meta)
    {
        return meta.RouteUrl == "/userdashboard";
    }

    private static bool EnableShowUserDashboard(IUserStore _, AppSetting setting) => setting.ClientHubOptions.Enable;

    public Type? GetRouteType(string routeUrl)
    {
        return AllPages.Pages.FirstOrDefault(meta => meta.RouteUrl == routeUrl)?.RouteType;
    }
}

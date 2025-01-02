using AutoInjectGenerator;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Project.Constraints;
using Project.Constraints.Options;
using Project.Constraints.Store;
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
    readonly Dictionary<string, TagRoute> pages = new();

    public List<TagRoute> TopLinks => [.. pages.Values];

    public List<RouteMenu> Menus { get; set; } = new List<RouteMenu>();

    public int Count { get; set; }

    public TagRoute? Current => pages.TryGetValue(CurrentUrl, out TagRoute? value) ? value : null;

    protected override void Release()
    {
        pages.Clear();
        Menus.Clear();
    }

    public string CurrentUrl => '/' + navigationManager.ToBaseRelativePath(navigationManager.Uri);
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
            if (Menus.Count == 0) return;
            RouterMeta? meta = Menus.FirstOrDefault(r => CompareUrl(r.RouteUrl, CurrentUrl));
            if (meta == null)
            {
                meta = AllPages.AllRoutes.FirstOrDefault(r => CompareUrl(r.RouteUrl, CurrentUrl));
                if (meta != null)
                    meta.Cache = false;
            }
            tag = new TagRoute
            {
                RouteId = meta?.RouteId,
                RouteUrl = AttachFirstSlash(meta?.RouteUrl ?? CurrentUrl),
                RouteTitle = meta?.RouteTitle ?? "",
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
        }
        else
        {
            //TODO 不允许导航到此页面
            tag.Body ??= b => b.Component<ForbiddenPage>().Build();
        }
        if (preview != null)
        {
            if (!preview.Cache)
            {
                Remove(preview.RouteUrl);
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
        {                //dont reference RouteData again
            builder.OpenComponent(0, pagetype);
            foreach (KeyValuePair<string, object?> routeValue in routeValues)
            {
                builder.AddAttribute(1, routeValue.Key, routeValue.Value);
            }
            builder.AddComponentReferenceCapture(2, obj =>
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
            pages.Remove(key);
        }
        NotifyChanged();
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
            Icon = "home",
            Pin = true,
            IsActive = true
        };
        pages.Add("/", homeTag);
        preview = homeTag;
        return Task.CompletedTask;
    }
    //TODO 获取权限列表
    public async Task InitRoutersAsync(UserInfo? userInfo)
    {
        try
        {
            await Reset();
            Menus.Clear();
            Menus.Add(new()
            {
                RouteId = "Home",
                RouteUrl = "/",
                Icon = "home",
                Group = "ROOT",
                Cache = true,
                RouteTitle = "主页",
            });
            if (userInfo != null && setting.CurrentValue.LoadPageFromDatabase)
            {
                await InitRoutersAsyncByUser(userInfo);
            }
            if (setting.CurrentValue.LoadUnregisteredPage)
            {
                await InitRoutersByDefault();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

    private async Task InitRoutersByDefault()
    {
        foreach (var meta in AllPages.AllRoutes.Where(m => m.HasPageInfo).OrderBy(m => m.Sort))
        {
            if (Menus.Any(m => CompareUrl(m.RouteUrl, meta.RouteUrl) && m.RouteId == meta.RouteId))
            {
                continue;
            }
            var enable = await OnRouteMetaFilterAsync(meta);
            if (!enable)
                continue;
            //var title = GetLocalizerString(meta);
            //if (title == meta.RouteId)
            //    title = meta.RouteTitle;
            //meta.RouteTitle = title;
            Menus.Add(new RouteMenu(meta));
        }
    }

    private async Task InitRoutersAsyncByUser(UserInfo? userInfo)
    {
        if (userInfo == null) return;
        var result = await settingService.GetUserPowersAsync(userInfo);
        userInfo.UserPowers = result.Where(p => p.PowerType == PowerType.Button).Select(p => p.PowerId).ToArray();
        var powers = result.Where(p => p.PowerType == PowerType.Page);

        foreach (var pow in powers)
        {
            var meta = AllPages.AllRoutes.FirstOrDefault(m => CompareUrl(pow.Path, m.RouteUrl));
            meta ??= new RouterMeta()
            {
                RouteUrl = pow.Path,
                RouteId = pow.PowerId
            };
            if (Menus.Any(m => m.RouteUrl == meta.RouteUrl && m.RouteId == meta.RouteId))
            {
                continue;
            }
            var enable = await OnRouteMetaFilterAsync(meta);
            if (!enable)
                continue;
            meta.RouteTitle = pow.PowerName;
            meta.Icon = pow.Icon;
            meta.Group = pow.ParentId;
            meta.Sort = pow.Sort;
            meta.Cache = true;
            Menus.Add(new(meta));
        }
    }

    public event Func<TagRoute, Task<bool>>? RouterChangingEvent;

    private async Task<bool> OnRouterChangingAsync(TagRoute tag)
    {
        if (IsUserDashboard(tag))
        {
            return EnableShowUserDashboard(userStore, setting.CurrentValue);
        }
        if (RouterChangingEvent != null)
        {
            return await RouterChangingEvent.Invoke(tag);
        }
        return true;
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
        return AllPages.AllRoutes.FirstOrDefault(meta => meta.RouteUrl == routeUrl)?.RouteType;
    }
}

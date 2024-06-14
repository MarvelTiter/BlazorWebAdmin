using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using Project.AppCore.Store;
using Project.Constraints;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Options;
using Project.Constraints.Store;
using Project.Constraints.Store.Models;
using System.Reflection;

namespace Project.AppCore.Routers;

public class RouterStore : StoreBase, IRouterStore
{
    private readonly IProjectSettingService settingService;
    private readonly NavigationManager navigationManager;
    private readonly IUserStore userStore;
    private readonly IStringLocalizer<RouterStore> localizer;
    private readonly IOptionsMonitor<CultureOptions> options;
    private readonly ILogger<RouterStore> logger;
    private readonly IOptionsMonitor<AppSetting> setting;

    public RouterStore(IProjectSettingService settingService
        , NavigationManager navigationManager
        , IUserStore userStore
        , IStringLocalizer<RouterStore> localizer
        , IOptionsMonitor<CultureOptions> options
        , ILogger<RouterStore> logger
        , IOptionsMonitor<AppSetting> setting)
    {
        this.settingService = settingService;
        this.navigationManager = navigationManager;
        this.userStore = userStore;
        this.localizer = localizer;
        this.options = options;
        this.logger = logger;
        this.setting = setting;
    }


    readonly Dictionary<string, TagRoute> pages = new();

    public List<TagRoute> TopLinks => pages.Values.ToList();

    public List<RouteMenu> Menus { get; set; } = new List<RouteMenu>();

    public int Count { get; set; }

    public TagRoute? Current => pages.TryGetValue(CurrentUrl, out TagRoute? value) ? value : null;

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
    public async Task RouteDataChangedHandleAsync(RouteData routeData)
    {
        if (!pages.TryGetValue(CurrentUrl, out var tag))
        {
            // TODO 可能有BUG，先观察观察
            if (Menus.Count == 0) return;
            RouterMeta? meta = Menus.FirstOrDefault(r => r.RouteUrl == CurrentUrl);
            if (meta == null)
            {
                meta = AllPages.AllRoutes.FirstOrDefault(r => r.RouteUrl == CurrentUrl);
                if (meta != null)
                    meta.Cache = false;
            }
            tag = new TagRoute
            {
                RouteUrl = meta?.RouteUrl ?? CurrentUrl,
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
            //tag.Body ??=
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

    private RenderFragment CreateBody(TagRoute? route, Microsoft.AspNetCore.Components.RouteData routeData)
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
        try
        {
            await Reset();
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
            if (Menus.Any(m => m.RouteUrl == meta.RouteUrl && m.RouteId == meta.RouteId))
            {
                continue;
            }
            var enable = await OnRouteMetaFilterAsync(meta);
            if (!enable)
                continue;
            var title = GetLocalizerString(meta.RouteId, meta.RouteTitle);
            if (title == meta.RouteId)
                title = meta.RouteTitle;
            meta.RouteTitle = title;
            Menus.Add(new RouteMenu(meta));
        }
    }

    private async Task InitRoutersAsyncByUser(UserInfo? userInfo)
    {
        if (userInfo == null) return;
        var result = await settingService.GetUserPowersAsync(userInfo);
        var powers = result.Where(p => p.PowerType == PowerType.Page);
        Menus.Clear();
        Menus.Add(new()
        {
            RouteId = "Dashboard",
            RouteUrl = "/",
            Icon = "home",
            Group = "ROOT",
            Cache = true,
            RouteTitle = GetHomeLocalizer(),
        });
        foreach (var pow in powers)
        {
            var meta = AllPages.AllRoutes.FirstOrDefault(m => m.RouteUrl == "/" + pow.Path);
            if (meta == null)
                continue;
            var enable = await OnRouteMetaFilterAsync(meta);
            if (!enable)
                continue;
            meta.RouteTitle = GetLocalizerString(pow.PowerId, pow.PowerName);
            meta.RouteId = pow.PowerId;
            meta.Icon = pow.Icon;
            meta.Group = pow.ParentId;
            meta.Sort = pow.Sort;
            meta.Cache = true;
            Menus.Add(new(meta));
        }
    }

    public event Func<TagRoute, Task<bool>> RouterChangingEvent;

    private Task<bool> OnRouterChangingAsync(TagRoute tag)
    {
        if (RouterChangingEvent != null)
            return RouterChangingEvent.Invoke(tag);
        return Task.FromResult(true);
    }

    public event Func<RouterMeta, Task<bool>> RouteMetaFilterEvent;
    private async Task<bool> OnRouteMetaFilterAsync(RouterMeta meta)
    {
        var used = meta.RouteType == null
            || AppConst.Pages.IndexOf(meta.RouteType.Assembly) > -1
            || meta.RouteType.Assembly == Assembly.GetEntryAssembly();
        if (RouteMetaFilterEvent != null)
        {
            var enable = await RouteMetaFilterEvent.Invoke(meta);
            return used && enable;
        }
        return used;
    }

    public Type? GetRouteType(string routeUrl)
    {
        return AllPages.AllRoutes.FirstOrDefault(meta => meta.RouteUrl == routeUrl)?.RouteType;
    }
}

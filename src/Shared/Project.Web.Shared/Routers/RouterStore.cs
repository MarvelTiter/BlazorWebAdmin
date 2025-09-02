using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using Project.Constraints.Common.Attributes;
using Project.Constraints.Options;
using Project.Constraints.PageHelper;
using Project.Constraints.Store.Models;
using Project.Constraints.UI.Extensions;
using Project.Constraints.Utils;
using Project.Web.Shared.Components;
using Project.Web.Shared.Store;
using System.Reflection;
using Microsoft.AspNetCore.Components.Routing;
using static Project.Web.Shared.Routers.RouterStoreExtensions;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Web.Shared.Routers;

public partial class RouterStore
{
    private readonly AsyncHandlerManager<RouteTag, bool> routerChangingHandlerManager = new();

    public IDisposable RegisterRouterChangingHandler(Func<RouteTag, Task<bool>> handler)
    {
        return routerChangingHandlerManager.RegisterHandler(handler);
    }

    private async Task<bool> OnRouterChangingAsync(RouteTag tag)
    {
        bool enable = true;
        if (IsUserDashboard(tag))
        {
            enable = EnableShowUserDashboard(userStore, setting.CurrentValue);
        }

        bool pass = true;
        await routerChangingHandlerManager.NotifyInvokeHandlers(tag, (_, newValue) =>
        {
            pass = pass && newValue;
            return pass;
        });

        return enable && pass;
    }

    private readonly AsyncHandlerManager<RouteMeta, bool> routerMetaFilterHandlerManager = new();

    public IDisposable RegisterRouterMetaFilterHandler(Func<RouteMeta, Task<bool>> handler)
    {
        return routerMetaFilterHandlerManager.RegisterHandler(handler);
    }

    private async Task<bool> OnRouteMetaFilterAsync(RouteMeta meta)
    {
        if (IsUserDashboard(meta))
        {
            return EnableShowUserDashboard(userStore, setting.CurrentValue);
        }

        var used = meta.RouteType == null
                   || AppConst.AllAssemblies.IndexOf(meta.RouteType.Assembly) > -1
                   || meta.RouteType.Assembly == Assembly.GetEntryAssembly()
                   || (meta.RouteType.Assembly.GetName().Name?.EndsWith(".Client") ?? false);

        bool pass = true;
        await routerMetaFilterHandlerManager.NotifyInvokeHandlers(meta, (_, newValue) =>
        {
            pass = pass && newValue;
            return pass;
        });

        return used && pass;
    }
}

[AutoInject(ServiceType = typeof(IRouterStore))]
public partial class RouterStore : StoreBase, IRouterStore
{
    private readonly Dictionary<string, RouteTag> pages = new(StringComparer.OrdinalIgnoreCase);

    //private FrozenDictionary<string, RouteMenu>? frozenMenus;
    private readonly List<RouteMenu> menus = [];

    private RouteTag? current;

    public List<RouteTag> TopLinks => [.. pages.Values];

    public IEnumerable<RouteMenu> Menus => menus; // frozenMenus?.Values ?? [];

    public RouteTag? Current => current ?? pages.GetValueOrDefault("/");

    public WeakReference<object?> CurrentPageInstance { get; set; } = new WeakReference<object?>(null);
    public bool LastRouterChangingCheck => lastRouterChangingCheck;

    protected override void Release()
    {
        pages.Clear();
        menus.Clear();
        try
        {
            navigationManager.LocationChanged -= NavigationManager_LocationChanged;
            locationChangingHandler?.Dispose();
        }
        finally
        {
        }
    }

    public string CurrentUrl => '/' + navigationManager.ToBaseRelativePath(navigationManager.Uri);

    private RouteTag? preview;
    private readonly IProjectSettingService settingService;
    private readonly NavigationManager navigationManager;
    private readonly IUserStore userStore;
    private readonly IStringLocalizer<RouterStore> localizer;
    private readonly IOptionsMonitor<CultureOptions> options;
    private readonly ILogger<RouterStore> logger;
    private readonly IOptionsMonitor<AppSetting> setting;
    private readonly PagesService pagesService;
    private IDisposable? locationChangingHandler;
    public RouterStore(IProjectSettingService settingService
        , NavigationManager navigationManager
        , IUserStore userStore
        , IStringLocalizer<RouterStore> localizer
        , IOptionsMonitor<CultureOptions> options
        , ILogger<RouterStore> logger
        , IOptionsMonitor<AppSetting> setting
        , PagesService pagesService)
    {
        this.settingService = settingService;
        this.navigationManager = navigationManager;
        this.userStore = userStore;
        this.localizer = localizer;
        this.options = options;
        this.logger = logger;
        this.setting = setting;
        this.pagesService = pagesService;
    }

    private bool lastRouterChangingCheck = true;

    private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        try
        {
            preview = current;
        }
        finally
        {
            NotifyChanged();
        }
    }
    public void AttchNavigateEvent()
    {
        locationChangingHandler = navigationManager.RegisterLocationChangingHandler(LocationChangingHandlerAsync);
        navigationManager.LocationChanged += NavigationManager_LocationChanged;
    }
    public async ValueTask LocationChangingHandlerAsync(LocationChangingContext ctx)
    {
        var url = ctx.TargetLocation;
        url = string.IsNullOrEmpty(url) ? "/" : ParsedUriPathAndQuery(url);

        if (!pages.TryGetValue(url, out var tag))
        {
            // TODO 可能有BUG，先观察观察
            if (menus.Count == 0) return;
            var menu = menus.FirstOrDefault(r => CompareUrl(r.RouteUrl, url));
            if (menu == default)
            {
                var meta = pagesService.Pages.FirstOrDefault(r => CompareUrl(r.RouteUrl, url));
                if (meta == default)
                {
                    meta = new()
                    {
                        RouteId = url,
                        RouteTitle = url,
                        RouteUrl = url,
                    };
                }
                menu = new RouteMenu(meta);
            }
            tag = new RouteTag(menu)
            {
                RouteId = menu.RouteId ?? url,
                RouteUrl = AttachFirstSlash(menu.RouteUrl ?? url),
                RouteTitle = menu.RouteTitle,
                Icon = menu.Icon ?? "",
                Pin = menu.Pin,
            };
            pages[url] = tag;
        }

        current = tag;
        lastRouterChangingCheck = await OnRouterChangingAsync(tag);
        if (lastRouterChangingCheck && preview is not null)
        {
            preview?.TrySetDisactive(CurrentPageInstance);
        }

        return;

        static string ParsedUriPathAndQuery(string url)
        {
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var parsedUri)) throw new Exception();
            // 如果是绝对路径，返回 PathAndQuery（如 "/permission?x=1"）
            var parsed = parsedUri.IsAbsoluteUri
                ? parsedUri.PathAndQuery
                :
                // 已经是相对路径，直接返回
                parsedUri.OriginalString;
            if (!parsed.StartsWith('/'))
            {
                parsed = '/' + parsed;
            }
            return Uri.UnescapeDataString(parsed);
        }

        static string? AttachFirstSlash(string? url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            if (url.StartsWith('/')) return url;
            return "/" + url;
        }
    }

    public void CollectPageAdditionalInfo(object obj)
    {
        CurrentPageInstance.SetTarget(obj);
        if (Current is null) return;
        if (obj is IRoutePage page)
        {
            var title = page.GetTitle();
            if (!string.IsNullOrEmpty(title))
            {
                Current.RouteTitle ??= title;
                Current.Title = title.AsContent();
            }
        }
        else if (Current.RouteTitle is null)
        {
            var tta = obj.GetType().GetCustomAttribute<TagTitleAttribute>();
            if (tta != null)
            {
                Current.Title = tta.Title.AsContent();
            }
            else
            {
                Current.Title = CurrentUrl.AsContent();
            }
        }
        // Current.Rendered = true;
        NotifyChanged();
    }

    public void Remove(string link)
    {
        // if (pages.TryGetValue(link, out var p))
        // {
        //     p.Drop();
        // }

        pages.Remove(link);
    }

    public string GetLocalizerString<T>(T meta)
        where T : IRouteInfo
    {
        if (!options.CurrentValue.Enabled)
            return meta.RouteTitle;
        var l = localizer[meta.RouteId];
        return !string.Equals(l, meta.RouteId) ? l :
            //return localizer[meta.RouteId];
            meta.RouteTitle;
    }

    public Task RemoveOther(string link)
    {
        var removeKeys = pages.Keys.Where(k => k != link);
        foreach (var key in removeKeys)
        {
            if (pages[key].Pin) continue;
            // if (pages.TryGetValue(key, out var p))
            // {
            //     p.Drop();
            // }

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

        Current?.Drop();
        GoTo(CurrentUrl);
        return Task.CompletedTask;
    }

    public void GoTo(string uri)
    {
        navigationManager.NavigateTo(uri);
    }

    public Task Reset()
    {
        pages.TryGetValue("/", out var homeTag);
        ArgumentNullException.ThrowIfNull(homeTag);
        pages.Clear();
        pages.Add("/", homeTag);
        preview = homeTag;
        return Task.CompletedTask;
    }

    public async Task InitMenusAsync(UserInfo? userInfo)
    {
        try
        {
            menus.Clear();
            pages.Clear();
            var homeMenu = new RouteMenu()
            {
                RouteId = "Home",
                RouteUrl = "/",
                Icon = "svg-home",
                Group = "ROOT",
                RouteTitle = "主页",
            };
            var homeTag = new RouteTag(homeMenu)
            {
                RouteUrl = "/",
                RouteId = "Home",
                RouteTitle = "主页",
                Icon = "svg-home",
                Pin = true,
                IsActive = true
            };
            pages.Add("/", homeTag);
            menus.Add(homeMenu);

            IPermission[] savedInfos = [];
            if (userInfo is not null)
            {
                savedInfos = [.. await settingService.GetUserPowersAsync(userInfo)];
            }

            foreach (var meta in pagesService.Pages.Where(m => m.HasPageInfo).OrderBy(m => m.Sort))
            {
                if (menus.Any(m => m.RouteId == meta.RouteId)) continue;
                var enable = await OnRouteMetaFilterAsync(meta);
                if (!enable)
                    continue;
                // 没登录
                if (userInfo is null)
                {
                    if (!meta.IsAllowAnonymous)
                    {
                        continue;
                    }
                }
                var savedMeta = savedInfos.FirstOrDefault(p => p.PermissionId == meta.RouteId);
                if (savedMeta != null)
                {
                    meta.Icon = savedMeta.Icon;
                    meta.RouteTitle = savedMeta.PermissionName;
                    meta.Sort = savedMeta.Sort;
                }
                menus.Add(new RouteMenu(meta));
            }

            this.menus.Sort((a, b) => a.Sort - b.Sort);
            NotifyChanged();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "InitMenusAsync异常: {Message}", ex.Message);
        }
    }

    private static bool IsUserDashboard<T>(T meta)
        where T : IRouteInfo
    {
        return meta.RouteUrl == "/userdashboard";
    }

    private static bool EnableShowUserDashboard(IUserStore _, AppSetting setting) => setting.ClientHubOptions.Enable;

    [Obsolete("没什么用")]
    public Type? GetRouteType(string routeUrl)
    {
        return pagesService.Pages.FirstOrDefault(meta => meta.RouteUrl == routeUrl)?.RouteType;
    }
}
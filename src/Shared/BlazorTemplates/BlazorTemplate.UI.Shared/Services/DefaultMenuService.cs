using BlazorTemplate.Constraints.Store.Models;
using BlazorTemplate.UI.Shared.Routers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorTemplate.UI.Shared.Services;

[AutoInject]
internal class DefaultMenuService(IProjectSettingService settingService
    , PagesService pagesService) : List<RouteMenu>, IMenuService
{
    private static readonly RouteMenu defaultHome = new()
    {
        RouteId = "Home",
        RouteUrl = "/",
        Icon = "svg-home",
        Group = "ROOT",
        RouteTitle = "主页",
    };
    public RouteMenu Home => defaultHome;

    public List<RouteMenu> AllMenus => this;

    public IEnumerable<RouteMenu> RootMenus
    {
        get
        {
            foreach (var menu in AllMenus)
            {
                if (menu.Group == "ROOT")
                {
                    yield return menu;
                }
            }
        }
    }

    public IEnumerable<RouteMenu> GetChildMenus(string parentKey)
    {
        foreach (var menu in AllMenus)
        {
            if (menu.Group == parentKey)
            {
                yield return menu;
            }
        }
    }

    public async Task InitMenusAsync(UserInfo? userInfo, Func<RouteMeta, Task<bool>> predicate)
    {
        IPermission[] savedInfos = [];
        if (userInfo is not null)
        {
            savedInfos = [.. await settingService.GetUserPowersAsync(userInfo)];
        }

        foreach (var meta in pagesService.Pages.Where(m => m.HasPageInfo).OrderBy(m => m.Sort))
        {
            if (this.Any(m => m.RouteId == meta.RouteId)) continue;
            var enable = await predicate(meta);
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
            Add(new RouteMenu(meta));
        }
        Sort((a, b) => a.Sort - b.Sort);
    }
}

using Microsoft.AspNetCore.Components;
using Project.Constraints.Store.Models;

namespace Project.Constraints.Store;

public interface IStore
{
    event Action DataChangedEvent;
}

public interface IRouterStore : IStore, IDisposable
{
    IDisposable RegisterRouterChangingHandler(Func<RouteMeta, Task<bool>> handler);
    IDisposable RegisterRouterMetaFilterHandler(Func<RouteMeta, Task<bool>> handler);

    ICollection<RouteTag> TopLinks { get; }
    ICollection<RouteMenu> Menus { get; }
    RouteTag? Current { get; }
    RenderFragment? Content { get; }
    WeakReference<object?> CurrentPageInstance { get; }
    bool LastRouterChangingCheck { get; }
    bool RouteChanging { get; }
    string CurrentUrl { get; }
    void GoTo(string uri);
    //ValueTask LocationChangingHandlerAsync(string url);
    void TryRenderRouteData(RouteData? routeData);
    void CollectPageAdditionalInfo(object pageInstance);
    void Remove(string link);
    Task RemoveOther(string link);
    Task Reset();
    void AttchNavigateEvent();
    void Update();
    void NavigateToPreiousPage();
    void NavigateToNextPage();
    //Task Reload();
    /// <summary>
    /// 初始化菜单
    /// </summary>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    Task InitMenusAsync(UserInfo? userInfo);
    [Obsolete("没什么用")]
    Type? GetRouteType(string routeUrl);
    string GetLocalizerString<T>(T meta)
        where T : IRouteInfo;
}
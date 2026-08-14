using BlazorTemplate.ClientCore.Store.Models;

namespace BlazorTemplate.ClientCore.Services;

public interface IMenuService
{
    RouteMenu Home { get; }
    int Count { get; }
    List<RouteMenu> AllMenus { get; }
    IEnumerable<RouteMenu> RootMenus { get; }
    IEnumerable<RouteMenu> GetChildMenus(string parentKey);
    Task InitMenusAsync(UserInfo? userInfo, Func<RouteMeta, Task<bool>> predicate);
    void Clear();
}

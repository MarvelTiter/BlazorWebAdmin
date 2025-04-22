using Project.Constraints.Store.Models;

namespace Project.Constraints.Store
{
    public interface IStore
    {
        event Action DataChangedEvent;
    }

    public interface IRouterStore : IStore, IDisposable
    {
        event Func<TagRoute, Task<bool>>? RouterChangingEvent;
        event Func<RouterMeta, Task<bool>>? RouteMetaFilterEvent;
        List<TagRoute> TopLinks { get; }
        IEnumerable<RouteMenu> Menus { get; }
        TagRoute? Current { get; }
        string CurrentUrl { get; }
        void GoTo(string uri);
        Task RouteDataChangedHandleAsync(Microsoft.AspNetCore.Components.RouteData routeData);
        void Remove(string link);
        Task RemoveOther(string link);
        Task Reset();
        //Task Reload();
        /// <summary>
        /// 初始化菜单
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        Task InitMenusAsync(UserInfo? userInfo);
        Type? GetRouteType(string routeUrl);
        string GetLocalizerString(RouterMeta meta);
    }
}

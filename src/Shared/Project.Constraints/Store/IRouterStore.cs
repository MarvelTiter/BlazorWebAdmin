using Microsoft.AspNetCore.Components;
using Project.Constraints.Store.Models;

namespace Project.Constraints.Store
{
    public interface IStore
    {
        event Action DataChangedEvent;
    }

    public interface IRouterStore : IStore, IDisposable
    {
        IDisposable RegisterRouterChangingHandler(Func<TagRoute, Task<bool>> handler);
        IDisposable RegisterRouterMetaFilterHandler(Func<RouterMeta, Task<bool>> handler);

        List<TagRoute> TopLinks { get; }
        IEnumerable<RouteMenu> Menus { get; }
        TagRoute? Current { get; }
        WeakReference<object?> CurrentPageInstance { get; }
        bool LastRouterChangingCheck { get; }
        string CurrentUrl { get; }
        void GoTo(string uri);
        ValueTask LocationChangingHandlerAsync(string url);
        void CollectPageAdditionalInfo(object pageInstance);
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
using Project.Constraints.Store.Models;

namespace Project.Constraints.Store
{
    public interface IStore
    {
        event Action DataChangedEvent;
    }

    [AutoInject]
    public interface IRouterStore : IStore, IDisposable
    {
        event Func<TagRoute, Task<bool>>? RouterChangingEvent;
        event Func<RouterMeta, Task<bool>>? RouteMetaFilterEvent;
        List<TagRoute> TopLinks { get; }
        List<RouteMenu> Menus { get; set; }
        TagRoute? Current { get; }
        string CurrentUrl { get; }
        Task RouteDataChangedHandleAsync(Microsoft.AspNetCore.Components.RouteData routeData);
        void Remove(string link);
        Task RemoveOther(string link);
        Task Reset();
        Task InitRoutersAsync(UserInfo? userInfo);
        Type? GetRouteType(string routeUrl); 
    }
}

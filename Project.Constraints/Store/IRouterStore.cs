using Project.Constraints.Store.Models;
using Project.Models.Permissions;

namespace Project.Constraints.Store
{
    public interface IStore
    {
        event Action DataChangedEvent;
    }
    public interface IRouterStore : IStore, IDisposable
    {
        List<TagRoute> TopLinks { get; }
        List<RouteMenu> Menus { get; set; }
        TagRoute? Current { get; }
        string CurrentUrl { get; }
        Task RouteDataChangedHandleAsync(Microsoft.AspNetCore.Components.RouteData routeData);
        void Remove(string link);
        Task RemoveOther(string link);
        Task Reset();
        Task InitRoutersAsync(UserInfo? userInfo);
    }
}

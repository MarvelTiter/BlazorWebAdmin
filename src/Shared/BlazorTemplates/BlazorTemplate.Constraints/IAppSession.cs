using Microsoft.AspNetCore.Components;
using BlazorTemplate.Constraints.Store;
using BlazorTemplate.Constraints.UI;

namespace BlazorTemplate.Constraints;

//[AutoInject]
public interface IAppSession
{
    // event Func<Task>? WebApplicationAccessedEvent;
    // event Func<UserInfo, Task>? LoginSuccessEvent;
    // event Func<Task>? OnLoadedAsync;
    IDisposable RegisterWebApplicationAccessedHandler(Func<Task> handler);
    IDisposable RegisterLoginSuccessHandler(Func<UserInfo,Task> handler);
    IDisposable RegisterLoadedHandler(Func<Task> handler);
    NavigationManager Navigator { get; }
    public bool Loaded { get; set; }
    IAppStore AppStore { get; }
    //IAuthenticationStateProvider AuthenticationStateProvider { get; }
    IRouterStore RouterStore { get; }
    IUserStore UserStore { get; }
    IUIService UI { get; }
    Action? Update { get; set; }
    Task NotifyWebApplicationAccessedAsync();
    Task NotifyLoginSuccessAsync();
}

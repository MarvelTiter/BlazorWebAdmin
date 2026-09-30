using BlazorTemplate.ClientCore.UI;

namespace BlazorTemplate.ClientCore;

//[AutoInject]
public interface IAppSession
{
    // event Func<Task>? WebApplicationAccessedEvent;
    // event Func<UserInfo, Task>? LoginSuccessEvent;
    // event Func<Task>? OnLoadedAsync;
    event Action? OnUpdate;

    IDisposable RegisterWebApplicationAccessedHandler(Func<Task> handler);
    IDisposable RegisterLoginSuccessHandler(Func<UserInfo, Task> handler);
    IDisposable RegisterLoadedHandler(Func<Task> handler);
    NavigationManager Navigator { get; }
    public bool Loaded { get; set; }
    IAppStore AppStore { get; }
    //IAuthenticationStateProvider AuthenticationStateProvider { get; }
    IRouterStore RouterStore { get; }
    IUserStore UserStore { get; }
    IUIService UI { get; }
    void NotifyUpdate();
    Task NotifyWebApplicationAccessedAsync();
    Task NotifyLoginSuccessAsync();
}

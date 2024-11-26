using Microsoft.AspNetCore.Components;
using Project.Constraints.Store;
using Project.Constraints.UI;

namespace Project.Constraints;

//[AutoInject]
public interface IAppSession
{
    event Func<Task>? WebApplicationAccessedEvent;
    event Func<UserInfo, Task>? LoginSuccessEvent;
    event Func<Task>? OnLoadedAsync;
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

using Microsoft.AspNetCore.Components;
using Project.Constraints.Store;
using Project.Constraints.UI;

namespace Project.Constraints;

[AutoInject]
public interface IAppSession
{
    NavigationManager Navigator { get; set; }
    IAppStore AppStore {  get; set; }
    IAuthenticationStateProvider AuthenticationStateProvider { get; set; }
    IRouterStore RouterStore { get; set; }
    IUserStore UserStore { get; set; }
    IUIService UI { get; set; }
    Action? Update { get; set; }
}

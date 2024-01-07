using Microsoft.AspNetCore.Components;
using Project.Constraints.Store;
using Project.Constraints.UI;
using Project.Constraints;

namespace Project.AppCore.Store
{
    public class AppSession(NavigationManager navigationManager
        , IAppStore appStore
        , IAuthenticationStateProvider authenticationStateProvider
        , IRouterStore routerStore
        , IUserStore userStore
        , IUIService ui) : IAppSession
    {
        public NavigationManager Navigator { get; set; } = navigationManager;
        public IAppStore AppStore { get; set; } = appStore;
        public IAuthenticationStateProvider AuthenticationStateProvider { get; set; } = authenticationStateProvider;
        public IRouterStore RouterStore { get; set; } = routerStore;
        public IUserStore UserStore { get; set; } = userStore;
        public IUIService UI { get; set; } = ui;
        public Action? Update { get; set; }
    }
}

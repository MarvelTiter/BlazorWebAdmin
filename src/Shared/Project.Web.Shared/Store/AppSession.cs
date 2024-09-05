using Project.Constraints.UI;

namespace Project.Web.Shared.Store
{
    [AutoInject]
    public class AppSession(NavigationManager navigationManager
        , IAppStore appStore
        , IRouterStore routerStore
        , IUserStore userStore
        , IUIService ui) : IAppSession
    {
        //private readonly IServiceProvider provider = provider;
        public NavigationManager Navigator { get; } = navigationManager;
        public IAppStore AppStore { get; } = appStore;
        //public IAuthenticationStateProvider AuthenticationStateProvider { get; } = 
        public IRouterStore RouterStore { get; } = routerStore;
        public IUserStore UserStore { get; } = userStore;
        public IUIService UI { get; } = ui;
        public Action? Update { get; set; }

        public event Func<Task>? WebApplicationAccessedEvent;
        public Task NotifyWebApplicationAccessedAsync()
        {
            if (WebApplicationAccessedEvent != null)
            {
                return WebApplicationAccessedEvent();
            }
            return Task.CompletedTask;
        }

        public event Func<UserInfo, Task>? LoginSuccessEvent;

        public Task NotifyLoginSuccessAsync()
        {
            if (LoginSuccessEvent != null)
            {
                return LoginSuccessEvent(UserStore.UserInfo!);
            }
            return Task.CompletedTask;
        }
    }
}

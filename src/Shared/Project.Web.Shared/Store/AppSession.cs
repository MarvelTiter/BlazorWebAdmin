using Project.Constraints.UI;
using Project.Constraints.Utils;

namespace Project.Web.Shared.Store
{
    [AutoInject]
    public class AppSession(NavigationManager navigationManager
        , IAppStore appStore
        , IRouterStore routerStore
        , IUserStore userStore
        , IUIService ui) : IAppSession
    {
        private bool webAccessedDone;
        private bool loginEventDone;
        //private readonly IServiceProvider provider = provider;
        public NavigationManager Navigator { get; } = navigationManager;
        public IAppStore AppStore { get; } = appStore;
        //public IAuthenticationStateProvider AuthenticationStateProvider { get; } = 
        public IRouterStore RouterStore { get; } = routerStore;
        public IUserStore UserStore { get; } = userStore;
        public IUIService UI { get; } = ui;
        public Action? Update { get; set; }
        public bool Loaded { get; set; }

        public event Func<Task>? OnLoadedAsync;

        public event Func<Task>? WebApplicationAccessedEvent;
        public async Task NotifyWebApplicationAccessedAsync()
        {
            await WebApplicationAccessedEvent.InvokeAsync();
            webAccessedDone = true;
            await InvokeInit();
        }

        public event Func<UserInfo, Task>? LoginSuccessEvent;

        public async Task NotifyLoginSuccessAsync()
        {
            await LoginSuccessEvent.InvokeAsync(UserStore.UserInfo!);
            loginEventDone = true;
            await InvokeInit();
        }

        private Task InvokeInit()
        {
            if (loginEventDone && webAccessedDone)
            {
                return OnLoadedAsync?.Invoke() ?? Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
    }
}

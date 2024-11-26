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
            if (WebApplicationAccessedEvent != null)
            {
                foreach (Func<Task> item in WebApplicationAccessedEvent.GetInvocationList().Cast<Func<Task>>())
                {
                    await item.Invoke();
                }
                //await WebApplicationAccessedEvent.Invoke();
            }
            webAccessedDone = true;
            await InvokeInit();
        }

        public event Func<UserInfo, Task>? LoginSuccessEvent;

        public async Task NotifyLoginSuccessAsync()
        {
            if (LoginSuccessEvent != null)
            {
                // TODO 等待异步事件
                foreach (Func<UserInfo, Task> item in LoginSuccessEvent.GetInvocationList().Cast<Func<UserInfo, Task>>())
                {
                    await item.Invoke(UserStore.UserInfo!);
                }
                //await LoginSuccessEvent.Invoke(UserStore.UserInfo!);
            }
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

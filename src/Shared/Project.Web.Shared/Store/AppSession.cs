using Project.Constraints.UI;
using Project.Constraints.Utils;

namespace Project.Web.Shared.Store;

[AutoInject]
public class AppSession(NavigationManager navigationManager, IAppStore appStore, IRouterStore routerStore, IUserStore userStore, IUIService ui) : IAppSession
{
    public NavigationManager Navigator { get; } = navigationManager;
    public IAppStore AppStore { get; } = appStore;
    public IRouterStore RouterStore { get; } = routerStore;
    public IUserStore UserStore { get; } = userStore;
    public IUIService UI { get; } = ui;
    public Action? Update { get; set; }
    public bool Loaded { get; set; }

    private readonly AsyncHandlerManager webappAccessed = new();

    public IDisposable RegisterWebApplicationAccessedHandler(Func<Task> handler)
    {
        return webappAccessed.RegisterHandler(handler);
    }

    private readonly AsyncHandlerManager<UserInfo> loginSuccess = new();

    public IDisposable RegisterLoginSuccessHandler(Func<UserInfo, Task> handler)
    {
        return loginSuccess.RegisterHandler(handler);
    }

    private readonly AsyncHandlerManager loaded = new();

    public IDisposable RegisterLoadedHandler(Func<Task> handler)
    {
        return loaded.RegisterHandler(handler);
    }
    private Task? loginSuccessTask;
    public async Task NotifyWebApplicationAccessedAsync()
    {
        if (loginSuccessTask is not null && !loginSuccessTask.IsCompleted)
        {
            await loginSuccessTask;
        }
        await webappAccessed.NotifyInvokeHandlers();
        await loaded.NotifyInvokeHandlers();
    }

    public Task NotifyLoginSuccessAsync()
    {
        if (UserStore.UserInfo is null)
            return Task.CompletedTask;
        loginSuccessTask = loginSuccess.NotifyInvokeHandlers(UserStore.UserInfo);
        return loginSuccessTask;
    }
}
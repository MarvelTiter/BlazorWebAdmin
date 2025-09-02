using Microsoft.Extensions.DependencyInjection;
using Project.Constraints.UI;
using Project.Constraints.Utils;

namespace Project.Web.Shared.Store;

[AutoInject]
public class AppSession(IServiceProvider serviceProvider) : IAppSession
{
    private readonly Lazy<NavigationManager> lazyNavigationManager = new(serviceProvider.GetRequiredService<NavigationManager>);
    private readonly Lazy<IAppStore> lazyAppStore = new(serviceProvider.GetRequiredService<IAppStore>);
    private readonly Lazy<IRouterStore> lazyRouterStore = new(serviceProvider.GetRequiredService<IRouterStore>);
    private readonly Lazy<IUserStore> lazyUserStore = new(serviceProvider.GetRequiredService<IUserStore>);
    private readonly Lazy<IUIService> lazyUI = new(serviceProvider.GetRequiredService<IUIService>);
    public NavigationManager Navigator => lazyNavigationManager.Value;
    public IAppStore AppStore => lazyAppStore.Value;
    public IRouterStore RouterStore => lazyRouterStore.Value;
    public IUserStore UserStore => lazyUserStore.Value;
    public IUIService UI => lazyUI.Value;
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
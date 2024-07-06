using Microsoft.AspNetCore.Components;
using Project.Constraints.Services;
using Project.Constraints.Store;
using Project.Constraints.UI;
using System.Diagnostics.CodeAnalysis;

namespace Project.Constraints.Page;

public class BasicComponent : ComponentBase, IAsyncDisposable
{
    private bool disposedValue;

    [CascadingParameter, NotNull] public IAppSession? Context { get; set; }
    //[Inject] public IProjectSettingService AppSettingService { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalParameters { get; set; } = [];
    public IUIService UI => Context.UI;
    public IAppStore App => Context.AppStore;
    public IRouterStore Router => Context.RouterStore;
    public IUserStore User => Context.UserStore;
    public NavigationManager Navigator => Context.Navigator;
    [Inject, NotNull] public IAuthenticationStateProvider? AuthenticationStateProvider { get; set; }

    protected virtual ValueTask OnDisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                try
                {
                    await OnDisposeAsync();
                }
                catch
                {
                }
            }
            disposedValue = true;
        }
    }

    public ValueTask DisposeAsync()
    {
        return DisposeAsync(disposing: true);
    }
}

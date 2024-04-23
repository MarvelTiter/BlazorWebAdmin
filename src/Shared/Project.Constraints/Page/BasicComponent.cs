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
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalParameters { get; set; }
    public IUIService UI => Context?.UI;
    public IAppStore App => Context?.AppStore;
    public IRouterStore Router => Context?.RouterStore;
    public IUserStore User => Context?.UserStore;
    public NavigationManager Navigator => Context?.Navigator;
    [Inject, NotNull] public IAuthenticationStateProvider? AuthenticationStateProvider { get; set; }

    protected virtual void OnDispose()
    {

    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                OnDispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}

using Microsoft.AspNetCore.Components;
using Project.Constraints.Store;
using Project.Constraints.UI;

namespace Project.Constraints.Page;

public class BasicComponent : ComponentBase, IDisposable
{
    private bool disposedValue;

    [Inject] public IAppSession Context { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalParameters { get; set; }
    public IUIService UI => Context?.UI;
    public IAppStore App => Context?.AppStore;
    public IRouterStore Router => Context?.RouterStore;
    public IUserStore User => Context?.UserStore;
    public IAuthenticationStateProvider AuthenticationStateProvider => Context?.AuthenticationStateProvider;
    public NavigationManager Navigator => Context?.Navigator;

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
}

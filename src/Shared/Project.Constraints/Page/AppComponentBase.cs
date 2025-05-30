using Microsoft.AspNetCore.Components;
using Project.Constraints.Services;
using Project.Constraints.Store;
using Project.Constraints.UI;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Routing;

namespace Project.Constraints.Page;

public class AppComponentBase : ComponentBase, IAsyncDisposable
{
    private bool disposedValue;
    private IDisposable? _disposable;

    [CascadingParameter, NotNull] public IAppSession? Context { get; set; }

    //[Inject] public IProjectSettingService AppSettingService { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalParameters { get; set; } = [];
    
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            _disposable = Navigator.RegisterLocationChangingHandler(LeavingAsync);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await OnFirstRenderAsync();
        }
    }

    public IUIService UI => Context.UI;
    public IAppStore App => Context.AppStore;
    public IRouterStore Router => Context.RouterStore;
    public IUserStore User => Context.UserStore;
    public NavigationManager Navigator => Context.Navigator;
    [Inject, NotNull] public IAuthenticationStateProvider? AuthenticationStateProvider { get; set; }

    private async ValueTask LeavingAsync(LocationChangingContext context)
    {
        var pass = await LeavingAsync();
        if (!pass)
        {
            context.PreventNavigation();
        }
    }
    
    protected virtual Task OnFirstRenderAsync() => Task.CompletedTask;

    protected virtual ValueTask<bool> LeavingAsync()
    {
        return ValueTask.FromResult(true);
    }

    protected virtual ValueTask OnDisposeAsync()
    {
        _disposable?.Dispose();
        return ValueTask.CompletedTask;
    }

    private async ValueTask DisposeAsync(bool disposing)
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

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(disposing: true);
        GC.SuppressFinalize(this);
    }
}
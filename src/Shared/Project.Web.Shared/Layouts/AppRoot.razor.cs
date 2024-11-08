using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Project.Web.Shared.Store;

namespace Project.Web.Shared.Layouts;
public partial class AppRoot : IAsyncDisposable
{
    private readonly List<IAddtionalInterceptor> initActions = [];
    private bool loading = true;
    [Inject] [NotNull] private IAppSession? Context { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter, NotNull] public Type? DefaultLayout { get; set; }
    [Inject] [NotNull] private IServiceProvider? Services { get; set; }
    [Inject] [NotNull] private IProjectSettingService? SettingService { get; set; }
    [Inject] [NotNull] private IProtectedLocalStorage? LocalStorage { get; set; }
    public ValueTask DisposeAsync()
    {
        Context.RouterStore.RouterChangingEvent -= SettingService.RouterChangingAsync;
        Context.RouterStore.RouteMetaFilterEvent -= SettingService.RouteMetaFilterAsync;
        Context.LoginSuccessEvent -= SettingService.LoginSuccessAsync;
        Context.WebApplicationAccessedEvent -= SettingService.AfterWebApplicationAccessed;

        foreach (var additional in initActions)
        {
            Context.LoginSuccessEvent -= additional.LoginSuccessAsync;
            Context.RouterStore.RouteMetaFilterEvent -= additional.RouteMetaFilterAsync;
            Context.RouterStore.RouterChangingEvent -= additional.RouterChangingAsync;
            Context.WebApplicationAccessedEvent -= additional.AfterWebApplicationAccessedAsync;
        }

        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    protected override void OnInitialized()
    {
        Context.OnLoadedAsync += Context_OnLoadedAsync;
        Context.RouterStore.RouterChangingEvent += SettingService.RouterChangingAsync;
        Context.RouterStore.RouteMetaFilterEvent += SettingService.RouteMetaFilterAsync;
        Context.LoginSuccessEvent += SettingService.LoginSuccessAsync;
        Context.WebApplicationAccessedEvent += SettingService.AfterWebApplicationAccessed;

        var interceptors = Services.GetServices<IAddtionalInterceptor>();

        foreach (var additional in interceptors)
        {
            initActions.Add(additional);
            Context.RouterStore.RouterChangingEvent += additional.RouterChangingAsync;
            Context.RouterStore.RouteMetaFilterEvent += additional.RouteMetaFilterAsync;
            Context.LoginSuccessEvent += additional.LoginSuccessAsync;
            Context.WebApplicationAccessedEvent += additional.AfterWebApplicationAccessedAsync;
        }
        base.OnInitialized();
    }

    private Task Context_OnLoadedAsync()
    {
        Context.OnLoadedAsync -= Context_OnLoadedAsync;
        loading = false;
        return InvokeAsync(StateHasChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await Context.NotifyWebApplicationAccessedAsync();
            var app = await LocalStorage.GetAsync<AppStore>(ConstraintString.APP_STORE_KEY);
            if (app.Success) Context.AppStore.ApplySetting(app.Value);
        }
    }
}
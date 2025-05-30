using System.Reflection;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Store;
using Project.Web.Shared.Utils;

namespace Project.Web.Shared.Layouts;

public partial class AppRoot : IAppDomEventHandler, IThemeChangedBroadcast, IAsyncDisposable
{
    public event Func<MouseEventArgs, Task>? BodyClickEvent;
    public event Func<KeyboardEventArgs, Task>? OnKeyDown;
    public event Func<KeyboardEventArgs, Task>? OnKeyUp;
    public event Func<Task>? OnThemeChanged;
    private readonly List<IAddtionalInterceptor> initActions = [];
    [Inject] [NotNull] private IAppSession? Context { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter, NotNull] public Type? DefaultLayout { get; set; }
    [Inject] [NotNull] private IServiceProvider? Services { get; set; }
    [Inject] [NotNull] private IJSRuntime? Js { get; set; }
    [Inject] [NotNull] private IProjectSettingService? SettingService { get; set; }
    [Inject] [NotNull] private IProtectedLocalStorage? LocalStorage { get; set; }
    [Inject,NotNull] private ILogger<AppRoot>? Logger { get; set; }
    
    // private RouteData? RouteData { get; set; }

    public Task NotifyThemeChangedAsync()
    {
        return OnThemeChanged?.Invoke() ?? Task.CompletedTask;
    }

    protected Task HandleRootClick(MouseEventArgs e)
    {
        return BodyClickEvent?.Invoke(e) ?? Task.CompletedTask;
    }

    protected Task HandleKeyDownAction(KeyboardEventArgs e)
    {
        return OnKeyDown?.Invoke(e) ?? Task.CompletedTask;
    }

    protected Task HandleKeyUpAction(KeyboardEventArgs e)
    {
        return OnKeyUp?.Invoke(e) ?? Task.CompletedTask;
    }

    private readonly List<IDisposable> registerHandlers = [];
    private IDisposable? loadedHandler;
    private IDisposable? locationChangingHandler;

    protected override void OnInitialized()
    {
        // Context.OnLoadedAsync += Context_OnLoadedAsync;
        // Context.RouterStore.RouterChangingEvent += SettingService.RouterChangingAsync;
        // Context.RouterStore.RouteMetaFilterEvent += SettingService.RouteMetaFilterAsync;
        // Context.LoginSuccessEvent += SettingService.LoginSuccessAsync;
        // Context.WebApplicationAccessedEvent += SettingService.AfterWebApplicationAccessed;
        Context.RouterStore.DataChangedEvent += StateHasChanged;
        loadedHandler = Context.RegisterLoadedHandler(Context_OnLoadedAsync);
        registerHandlers.Add(Context.RouterStore.RegisterRouterChangingHandler(SettingService.RouterChangingAsync));
        registerHandlers.Add(Context.RouterStore.RegisterRouterMetaFilterHandler(SettingService.RouteMetaFilterAsync));
        registerHandlers.Add(Context.RegisterLoginSuccessHandler(SettingService.LoginSuccessAsync));
        registerHandlers.Add(Context.RegisterWebApplicationAccessedHandler(SettingService.AfterWebApplicationAccessed));
        locationChangingHandler = Context.Navigator.RegisterLocationChangingHandler(LocationChangingAsync);
        // Context.Navigator.LocationChanged += NavigatorOnLocationChanged;
        var interceptors = Services.GetServices<IAddtionalInterceptor>();

        foreach (var additional in interceptors)
        {
            initActions.Add(additional);
            // Context.RouterStore.RouterChangingEvent += additional.RouterChangingAsync;
            // Context.RouterStore.RouteMetaFilterEvent += additional.RouteMetaFilterAsync;
            // Context.LoginSuccessEvent += additional.LoginSuccessAsync;
            // Context.WebApplicationAccessedEvent += additional.AfterWebApplicationAccessedAsync;
            registerHandlers.Add(Context.RouterStore.RegisterRouterChangingHandler(additional.RouterChangingAsync));
            registerHandlers.Add(Context.RouterStore.RegisterRouterMetaFilterHandler(additional.RouteMetaFilterAsync));
            registerHandlers.Add(Context.RegisterLoginSuccessHandler(additional.LoginSuccessAsync));
            registerHandlers.Add(Context.RegisterWebApplicationAccessedHandler(additional.AfterWebApplicationAccessedAsync));
        }

        base.OnInitialized();
    }

    // private void NavigatorOnLocationChanged(object? sender, LocationChangedEventArgs e)
    // {
    //     try
    //     {
    //         if (RouteData != null)
    //         {
    //             Logger.LogInformation(RouteData.PageType.FullName);
    //             Context.RouterStore.RouteDataChangedHandle(RouteData);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Logger.LogError(ex, "LocationChangedEventHandler");
    //     }
    // }

    private ValueTask LocationChangingAsync(LocationChangingContext context)
    {
        return Context.RouterStore.LocationChangingHandlerAsync(context.TargetLocation);
    }

    private Task Context_OnLoadedAsync()
    {
        // Context.OnLoadedAsync -= Context_OnLoadedAsync;
        loadedHandler?.Dispose();
        Context.Loaded = true;
        return InvokeAsync(StateHasChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await Context.NotifyWebApplicationAccessedAsync();
            var app = await LocalStorage.GetAsync<AppStore>(ConstraintString.APP_STORE_KEY);
            if (app.Success)
            {
                Context.AppStore.ApplySetting(app.Value);
                await Js.InvokeUtilsAsync("setTheme", $"{app.Value!.Theme}".ToLower(), Context.UI.DarkStyle());
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        // Context.RouterStore.RouterChangingEvent -= SettingService.RouterChangingAsync;
        // Context.RouterStore.RouteMetaFilterEvent -= SettingService.RouteMetaFilterAsync;
        // Context.LoginSuccessEvent -= SettingService.LoginSuccessAsync;
        // Context.WebApplicationAccessedEvent -= SettingService.AfterWebApplicationAccessed;

        // foreach (var additional in initActions)
        // {
        //     Context.LoginSuccessEvent -= additional.LoginSuccessAsync;
        //     Context.RouterStore.RouteMetaFilterEvent -= additional.RouteMetaFilterAsync;
        //     Context.RouterStore.RouterChangingEvent -= additional.RouterChangingAsync;
        //     Context.WebApplicationAccessedEvent -= additional.AfterWebApplicationAccessedAsync;
        // }
        locationChangingHandler?.Dispose();
        initActions.Clear();
        foreach (var handler in registerHandlers)
        {
            handler.Dispose();
        }
        Context.RouterStore.DataChangedEvent -= StateHasChanged;
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
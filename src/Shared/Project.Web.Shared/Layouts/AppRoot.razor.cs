using System.Reflection;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Project.Constraints.Options;
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
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter, NotNull] public Type? DefaultLayout { get; set; }
    [Inject, NotNull] private IAppSession? Context { get; set; }
    [Inject, NotNull] private IServiceProvider? Services { get; set; }
    [Inject, NotNull] private IJSRuntime? Js { get; set; }
    [Inject, NotNull] private IProjectSettingService? SettingService { get; set; }
    [Inject, NotNull] private IProtectedLocalStorage? LocalStorage { get; set; }
    [Inject, NotNull] IOptions<Token>? Token { get; set; }
    [Inject, NotNull] private ILogger<AppRoot>? Logger { get; set; }
    [Inject, NotNull] IAuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
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
    //private IDisposable? locationChangingHandler;

    protected override void OnInitialized()
    {
        Context.RouterStore.DataChangedEvent += StateHasChanged;
        loadedHandler = Context.RegisterLoadedHandler(OnLoadedAsync);
        registerHandlers.Add(Context.RouterStore.RegisterRouterChangingHandler(SettingService.RouterChangingAsync));
        registerHandlers.Add(Context.RouterStore.RegisterRouterMetaFilterHandler(SettingService.RouteMetaFilterAsync));
        registerHandlers.Add(Context.RegisterLoginSuccessHandler(SettingService.LoginSuccessAsync));
        registerHandlers.Add(Context.RegisterWebApplicationAccessedHandler(SettingService.AfterWebApplicationAccessed));
        //locationChangingHandler = Context.Navigator.RegisterLocationChangingHandler(LocationChangingAsync);
        Context.RouterStore.AttchNavigateEvent();
        var interceptors = Services.GetServices<IAddtionalInterceptor>();

        foreach (var additional in interceptors)
        {
            initActions.Add(additional);
            registerHandlers.Add(Context.RouterStore.RegisterRouterChangingHandler(additional.RouterChangingAsync));
            registerHandlers.Add(Context.RouterStore.RegisterRouterMetaFilterHandler(additional.RouteMetaFilterAsync));
            registerHandlers.Add(Context.RegisterLoginSuccessHandler(additional.LoginSuccessAsync));
            registerHandlers.Add(Context.RegisterWebApplicationAccessedHandler(additional.AfterWebApplicationAccessedAsync));
        }
        Context.Update = StateHasChanged;
        base.OnInitialized();
    }
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (AuthenticationStateTask != null)
        {
            var state = await AuthenticationStateTask;
            if (state.User.Identity!.IsAuthenticated)
            {
                await Context.NotifyLoginSuccessAsync();
            }
            await Context.RouterStore.InitMenusAsync(AuthenticationStateProvider.Current);
        }
    }
    private async Task NoneOperation()
    {
        if (Context.AppStore.Working || AuthenticationStateProvider.AuthService is null)
        {
            return;
        }

        // 退出
        await AuthenticationStateProvider.ClearState();

        await InvokeAsync(StateHasChanged);
    }

    //private ValueTask LocationChangingAsync(LocationChangingContext context)
    //{
    //    return Context.RouterStore.LocationChangingHandlerAsync(context.TargetLocation);
    //}

    private Task OnLoadedAsync()
    {
        loadedHandler?.Dispose();
        Context.Loaded = true;
        return InvokeAsync(StateHasChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var app = await LocalStorage.GetAsync<AppStore>(ConstraintString.APP_STORE_KEY);
            if (app.Success)
            {
                Context.AppStore.ApplySetting(app.Value);
                //await Js.InvokeUtilsAsync("setTheme", $"{app.Value!.Theme}".ToLower(), Context.UI.DarkStyle());
                await Context.UI.OnAppMounted(app.Value!);
            }
            await Context.NotifyWebApplicationAccessedAsync();
            var url = Context.Navigator.ToBaseRelativePath(Context.Navigator.Uri);
            if (!string.IsNullOrEmpty(url))
            {
                Context.Navigator.NavigateTo(url);
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        //locationChangingHandler?.Dispose();
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
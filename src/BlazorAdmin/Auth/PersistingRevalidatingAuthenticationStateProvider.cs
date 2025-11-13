using AutoInjectGenerator;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Project.AppCore;
using Project.Constraints;
using Project.Constraints.Models;
using Project.Constraints.Services;
using Project.Constraints.Store;
using Project.Constraints.Utils;

namespace BlazorAdmin.Auth;

// 使用AutoInject特性自动注入服务
[AutoInject(Group = "SERVER", ServiceType = typeof(IAuthenticationStateProvider))]
[AutoInject(Group = "SERVER", ServiceType = typeof(AuthenticationStateProvider))]
// 类声明，继承自RevalidatingServerAuthenticationStateProvider并实现IAuthenticationStateProvider接口
public sealed class PersistingRevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider,
    IAuthenticationStateProvider
{
    private readonly IUserStore userStore;
    private readonly NavigationManager navigation;
    private readonly IProjectSettingService settingService;
    private readonly PersistentComponentState state;
    private readonly PersistingComponentStateSubscription subscription;
    private Task<AuthenticationState>? authenticationStateTask;

    public PersistingRevalidatingAuthenticationStateProvider(ILoggerFactory loggerFactory
        , PersistentComponentState persistentComponentState
        , IUserStore userStore
        , NavigationManager navigation
        , IProjectSettingService settingService
        , IHttpContextAccessor httpContextAccessor
        , IServiceProvider provider) : base(loggerFactory)
    {
        state = persistentComponentState;
        this.userStore = userStore;
        this.navigation = navigation;
        this.settingService = settingService;
        AuthService = provider.GetService<IAuthService>();
        AuthenticationStateChanged += OnAuthenticationStateChanged;
        subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
        if (httpContextAccessor.HttpContext?.User.GetCookieClaimsIdentity(out var identity) == true && identity!.IsAuthenticated == true)
        {
            var u = identity.GetUserInfo();
            userStore.SetUser(u);
        }
    }
    public IAuthService? AuthService { get; }

    // 重写RevalidationInterval属性，设置验证间隔时间为5分钟
    protected override TimeSpan RevalidationInterval => settingService.RevalidationInterval;

    // 重写ValidateAuthenticationStateAsync方法
    protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState,
        CancellationToken cancellationToken)
    {
        if (AuthService is null)
            return true;
        var ok = await AuthService.CheckUserStatusAsync(Current);
        return ok;
    }

    // 提供当前用户信息的属性
    public UserInfo? Current => userStore.UserInfo;

    // 清除认证状态的方法
    public Task ClearState()
    {
        var redirect = navigation.ToAbsoluteUri(navigation.Uri);
        var encoded = Uri.EscapeDataString(redirect.PathAndQuery);
        navigation.NavigateTo($"/api/account/logout?Redirect={encoded}", true);
        return Task.CompletedTask;
    }


    // 处理认证状态变化的事件处理方法
    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        authenticationStateTask = task;
    }

    // 持久化认证状态的方法
    private async Task OnPersistingAsync()
    {
        if (authenticationStateTask is null)
            return;

        var authenticationState = await authenticationStateTask;
        var principal = authenticationState.User;

        if (principal.GetCookieClaimsIdentity(out var identity) && identity!.IsAuthenticated == true)
        {
            var u = identity.GetUserInfo();
            state.PersistAsJson(nameof(UserInfo), u);
        }
    }

    // 重写Dispose方法，释放资源
    protected override void Dispose(bool disposing)
    {
        subscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        base.Dispose(disposing);
    }
}
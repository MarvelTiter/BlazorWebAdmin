using System.Web;
using AutoInjectGenerator;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Project.Constraints;

namespace Project.AppCore.Auth;

// 使用AutoInject特性自动注入服务
[AutoInject(Group = "SERVER", ServiceType = typeof(IAuthenticationStateProvider))]
[AutoInject(Group = "SERVER", ServiceType = typeof(AuthenticationStateProvider))]
// 类声明，继承自RevalidatingServerAuthenticationStateProvider并实现IAuthenticationStateProvider接口
public sealed class PersistingRevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider,
    IAuthenticationStateProvider
{
    private readonly IAppSession app;
    private readonly PersistentComponentState state;
    private readonly PersistingComponentStateSubscription subscription;
    private Task<AuthenticationState>? authenticationStateTask;

    public PersistingRevalidatingAuthenticationStateProvider(ILoggerFactory loggerFactory
        , PersistentComponentState persistentComponentState
        , IAppSession app
        , IHttpContextAccessor httpContextAccessor) : base(loggerFactory)
    {
        state = persistentComponentState;
        this.app = app;
        AuthenticationStateChanged += OnAuthenticationStateChanged;
        subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
        if (httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var u = httpContextAccessor.HttpContext.User.GetUserInfo();
            app.UserStore.SetUser(u);
        }
    }

    // 重写RevalidationInterval属性，设置验证间隔时间为30分钟
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    // 提供当前用户信息的属性
    public UserInfo? Current => app.UserStore.UserInfo;

    // 清除认证状态的方法
    public Task ClearState()
    {
        var redirect = app.Navigator.ToBaseRelativePath(app.Navigator.Uri);
        app.Navigator.NavigateTo($"/api/account/logout?Redirect={HttpUtility.UrlEncode(redirect)}", true);
        return Task.CompletedTask;
    }

    // 重写ValidateAuthenticationStateAsync方法，始终返回true
    protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
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

        if (principal.Identity?.IsAuthenticated == true)
        {
            var u = principal.GetUserInfo();
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
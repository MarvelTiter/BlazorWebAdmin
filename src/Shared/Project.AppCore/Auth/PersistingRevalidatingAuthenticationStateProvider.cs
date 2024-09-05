using System.Web;
using AutoInjectGenerator;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Project.Constraints;

namespace Project.AppCore.Auth;

[AutoInject(Group = "SERVER", ServiceType = typeof(IAuthenticationStateProvider))]
[AutoInject(Group = "SERVER", ServiceType = typeof(AuthenticationStateProvider))]
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

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    public UserInfo? Current => app.UserStore.UserInfo;

    // public Task IdentifyUser(UserInfo info)
    // {
    //     throw new NotImplementedException();
    // }

    public Task ClearState()
    {
        //var t = Task.FromResult(new AuthenticationState(new(new ClaimsIdentity())));
        //SetAuthenticationState(t);

        //app.UserStore.ClearUser();
        //await httpContextAccessor.HttpContext.SignOutAsync();
        var redirect = app.Navigator.ToBaseRelativePath(app.Navigator.Uri);
        app.Navigator.NavigateTo($"/api/account/logout?Redirect={HttpUtility.UrlEncode(redirect)}", true);
        return Task.CompletedTask;
    }

    protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        if (authenticationStateTask is null)
            //throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
            return;

        var authenticationState = await authenticationStateTask;
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var u = principal.GetUserInfo();
            state.PersistAsJson(nameof(UserInfo), u);
        }
    }

    protected override void Dispose(bool disposing)
    {
        subscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        base.Dispose(disposing);
    }
}
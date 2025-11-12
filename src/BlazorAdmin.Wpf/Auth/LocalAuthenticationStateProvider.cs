using AutoInjectGenerator;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Project.Constraints;
using Project.Constraints.Models;
using Project.Constraints.Services;
using Project.Constraints.Store;
using Project.Constraints.Utils;
using System.Security.Claims;

namespace BlazorAdmin.Wpf.Auth;

[AutoInject(ServiceType = typeof(IAuthenticationStateProvider))]
[AutoInject(ServiceType = typeof(AuthenticationStateProvider))]
public sealed class LocalAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider, IAuthenticationStateProvider
{
    public const string USER_KEY = "UserInfo";
    private readonly IUserStore userStore;
    private readonly IProtectedLocalStorage protectedLocalStorage;
    private readonly NavigationManager navigationManager;
    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
    public LocalAuthenticationStateProvider(ILoggerFactory loggerFactory
        , IServiceProvider provider
        , IUserStore userStore
        , IProtectedLocalStorage protectedLocalStorage
        , NavigationManager navigationManager) : base(loggerFactory)
    {
        AuthService = provider.GetService<IAuthService>();

        this.userStore = userStore;
        this.protectedLocalStorage = protectedLocalStorage;
        this.navigationManager = navigationManager;
    }
    public IAuthService? AuthService { get; }

    public UserInfo? Current => userStore.UserInfo;

    protected override TimeSpan RevalidationInterval => throw new NotImplementedException();

    public async Task ClearState()
    {
        await protectedLocalStorage.DeleteAsync(USER_KEY);
        NotifyAuthenticationStateChanged(defaultUnauthenticatedTask);
        var redirect = navigationManager.ToAbsoluteUri(navigationManager.Uri);
        var encoded = Uri.EscapeDataString(redirect.PathAndQuery);
        navigationManager.NavigateTo($"/account/login?Redirect={encoded}");
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var v = await protectedLocalStorage.GetAsync<UserInfo>(USER_KEY);
            if (v.Success)
            {
                userStore.SetUser(v.Value);
                var p = v.Value!.BuildClaims("Local");
                return new AuthenticationState(new ClaimsPrincipal(p));
            }
            return await defaultUnauthenticatedTask;
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

}

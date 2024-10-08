namespace Project.Web.Shared.Auth;

[AutoInject(Group = "WASM", ServiceType = typeof(IAuthenticationStateProvider))]
[AutoInject(Group = "WASM", ServiceType = typeof(AuthenticationStateProvider))]
public class PersistentAuthenticationStateProvider : AuthenticationStateProvider, IAuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly IAppSession appSession;
    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;
    private readonly ILogger<IAuthenticationStateProvider> logger;

    public PersistentAuthenticationStateProvider(PersistentComponentState state
        , IAppSession appSession
        , ILogger<IAuthenticationStateProvider> logger)
    {
        this.appSession = appSession;
        this.logger = logger;

        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var u) || u is null) return;
        UserStore.SetUser(u);
        authenticationStateTask = Task.FromResult(new AuthenticationState(BuildClaims(u)));
    }

    private IUserStore UserStore => appSession.UserStore;

    public Task ClearState()
    {
        //appSession.UserStore.ClearUser();
        //await httpContextAccessor.HttpContext.SignOutAsync();
        var redirect = appSession.Navigator.ToBaseRelativePath(appSession.Navigator.Uri);
        appSession.Navigator.NavigateTo($"/api/account/logout?Redirect={HttpUtility.UrlEncode(redirect)}", true);
        return Task.CompletedTask;
    }

    public UserInfo? Current => UserStore.UserInfo;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return authenticationStateTask;
    }

    private static ClaimsPrincipal BuildClaims(UserInfo info)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, info.UserId),
            new(ClaimTypes.GivenName, info.UserName!),
            new(nameof(UserInfo.Token), info.Token)
        };
        claims.AddRange(info.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(PersistentAuthenticationStateProvider)));
    }
}
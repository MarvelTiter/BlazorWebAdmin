using AutoInjectGenerator;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Project.Constraints;
using Project.Constraints.Options;
using Project.Constraints.Store;
using Project.Web.Shared.Store;
using System.Security.Claims;

namespace Project.Web.Shared.Auth;

[AutoInject(ServiceType = typeof(IAuthenticationStateProvider))]
[AutoInject(ServiceType = typeof(AuthenticationStateProvider))]
public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IAuthenticationStateProvider
{
    private readonly IProtectedLocalStorage storageService;
    private readonly IAuthenticationService authenticationService;
    private readonly IAppSession appSession;
    private readonly ILogger<IAuthenticationStateProvider> logger;

    private IUserStore Store => appSession.UserStore;
    private IAppStore AppStore => appSession.AppStore;

    private readonly IOptionsMonitor<Token> token;
    public CustomAuthenticationStateProvider(IProtectedLocalStorage storageService
        , IAuthenticationService authenticationService
        , IAppSession appSession
        , ILogger<IAuthenticationStateProvider> logger
        , IOptionsMonitor<Token> token)
    {
        this.storageService = storageService;
        this.authenticationService = authenticationService;
        this.appSession = appSession;
        this.logger = logger;
        this.token = token;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var app = await storageService.GetAsync<AppStore>(ConstraintString.APP_STORE_KEY);
            logger.LogInformation(app.Value?.AppLanguage);
            AppStore.ApplySetting(app.Value);
            if (token.CurrentValue.NeedAuthentication)
            {
                var result = await storageService.GetAsync<UserInfo>("UID");
                logger.LogInformation(result.Value?.UserName);
                var diff = DateTime.Now - result.Value?.CreatedTime;
                var actived = DateTime.Now - result.Value?.ActiveTime;
                if (result.Success && (diff?.Days < token.CurrentValue.Expire || actived?.TotalSeconds < token.CurrentValue.LimitedFreeTime))
                {
                    //await loginService.UpdateLastLoginTimeAsync(result.Value!);
                    return await UpdateState(result.Value);
                }
            }
        }
        catch (Exception)
        {
        }
        return await UpdateState();
    }

    async Task<AuthenticationState> UpdateState(UserInfo? info = null)
    {
        ClaimsIdentity identity;
        if (info != null)
        {
            identity = Build(info!);
        }
        else
        {
            identity = new ClaimsIdentity();
        }
        await Store.SetUserAsync(info);
        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public async Task IdentifyUser(UserInfo info)
    {
        logger.LogInformation("IdentifyUser");
        await storageService.SetAsync("UID", info);
        NotifyAuthenticationStateChanged(UpdateState(info));
    }

    public async Task ClearState()
    {
        if (token.CurrentValue.NeedAuthentication)
        {
        }
        await storageService.DeleteAsync("UID");
        NotifyAuthenticationStateChanged(UpdateState());
    }

    public UserInfo? Current => Store.UserInfo;
    private static ClaimsIdentity Build(UserInfo info)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, info.UserId),
            new Claim(ClaimTypes.GivenName, info.UserName!),
        };
        foreach (var r in info.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, r));
        }
        return new ClaimsIdentity(claims, "authentication");
    }

    //private UserInfo Deserialize(string json)
    //{
    //    if (string.IsNullOrEmpty(json)) return null;
    //    return System.Text.Json.JsonSerializer.Deserialize<UserInfo>(json);
    //}
}

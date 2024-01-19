using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using Project.AppCore.Store;
using Project.Constraints;
using Project.Constraints.Common.Attributes;
using Project.Constraints.Options;
using Project.Constraints.Store;
using System.Security.Claims;

namespace Project.AppCore.Auth
{
    [IgnoreAutoInject]
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IAuthenticationStateProvider
    {
        private readonly ProtectedLocalStorage storageService;
        private readonly ILoginService loginService;
        private readonly IUserStore store;
        private readonly IAppStore appStore;
        private readonly IOptionsMonitor<Token> token;

        public CustomAuthenticationStateProvider(ProtectedLocalStorage storageService
            , ILoginService loginService
            , IUserStore store
            , IAppStore appStore
            , IOptionsMonitor<Token> token)
        {
            this.storageService = storageService;
            this.loginService = loginService;
            this.store = store;
            this.appStore = appStore;
            this.token = token;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var app = await storageService.GetAsync<AppStore>(AppStore.KEY);
                appStore.ApplySetting(app.Value);
                if (token.CurrentValue.NeedAuthentication)
                {
                    var result = await storageService.GetAsync<UserInfo>("UID");
                    var diff = DateTime.Now - result.Value?.CreatedTime;
                    var actived = DateTime.Now - result.Value?.ActiveTime;
                    if (result.Success && (diff?.Days < token.CurrentValue.Expire || actived?.TotalSeconds < token.CurrentValue.LimitedFreeTime))
                    {
                        await loginService.UpdateLastLoginTimeAsync(result.Value!);
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
            if (info != null && await loginService.CheckUser(info!))
            {
                identity = Build(info!);
            }
            else
            {
                identity = new ClaimsIdentity();
            }
            store.SetUser(info);
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task IdentifyUser(UserInfo info)
        {
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

        public UserInfo? Current => store.UserInfo;
        private static ClaimsIdentity Build(UserInfo info)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, info.UserId),
                new Claim(ClaimTypes.GivenName, info.UserName),
            };
            foreach (var r in info.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, r));
            }
            return new ClaimsIdentity(claims, "authentication");
        }

        private UserInfo Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            return System.Text.Json.JsonSerializer.Deserialize<UserInfo>(json);
        }
    }
}

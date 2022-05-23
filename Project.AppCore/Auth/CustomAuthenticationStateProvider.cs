using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Project.AppCore.Services;
using Project.Common.Attributes;
using Project.Models.Permissions;
using System.Security.Claims;

namespace Project.AppCore.Auth
{
    [IgnoreAutoInject]
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService storageService;
        private readonly ILoginService loginService;

        public CustomAuthenticationStateProvider(ISessionStorageService storageService, ILoginService loginService)
        {
            this.storageService = storageService;
            this.loginService = loginService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string? json = await storageService.GetItemAsync("UID");
            return await UpdateState(Deserialize(json));
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

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task IdentifyUser(UserInfo info)
        {
            await storageService.SetItemAsync("UID", System.Text.Json.JsonSerializer.Serialize(info));
            NotifyAuthenticationStateChanged(UpdateState(info));
        }

        public async Task ClearState()
        {
            await storageService.SetItemAsync("UID", null);
            NotifyAuthenticationStateChanged(UpdateState());
        }

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

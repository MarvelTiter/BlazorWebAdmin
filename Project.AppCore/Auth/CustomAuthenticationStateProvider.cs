using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Project.AppCore.Services;
using Project.AppCore.Store;
using Project.Common.Attributes;
using Project.Models.Permissions;
using System.Security.Claims;

namespace Project.AppCore.Auth
{
	[IgnoreAutoInject]
	public class CustomAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly ProtectedLocalStorage storageService;
		private readonly ILoginService loginService;
		private readonly UserStore store;
        private readonly AppStore appStore;
        private readonly int tokenExpire;

		public CustomAuthenticationStateProvider(ProtectedLocalStorage storageService
			, ILoginService loginService
			, UserStore store
			, AppStore appStore
			, IConfiguration configuration)
		{
			this.storageService = storageService;
			this.loginService = loginService;
			this.store = store;
            this.appStore = appStore;
            this.tokenExpire = configuration.GetSection("Token").GetValue<int>("Expire");
		}
		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			try
			{
				var app = await storageService.GetAsync<AppStore>(AppStore.KEY);
				if (app.Success)
				{
					appStore.ApplySetting(app.Value);
				}
				var result = await storageService.GetAsync<UserInfo>("UID");
				if (result.Success && (DateTime.Now - result.Value?.CreatedTime)?.Days < tokenExpire)
				{
					await loginService.UpdateLastLoginTimeAsync(result.Value!);
					return await UpdateState(result.Value);
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
			await storageService.SetAsync("UID", null);
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

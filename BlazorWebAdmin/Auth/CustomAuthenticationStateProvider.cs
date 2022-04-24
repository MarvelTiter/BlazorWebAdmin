﻿using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Project.Services;
using Project.Services.interfaces;
using System.Security.Claims;

namespace BlazorWebAdmin.Auth
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage sessionStorage;
        private readonly ILoginService loginService;

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage, ILoginService loginService)
        {
            this.sessionStorage = sessionStorage;
            this.loginService = loginService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var info = await sessionStorage.GetAsync<UserInfo>("UID");
            ClaimsIdentity identity;
            if (info.Success)
            {
                identity = Build(info.Value!);
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
            await sessionStorage.SetAsync("UID", info);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task ClearState()
        {
            await sessionStorage.SetAsync("UID", null!);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private ClaimsIdentity Build(UserInfo info)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, info.UserName));
            foreach (var r in info.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, r));
            }
            return new ClaimsIdentity(claims);
        }
    }
}

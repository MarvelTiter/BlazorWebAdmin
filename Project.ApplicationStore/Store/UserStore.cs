using Project.Models.Forms;
using Project.Services.interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Project.Services;
using Project.ApplicationStore.Auth;

namespace Project.ApplicationStore.Store
{
    public class UserStore : StoreBase
    {
        private readonly ILoginService loginService;
        private readonly CustomAuthenticationStateProvider auth;
        private readonly RouterStore routerStore;
        private readonly NavigationManager navigationManager;

        public UserStore(ILoginService loginService, AuthenticationStateProvider authenticationStateProvider, RouterStore routerStore, NavigationManager navigationManager)
        {
            this.loginService = loginService;
            auth = (CustomAuthenticationStateProvider)authenticationStateProvider;
            this.routerStore = routerStore;
            this.navigationManager = navigationManager;
        }
        public UserInfo? UserInfo { get; set; }
        public IEnumerable<string> Roles => UserInfo?.Roles;
        public string? UserId => UserInfo?.UserId;
        public string UserDisplayName => GetUserName();

        private string GetUserName()
        {
            return UserInfo?.UserName ?? "Unknow";
        }

        public async Task Init(UserInfo userInfo)
        {
            UserInfo = userInfo;
            await routerStore.InitRoutersAsync(userInfo);
        }

        public async Task<string> LoginAsync(LoginFormModel loginForm)
        {
            var flag = await loginService.LoginAsync(loginForm.UserName, loginForm.Password);
            if (flag.Success)
            {
                UserInfo = flag.Payload;
                await auth.IdentifyUser(flag.Payload);
                await routerStore.InitRoutersAsync(flag.Payload);
                navigationManager.NavigateTo("/");
                return "登录成功";
            }
            else
            {
                return flag.Message;
            }
        }

        public async Task LogoutAsync()
        {
            await auth.ClearState();
            UserInfo = null;
        }
    }
}

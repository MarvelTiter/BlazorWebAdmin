using Project.Models.Forms;
using Project.Services.interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using AntDesign;
using BlazorWebAdmin.Auth;
using Project.Services;

namespace BlazorWebAdmin.Store
{
    public class UserStore : StoreBase
    {
        private readonly ILoginService loginService;
        private readonly CustomAuthenticationStateProvider auth;
        private readonly RouterStore routerStore;
        private readonly NavigationManager navigationManager;
        private readonly MessageService messageService;

        public UserStore(ILoginService loginService, AuthenticationStateProvider authenticationStateProvider, RouterStore routerStore, NavigationManager navigationManager, MessageService messageService)
        {
            this.loginService = loginService;
            this.auth = (CustomAuthenticationStateProvider)authenticationStateProvider;
            this.routerStore = routerStore;
            this.navigationManager = navigationManager;
            this.messageService = messageService;
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

        public async Task LoginAsync(LoginFormModel loginForm)
        {
            var flag = await loginService.LoginAsync(loginForm.UserName, loginForm.Password);
            if (flag.Success)
            {
                UserInfo = flag.Payload;
                await auth.IdentifyUser(flag.Payload);
                await routerStore.InitRoutersAsync(flag.Payload);
                navigationManager.NavigateTo("/");
            }
            else
            {
                await messageService.Error(flag.Message);
            }
        }

        public async Task LogoutAsync()
        {
            await auth.ClearState();
            UserInfo = null;
        }
    }
}

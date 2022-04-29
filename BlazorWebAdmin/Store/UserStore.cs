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

        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
        public string UserId { get; set; }
        public string UserDisplayName => GetUserName();

        private string GetUserName()
        {
            return "管理员";
        }

        public async Task Init(string name, IEnumerable<string> roles)
        {
            UserId = name;
            Roles = roles.ToList();
            await routerStore.InitRoutersAsync();
        }

        public async Task LoginAsync(LoginFormModel loginForm)
        {
            var flag = await loginService.LoginAsync(loginForm.UserName, loginForm.Password);
            if (flag.Success)
            {
                UserId = flag.Payload.UserId;
                Roles = flag.Payload.Roles;
                await auth.IdentifyUser(flag.Payload);
                await routerStore.InitRoutersAsync();
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
            Roles = Enumerable.Empty<string>();
            UserId = "";
        }
    }
}

using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Project.AppCore.Auth;
using Project.AppCore.Services;
using Project.AppCore.Store;
using Project.Models.Forms;

namespace BlazorWebAdmin.Pages
{
    public partial class Login
    {
        private LoginFormModel model = new LoginFormModel();
        [Inject]
        public UserStore UserStore { get; set; }
        [Inject]
        public RouterStore RouterStore { get; set; }
        [Inject]
        public MessageService MessageSrv { get; set; }
        [Inject]
        public ILoginService LoginSrv { get; set; }
        [Inject]
        public AuthenticationStateProvider Auth { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public bool Loading { get; set; } = false;
        private async Task HandleLogin()
        {
            Loading = true;
            var result = await LoginSrv.LoginAsync(model.UserName, model.Password);
            if (result.Success)
            {
                UserStore.SetUser(result.Payload);
                await ((CustomAuthenticationStateProvider)Auth).IdentifyUser(result.Payload);
                await RouterStore.InitRoutersAsync(result.Payload);
                _ = MessageSrv.Info("登录成功");
                NavigationManager.NavigateTo("/");
            }
            else
            {
                _ = MessageSrv.Info(result.Message);
            }
            Loading = false;
        }
    }
}

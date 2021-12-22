using AntDesign;
using BlazorWebAdmin.IServices;
using BlazorWebAdmin.Models;
using BlazorWebAdmin.Models.Forms;
using BlazorWebAdmin.Store;
using Microsoft.AspNetCore.Components;

namespace BlazorWebAdmin.Pages
{
    public partial class Login
    {
        private LoginFormModel model = new LoginFormModel();
        [Inject]
        public NavigationManager Navigator { get; set; }

        [Inject]
        public IUserService UserSrv { get; set; }
        [Inject]
        public MessageService MessageSrv { get; set; }
        [Inject]
        public UserStore Store { get; set; }
        public bool Loading { get; set; } = false;

        private async Task HandleLogin()
        {
            Loading = true;
            var user = UserSrv.Login(model);
            if (user == null)
            {
                await MessageSrv.Error("用户名或者密码错误!");
                Loading = false;
                return;
            }
            Store.UserId = user.UserId;
            Store.UserName = user.UserName;
            Store.Permissions = UserSrv.GetUserPermission("");            
            Navigator.NavigateTo("/");
        }
    }
}

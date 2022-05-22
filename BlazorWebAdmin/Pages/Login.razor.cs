using AntDesign;
using Microsoft.AspNetCore.Components;
using Project.ApplicationStore;
using Project.ApplicationStore.Store;
using Project.Models.Forms;

namespace BlazorWebAdmin.Pages
{
    public partial class Login
    {
        private LoginFormModel model = new LoginFormModel();
        [Inject]
        public UserStore UserStore { get; set; }
        [Inject]
        public MessageService MessageSrv { get; set; }
        public bool Loading { get; set; } = false;
        private async Task HandleLogin()
        {
            Loading = true;
            var msg = await UserStore.LoginAsync(model);
            _ = MessageSrv.Info(msg);
            Loading = false;
        }
    }
}

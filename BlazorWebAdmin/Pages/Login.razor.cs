using AntDesign;
using BlazorWebAdmin.Store;
using Microsoft.AspNetCore.Components;
using Project.Models.Forms;

namespace BlazorWebAdmin.Pages
{
    public partial class Login
    {
        private LoginFormModel model = new LoginFormModel();
        [Inject]
        public UserStore UserStore { get; set; }
        public bool Loading { get; set; } = false;
        private async Task HandleLogin()
        {
            Loading = true;
            await UserStore.LoginAsync(model);
            Loading = false;
        }
    }
}

using BlazorWebAdmin.Store;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Project.IServices;
using Project.Models.Forms;

namespace BlazorWebAdmin.Pages
{
    public partial class Login
    {
        private LoginFormModel model = new LoginFormModel();
        [Inject]
        public NavigationManager Navigator { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }
        [Inject]
        public UserStore UserStore { get; set; }
        [Inject]
        public RouterStore RouterStore { get; set; }
        public bool Loading { get; set; } = false;

        private async Task HandleLogin()
        {
            Loading = true;
            var message = await UserStore.LoginAsync(model);
            Loading = false;
            if (message is null)
            {
                await RouterStore.InitRoutersAsync();
                Navigator.NavigateTo("/");
            }
            else
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add(message,Severity.Error);
            }
            return;
        }
    }
}

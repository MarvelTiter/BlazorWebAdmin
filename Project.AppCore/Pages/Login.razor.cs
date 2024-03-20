using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Project.Constraints.Models;
using Project.Constraints.Page;
using Project.Constraints.Services;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using System.Web;

namespace Project.AppCore.Pages
{
    public partial class Login : BasicComponent
    {
        private LoginFormModel model = new LoginFormModel();
        [Inject] public ILoginService LoginSrv { get; set; }
        [Inject] public AuthenticationStateProvider Auth { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] IStringLocalizer<Login> Localizer { get; set; }
        [Inject] ICustomSettingService CustomSetting { get; set; }
        [CascadingParameter] IDomEventHandler Root { get; set; }
        public bool Loading { get; set; } = false;
        public string? Redirect { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            var full = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            Redirect = HttpUtility.ParseQueryString(full.Query).Get(nameof(Redirect));
            Root.OnKeyDown += OnPressEnter;
        }

        private async Task OnPressEnter(KeyboardEventArgs e)
        {
            //Console.WriteLine("OnPressEnter: " + e.Key);
            if (e.Key == "Enter")
            {
                if (Loading)
                {
                    return;
                }
                await HandleLogin();
            }
        }

        private async Task HandleLogin()
        {
            Loading = true;
            var result = await LoginSrv.LoginAsync(model.UserName, model.Password);
            if (result.Success)
            {
                User.SetUser(result.Payload);
                await AuthenticationStateProvider.IdentifyUser(result.Payload);
                await Router.InitRoutersAsync(result.Payload);
                UI.Success(Localizer["Login.SuccessTips"].Value);
                Root.OnKeyDown -= OnPressEnter;
                var goon = await CustomSetting.LoginSuccessAsync(result);
                if (goon)
                {
                    if (string.IsNullOrEmpty(Redirect))
                        NavigationManager.NavigateTo("/");
                    else
                        NavigationManager.NavigateTo(Redirect);
                }
            }
            else
            {
                UI.Error(result.Message);
            }
            Loading = false;
            //StateHasChanged();
        }
    }
}

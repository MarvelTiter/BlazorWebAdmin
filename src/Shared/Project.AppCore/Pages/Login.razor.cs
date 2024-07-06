using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Project.Constraints.Models;
using Project.Constraints.Page;
using Project.Constraints.Services;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace Project.AppCore.Pages
{
    public partial class Login : BasicComponent
    {
        private LoginFormModel model = new LoginFormModel();
        [Inject, NotNull] public ILoginService? LoginSrv { get; set; }
        [Inject, NotNull] IStringLocalizer<Login>? Localizer { get; set; }
        [Inject, NotNull] IProjectSettingService? CustomSetting { get; set; }
        [CascadingParameter, NotNull] IDomEventHandler? Root { get; set; }
        public bool Loading { get; set; } = false;
        public string? Redirect { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            var full = Navigator.ToAbsoluteUri(Navigator.Uri);
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
            await InvokeAsync(StateHasChanged);
            var result = await LoginSrv.LoginAsync(model.UserName, model.Password);
            if (result.Success)
            {
                await User.SetUserAsync(result.Payload);
                await AuthenticationStateProvider.IdentifyUser(result.Payload!);
                await Router.InitRoutersAsync(result.Payload);
                UI.Success(Localizer["Login.SuccessTips"].Value);
                Root.OnKeyDown -= OnPressEnter;
                var goon = await CustomSetting.LoginInterceptorAsync(result.Payload!);
                if (goon)
                {
                    if (string.IsNullOrEmpty(Redirect))
                        Navigator.NavigateTo("/");
                    else
                        Navigator.NavigateTo(Redirect);
                }
            }
            else
            {
                UI.Error(result.Message!);
            }
            Loading = false;
            //StateHasChanged();
        }
    }
}

using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using Project.Constraints.Utils;

namespace Project.Web.Shared.Pages;

public partial class Login : BasicComponent
{
    private readonly LoginFormModel model = new();
    [Inject] [NotNull] public IAuthenticationService? AuthService { get; set; }
    [Inject] [NotNull] private IStringLocalizer<Login>? Localizer { get; set; }
    [Inject] [NotNull] private IProjectSettingService? CustomSetting { get; set; }
    [Inject] [NotNull] private ILogger<Login>? Logger { get; set; }
    [CascadingParameter] [NotNull] private IDomEventHandler? Root { get; set; }
    public bool Loading { get; set; }
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
            if (Loading) return;

            await HandleLogin();
        }
    }

    private async Task HandleLogin()
    {
        //Loading = true;
        BooleanStatusManager.New(b => Loading = b, callback: () => InvokeAsync(StateHasChanged));
        //await InvokeAsync(StateHasChanged);
        var result = await AuthService.SignInAsync(model);
        if (result.Success)
        {
            await User.SetUserAsync(result.Payload);
            UI.Success(Localizer["Login.SuccessTips"].Value);
            Root.OnKeyDown -= OnPressEnter;
            var goon = await CustomSetting.LoginInterceptorAsync(result.Payload!);
            if (goon.Success)
            {
                await Router.InitRoutersAsync(result.Payload);
                await AuthenticationStateProvider.IdentifyUser(result.Payload!);
                if (string.IsNullOrEmpty(Redirect))
                {
                    Navigator.NavigateTo("/");
                }
                else
                {
                    Navigator.NavigateTo(Redirect);
                }
            }
            else
            {
                UI.Error(goon.Message!);
            }
        }
        else
        {
            UI.Error(result.Message!);
        }
        //StateHasChanged();
    }
}
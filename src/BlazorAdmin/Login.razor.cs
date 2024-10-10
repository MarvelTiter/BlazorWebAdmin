using System.Diagnostics.CodeAnalysis;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Project.AppCore.Auth;
using Project.Constraints.Models;
using Project.Constraints.Options;
using Project.Constraints.Page;
using Project.Constraints.Services;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using Project.Constraints.Utils;

//using Microsoft.AspNetCore.Authentication;

namespace BlazorAdmin;

public partial class Login : BasicComponent
{
    //private readonly LoginFormModel model = new();
    [Inject] [NotNull] public IAuthService? AuthService { get; set; }
    [Inject] [NotNull] private IStringLocalizer<Login>? Localizer { get; set; }
    [Inject] [NotNull] private IProjectSettingService? CustomSetting { get; set; }
    [Inject] [NotNull] private ILogger<Login>? Logger { get; set; }
    [Inject] [NotNull] private IOptionsMonitor<Token>? TokenOption { get; set; }
    [CascadingParameter] [NotNull] private IDomEventHandler? Root { get; set; }
    [CascadingParameter] [NotNull] private HttpContext? HttpContext { get; set; }
    public bool Loading { get; set; }
    public string? Redirect { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var full = Navigator.ToAbsoluteUri(Navigator.Uri);
        Redirect = HttpUtility.ParseQueryString(full.Query).Get(nameof(Redirect));
        //Root.OnKeyDown += OnPressEnter;
        var refer = HttpContext.Request.Headers.Referer.ToString();
        if (!string.IsNullOrEmpty(refer)) UI.Error("登录凭证超时，请重新登录");
    }

    //private async Task OnPressEnter(KeyboardEventArgs e)
    //{
    //    //Console.WriteLine("OnPressEnter: " + e.Key);
    //    if (e.Key == "Enter")
    //    {
    //        if (Loading) return;

    //        await HandleLogin();
    //    }
    //}

    private async Task HandleLogin(LoginFormModel model)
    {
        //Loading = true;
        using var _ = BooleanStatusManager.New(b => Loading = b, callback: () => InvokeAsync(StateHasChanged));
        //await InvokeAsync(StateHasChanged);
        var result = await AuthService.SignInAsync(model);
        if (result.IsSuccess)
        {
            User.SetUser(result.Payload);
            UI.Success(Localizer["Login.SuccessTips"].Value);
            //Root.OnKeyDown -= OnPressEnter;
            var goon = await CustomSetting.LoginInterceptorAsync(result.Payload!);
            if (goon.IsSuccess)
            {
                await Router.InitRoutersAsync(result.Payload);
                //await AuthenticationStateProvider.IdentifyUser(result.Payload!);

                await HttpContext.SignInAsync(result.Payload!.BuildClaims(), new AuthenticationProperties
                {
                    IsPersistent = true,
                    //cookie过期是单独指cookie，这里的是指
                    ExpiresUtc = TimeProvider.System.GetUtcNow().Add(TokenOption.CurrentValue.Expire)
                });

                if (string.IsNullOrEmpty(Redirect)) Redirect = "/";
                Navigator.NavigateTo(Redirect, true);
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
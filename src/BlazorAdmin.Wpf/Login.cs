using BlazorAdmin.Wpf.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Project.Constraints.Models;
using Project.Constraints.Options;
using Project.Constraints.Page;
using Project.Constraints.Services;
using Project.Constraints.Store;
using Project.Constraints.UI.Extensions;
using Project.Constraints.Utils;
using Project.Web.Shared.Layouts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BlazorAdmin.Wpf;

[Route("/account/login")]
[Layout(typeof(BlankLayout))]
public class Login : SystemPageIndex<Login>, ILoginPage
{
    [Inject, NotNull] public IServiceProvider? Provider { get; set; }
    [Inject, NotNull] private IProjectSettingService? CustomSetting { get; set; }
    [Inject, NotNull] private IOptionsMonitor<Token>? TokenOption { get; set; }
    [Inject, NotNull] IProtectedLocalStorage? LocalStorage { get; set; }
    public bool Loading { get; set; }
    public string? Redirect { get; set; }
    protected override bool CascadingSelf => true;
    protected override void OnInitialized()
    {
        base.OnInitialized();
        var full = Navigator.ToAbsoluteUri(Navigator.Uri);
        Redirect = HttpUtility.ParseQueryString(full.Query).Get(nameof(Redirect));
        //Root.OnKeyDown += OnPressEnter;
        //var refer = HttpContext.Request.Headers.Referer.ToString();
        //if (!string.IsNullOrEmpty(refer)) UI.Error("登录凭证超时，请重新登录");
    }

    public async Task HandleLogin(LoginFormModel model)
    {
        try
        {
            using var _ = BooleanStatusManager.New(b => Loading = b, callback: () => InvokeAsync(StateHasChanged));
            var authService = Provider.GetService<IAuthService>();
            if (authService is null)
            {
                UI.Error("未配置登录功能!");
                return;
            }
            var result = await authService.SignInAsync(model);
            if (result.IsSuccess)
            {
                User.SetUser(result.Payload);
                var goon = await CustomSetting.LoginInterceptorAsync(result.Payload!);
                if (goon.IsSuccess)
                {
                    await LocalStorage.SetAsync(LocalAuthenticationStateProvider.USER_KEY, result.Payload!);
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
        }
        catch (Exception ex)
        {
            UI.Error(ex.Message);
        }
    }

    protected override Type? GetPageType(IPageLocatorService customSetting)
    {
        return customSetting.GetLoginPageType();
    }
}

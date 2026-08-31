using BlazorTemplate.AppServer;
using BlazorTemplate.ClientCore.Components;
using BlazorTemplate.ClientCore.Options;
using BlazorTemplate.ClientCore.Store.Models;
using BlazorTemplate.ClientCore.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace BlazorAdmin;

partial class App
{
    private readonly RenderFragment loadBlazorFrameworkScript;
    [CascadingParameter, NotNull] public HttpContext? HttpContext { get; set; }
    public IOptions<AppSetting> AppOption { get; set; }
    public IOptions<Token> TokenOption { get; set; }
    public IUIService UIService { get; set; }
    IComponentRenderMode? RenderMode
    {
        get
        {
            if (!HttpContext.AcceptsInteractiveRouting())
            {
                return null;
            }
            return AppOption.Value.RunMode switch
            {
                AppRunMode.Auto => new InteractiveAutoRenderMode(false),
                AppRunMode.Server => new InteractiveServerRenderMode(false),
                AppRunMode.WebAssembly => new InteractiveWebAssemblyRenderMode(false),
                _ => throw new ArgumentException()
            };
        }
    }
    public App(IOptions<AppSetting> appOption, IOptions<Token> tokenOption, IUIService uIService)
    {
        AppOption = appOption;
        TokenOption = tokenOption;
        UIService = uIService;
        //-:cnd:noEmit
#if NET8_0
        loadBlazorFrameworkScript = b =>
        {
            b.OpenComponent<VScript>(0);
            b.AddAttribute(1, nameof(VScript.Src), "_framework/blazor.web.js");
            b.CloseComponent();
        };
#else
        if (AppOption.Value.RunMode == AppRunMode.Server)
        {
            loadBlazorFrameworkScript = b =>
            {
                b.OpenComponent<LegacyBlazorJs.Loader>(0);
                b.AddAttribute(1, nameof(LegacyBlazorJs.Loader.Target), "es2015");
                b.CloseComponent();
            };
        }
        else
        {
            loadBlazorFrameworkScript = b =>
            {
                b.OpenComponent<VScript>(0);
                b.AddAttribute(1, nameof(VScript.Src), "_framework/blazor.web.js");
                b.CloseComponent();
            };
        }
#endif
        //+:cnd:noEmit
    }
}

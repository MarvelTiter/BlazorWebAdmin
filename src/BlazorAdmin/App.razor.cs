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
    private readonly Lazy<RenderFragment> loadBlazorFrameworkScript;
    private readonly Lazy<RenderFragment> loadResources;
    [CascadingParameter, NotNull] public HttpContext? HttpContext { get; set; }
    [Inject, NotNull] public IOptions<AppSetting>? AppOption { get; set; }
    [Inject, NotNull] public IOptions<Token>? TokenOption { get; set; }
    [Inject, NotNull] public IUIService? UIService { get; set; }
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
    public App()
    {
        //-:cnd:noEmit
#if NET8_0
        loadResources = new(b => { });
        loadBlazorFrameworkScript = new(b =>
        {
            b.OpenComponent<VScript>(0);
            b.AddAttribute(1, nameof(VScript.Src), "_framework/blazor.web.js");
            b.CloseComponent();
        });
#else
        loadResources = new(b =>
        {
            b.OpenComponent<ResourcePreloader>(0);
            b.CloseComponent();
            b.OpenComponent<ImportMap>(0);
            b.CloseComponent();
        });
        loadBlazorFrameworkScript = new(b =>
        {
            if (AppOption.Value.RunMode == AppRunMode.Server)
            {
                b.OpenComponent<LegacyBlazorJs.Loader>(0);
                b.AddAttribute(1, nameof(LegacyBlazorJs.Loader.Target), "es2015");
                b.CloseComponent();
            }
            else
            {
                b.OpenComponent<VScript>(0);
                b.AddAttribute(1, nameof(VScript.Src), "_framework/blazor.web.js");
                b.CloseComponent();
            }
        });
#endif
        //+:cnd:noEmit
    }
}

using Project.Constraints.PageHelper;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Utils;

namespace Project.Web.Shared.Pages;

public partial class Dashboard : SystemPageIndex<Dashboard>
{
    private Type? homeType;
    private Task Update()
    {
        return InvokeAsync(StateHasChanged);
    }

    protected override Type? GetPageType(IPageLocatorService customSetting)
    {
        homeType ??= Locator.GetDashboardType();
        return homeType;
    }

    static string RenderMode => OperatingSystem.IsBrowser() ? "WebAssembly" : "Server";
    private RenderFragment CurrentRenderMode()
    {
#if DEBUG
        // @<p>RenderMode: @(IsWasm ? "WebAssembly" : "Server") @UI.BuildButton(this).OnClick(() => IsWasm = OperatingSystem.IsBrowser()).Text("刷新").Render() </p>;
        return b => b.Span().AddContent(s =>
        {
            s.AddContent(0, "RenderMode: ");
            s.AddContent(1, RenderMode);
            s.AddContent(2, UI.BuildButton(this).OnClick(StateHasChanged).Text("刷新").Render());

        }).Build();
#else
        return b => { };
#endif
    }

    private RenderFragment RenderCustomHomePage() => base.BuildRenderTree;
}
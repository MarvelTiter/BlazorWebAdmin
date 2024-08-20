using Project.Constraints.UI.Extensions;

namespace Project.Web.Shared.Pages;

public partial class Dashboard
{
    private Type? homeType;
    private bool IsWasm;
    [Inject] [NotNull] private IPageLocatorService? Locator { get; set; }

    private Task Update()
    {
        // if (User?.UserInfo != null)
        // {
        //     User.UserInfo.ActiveTime = DateTime.Now;
        // }

        return InvokeAsync(StateHasChanged);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        homeType = Locator.GetDashboardType();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        IsWasm = OperatingSystem.IsBrowser();
    }

    private RenderFragment CurrentRenderMode()
    {
#if DEBUG
        // @<p>RenderMode: @(IsWasm ? "WebAssembly" : "Server") @UI.BuildButton(this).OnClick(() => IsWasm = OperatingSystem.IsBrowser()).Text("刷新").Render() </p>;
        return b => b.Span().AddContent(s =>
        {
            s.AddContent(0, "RenderMode: ");
            s.AddContent(1, IsWasm ? "WebAssembly" : "Server");
            s.AddContent(2,
                UI.BuildButton(this).OnClick(() => IsWasm = OperatingSystem.IsBrowser()).Text("刷新").Render());
        }).Build();
    }
#else
        return b => b.Span().Build();
#endif
}
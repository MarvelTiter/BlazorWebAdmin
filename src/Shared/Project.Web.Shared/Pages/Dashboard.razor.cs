using Project.Constraints.PageHelper;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Utils;

namespace Project.Web.Shared.Pages;

public partial class Dashboard //: IRoutePage
{
    private Type? homeType;
    private IRoutePage? homePage;
    [Inject][NotNull] private IPageLocatorService? Locator { get; set; }
    [Inject, NotNull] IRouterStore? RouterStore { get; set; }
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

    private RenderFragment RenderCustomHomePage()
    {
        if (homeType is null)
        {
            return b =>
            {
                b.AddContent(0, "未找到首页组件，请联系管理员配置");
            };
        }
        return b =>
        {
            b.OpenComponent(0, homeType);
            b.AddComponentReferenceCapture(1, obj =>
            {
                homePage = obj as IRoutePage;
                //if (RouterStore.Current?.RouteId == "Home")
                //{
                //    RouterStore.Current.Title = (homePage?.GetTitle() ?? "主页").AsContent();
                //}
            });
            b.CloseComponent();
        };
    }

    //public string? GetTitle()
    //{
    //    if (homeType is null)
    //        return null;
    //    return homePage?.GetTitle() ?? null;
    //}
}
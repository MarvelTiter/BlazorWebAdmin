using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Web.Shared.Layouts;
namespace BlazorAdmin.Wpf;

public class Routers : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // <AppRoot DefaultLayout="@typeof(MainLayout)"></AppRoot>
        builder.OpenComponent<AppRoot>(0);
        builder.AddAttribute(1, nameof(AppRoot.DefaultLayout), typeof(MainLayout));
        builder.CloseComponent();
    }
}
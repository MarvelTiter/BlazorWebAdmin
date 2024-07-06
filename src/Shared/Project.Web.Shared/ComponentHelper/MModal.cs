using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Page;
using Project.Constraints.UI.Flyout;

namespace Project.Web.Shared.ComponentHelper
{
    public class MModal : BasicComponent
    {
        [Parameter] public bool Visible { get; set; }
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
        [Parameter] public string? Title { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string? Width { get; set; }
        [Parameter] public bool HideDefaultFooter { get; set; }
        //FlyoutOptions
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            UI.BuildModal()
                  .Set(p => p.Title, Title)
                  .Set(p => p.ChildContent, ChildContent)
                  .Set(p => p.Visible, Visible)
                  .Set(p => p.VisibleChanged, VisibleChanged)
                  .Set(p => p.HideDefaultFooter, HideDefaultFooter)
                  .Set(p => p.Width, Width)
                  .Render().Invoke(builder);
        }
    }
}

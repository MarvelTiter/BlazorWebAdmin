using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Page;

namespace Project.Web.Shared.Components
{
    public class MCard : BasicComponent
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            UI.BuildCard()
                .Set("ChildContent", ChildContent)
                .Set("Title", Title)
                .AdditionalParameters(AdditionalParameters)
                .Render()
                .Invoke(builder);
        }
    }

    public class MRow : BasicComponent
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            UI.BuildRow()
                .Set("ChildContent", ChildContent)
                .AdditionalParameters(AdditionalParameters)
                .Render()
                .Invoke(builder);
        }
    }

    public class MCol : BasicComponent
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public int ColSpan { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            UI.BuildCol()
                .Set("ChildContent", ChildContent)
                .Set("ColSpan", ColSpan)
                .AdditionalParameters(AdditionalParameters)
                .Render()
                .Invoke(builder);
        }
    }
}

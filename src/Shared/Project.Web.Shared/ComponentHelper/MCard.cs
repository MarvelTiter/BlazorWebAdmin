using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Page;

namespace Project.Web.Shared.ComponentHelper
{
    public class MCard : BasicComponent
    {
        [Parameter] public string? Title { get; set; }
        [Parameter] public RenderFragment? TitleTemplate { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            UI.BuildCard()
                .Set(c=>c.Title, Title)
                .Set(c => c.TitleTemplate, TitleTemplate)
                .Set(c=>c.ChildContent, ChildContent)
                .AdditionalParameters(AdditionalParameters)
                .Render()
                .Invoke(builder);
        }
    }

    public class MRow : BasicComponent
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            UI.BuildRow()
                .Set(c => c.ChildContent, ChildContent)
                .AdditionalParameters(AdditionalParameters)
                .Render()
                .Invoke(builder);
        }
    }

    public class MCol : BasicComponent
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public int ColSpan { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            UI.BuildCol()
                .Set(p => p.ChildContent, ChildContent)
                .Set(p => p.ColSpan, ColSpan)
                .AdditionalParameters(AdditionalParameters)
                .Render()
                .Invoke(builder);
        }
    }
}

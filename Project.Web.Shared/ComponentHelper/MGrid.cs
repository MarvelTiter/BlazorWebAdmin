using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.UI.Extensions;

namespace Project.Web.Shared.ComponentHelper
{
    public static class GridHelper
    {
        //public static GridColumn Repeat(int count, double percent)
        //{

        //}
    }

    public class GridColumn
    {
        public int Mode { get; set; }

    }

    public class MGrid : ComponentBase
    {
        [Parameter] public int GridColumns { get; set; } = 1;
        [Parameter] public int? MinWidth { get; set; }
        [Parameter] public string Gap { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        string ColumnTemplate()
        {
            if (MinWidth.HasValue)
            {
                return $"repeat(auto-fill, {MinWidth.Value}px)";
            }
            else
            {
                return $"repeat({GridColumns}, 1fr)";
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.Component<CascadingValue<MGrid>>()
                .SetComponent(c => c.Value, this)
                .SetComponent(c => c.ChildContent, b =>
                {
                    b.Div()
                    .Set("style", $"display:grid; grid-template-columns:{ColumnTemplate()}; gap:{Gap ?? "normal"}")
                    .AddContent(ChildContent)
                    .Build();
                })
                .Build();
        }
    }

    public class MGridContent : ComponentBase
    {
        [Parameter] public (int Start, int End) ColSpan { get; set; }
        [Parameter] public (int Start, int End) RowSpan { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var style = "";
            if (ColSpan.Start * ColSpan.End > 0)
                style += $"grid-column:{ColSpan.Start}/{ColSpan.End};";
            if (RowSpan.Start * RowSpan.End > 0)
                style += $"grid-row:{RowSpan.Start}/{RowSpan.End};";

            builder.Div()
                .Set("style", style)
                .AddContent(ChildContent)
                .Build();
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

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
            builder.OpenComponent<CascadingValue<MGrid>>(0);
            builder.AddAttribute(1, nameof(CascadingValue<MGrid>.Value), this);
            builder.AddAttribute(2, nameof(CascadingValue<MGrid>.ChildContent), (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "style", $"display:grid; grid-template-columns:{ColumnTemplate()}; gap:{Gap ?? "normal"}");
                builder.AddContent(2, ChildContent);
                builder.CloseElement();
            }));
            builder.CloseComponent();
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
            builder.OpenElement(0, "div");
            if (ColSpan.Start * ColSpan.End > 0)
                style += $"grid-column:{ColSpan.Start}/{ColSpan.End};";
            if (RowSpan.Start * RowSpan.End > 0)
                style += $"grid-row:{RowSpan.Start}/{RowSpan.End};";
            builder.AddAttribute(1, "style", style);
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
    }
}

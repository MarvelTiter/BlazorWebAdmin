using Microsoft.AspNetCore.Components;
using Project.Constraints.Page;

namespace Project.Web.Shared.Components
{
    public class ConditionBase : AppComponentBase, ICondition
    {
        [CascadingParameter, NotNull] public IQueryCondition? Parent { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public DateType? DateConfig { get; set; }
        [Parameter] public string? Style { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public (int Start, int End) ColSpan { get; set; }
        [Parameter] public (int Start, int End) RowSpan { get; set; }
        [Parameter] public int? LabelWidth { get; set; }
        [Parameter] public int? ContentWidth { get; set; }
        private int? FinalContentWidth => ContentWidth ?? Parent.ContentWidth;
        protected string ContentWidthStyle => FinalContentWidth.HasValue ? $"{FinalContentWidth}px" : "unset";
        protected string LabelWidthStyle => $"{LabelWidth ?? Parent.LabelWidth}px";
        protected string ColumnWidthStyle => Parent.ColumnWidth.HasValue ? $"{Parent.ColumnWidth}px" : "unset";
    }
}

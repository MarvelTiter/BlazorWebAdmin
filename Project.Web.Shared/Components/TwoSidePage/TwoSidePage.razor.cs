using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace BlazorWeb.Shared.Components
{
    public partial class TwoSidePage
    {
        [Parameter]
        [NotNull]
        public RenderFragment FirstSide { get; set; }
        [Parameter]
        public int MainFlex { get; set; } = 1;
        [Parameter]
        [NotNull]
        public RenderFragment SecondSide { get; set; }
        [Parameter]
        public RenderFragment ClosedView { get; set; }
        [Parameter]
        public Func<bool> OpenTrigger { get; set; }
        [Parameter]
        public string Direction { get; set; } = Row;
        [Parameter]
        public bool Expand { get; set; }
        [Parameter]
        public EventCallback<bool> ExpandChanged { get; set; }
        [Parameter]
        public bool ShowClose { get; set; } = true;

        public bool CanExpand => (OpenTrigger?.Invoke() ?? true) && Expand;

        public const string Row = "row";
        public const string RowReverse = "row-reverse";
        public const string Column = "column";
        public const string ColumnReverse = "column-reverse";

        async Task CloseSide()
        {
            Expand = false;
            if (ExpandChanged.HasDelegate)
                await ExpandChanged.InvokeAsync(Expand);
        }
    }
}

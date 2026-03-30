namespace Project.Web.Shared.Components;

public partial class SimpleTable<TData>
{
    [Parameter] public string TableClass { get; set; } = string.Empty;
    [Parameter] public string HeaderClass { get; set; } = string.Empty;
    [Parameter] public string RowClass { get; set; } = string.Empty;
    [Parameter] public string[] Headers { get; set; } = [];
    [Parameter] public IEnumerable<TData>? Items { get; set; }
    [Parameter] public RenderFragment? Empty { get; set; }
    [Parameter] public RenderFragment<TData>? RowTemplate { get; set; }
}

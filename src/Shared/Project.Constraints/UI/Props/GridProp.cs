using Microsoft.AspNetCore.Components;

namespace Project.Constraints.UI.Props;

public class GridProp
{
    public RenderFragment? ChildContent { get; set; }
    public int RowSpan { get; set; }
    public int ColSpan { get; set; }
}
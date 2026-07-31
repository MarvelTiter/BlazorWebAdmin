using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Constraints.UI.Props;

public class CardProp
{
    public string? Title { get; set; }
    public RenderFragment? TitleTemplate { get; set; }
    public RenderFragment? ChildContent { get; set; }
}
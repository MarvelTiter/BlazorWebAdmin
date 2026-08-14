namespace BlazorTemplate.ClientCore.Components;

public partial class EdgeWidget : JsComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    /// <summary>
    /// left / top /right /bottom
    /// </summary>
    [Parameter] public string Position { get; set; } = "left";

    ElementReference? maskDiv;
    ElementReference? containerDiv;
    ElementReference? triggerDiv;
    protected override async ValueTask Init()
    {
        await InvokeVoidAsync("init", new
        {
            mask = maskDiv,
            container = containerDiv,
            trigger = triggerDiv,
        });
    }
}
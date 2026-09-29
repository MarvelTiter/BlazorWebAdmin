using BlazorTemplate.ClientCore.UI.Extensions;
using BlazorTemplate.ClientCore.UI.Flyout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorTemplate.UI.FluentUI.Components;

public class FluentDialogWrap<TContent, TInput, TReturn> : FluentDialogInstance
    where TContent : IComponent
{
    [Parameter]
    public required FlyoutOptions<TContent, TInput, TReturn> Options { get; set; }

    protected override Task OnActionClickedAsync(bool primary)
    {
        return primary
            ? DialogInstance.CloseAsync()
            : DialogInstance.CancelAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FluentDialogBody>().SetContent(i =>
        {
            i.AddContent(0, Options.Content);
        }).Build();
    }
}
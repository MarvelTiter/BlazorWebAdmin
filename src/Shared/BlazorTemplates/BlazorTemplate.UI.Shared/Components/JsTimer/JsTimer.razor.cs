using Microsoft.JSInterop;

namespace BlazorTemplate.UI.Shared.Components;

public partial class JsTimer
{
    [Parameter, NotNull] public int? Interval { get; set; }
    [Parameter] public EventCallback Callback { get; set; }

    DotNetObjectReference<JsTimer>? objRef;
    protected override async ValueTask Init()
    {
        objRef = DotNetObjectReference.Create<JsTimer>(this);
        await InvokeVoidAsync("init", new
        {
            dotNetRef = objRef,
            interval = Interval,
        });
    }

    [JSInvokable("Call")]
    public Task Invoke()
    {
        if (!Callback.HasDelegate)
        {
            return Task.CompletedTask;
        }
        return Callback.InvokeAsync();
    }
}
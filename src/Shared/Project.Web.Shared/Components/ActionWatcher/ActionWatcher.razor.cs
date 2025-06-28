using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Threading;

namespace Project.Web.Shared.Components;

public partial class ActionWatcher : JsComponentBase
{
    public enum WatchType
    {
        None,
        Debounce,
        Throttle
    }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool WatchRoot { get; set; }
    [Parameter] public int Timeout { get; set; } = 1000 * 60 * 15;
    [Parameter] public EventCallback Callback { get; set; }
    [Parameter] public WatchType Type { get; set; }

    public DotNetObjectReference<ActionWatcher>? objRef;
    public ElementReference? container;
    protected override async ValueTask Init()
    {
        objRef = DotNetObjectReference.Create(this);
        await InvokeInit(new
        {
            instance = objRef,
            type = Type,
            timeout = Timeout,
            target = WatchRoot ? null : container
        });
    }
    [JSInvokable("Call")]
    public Task Invoke()
    {
        if (Callback.HasDelegate)
        {
            return Callback.InvokeAsync();
        }
        else
        {
            return Task.CompletedTask;
        }
    }

}
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Project.Web.Shared.Components
{
    
    public partial class ActionWatcher : JsComponentBase
    {
        public enum WatchType
        {
            None,
            Debounce,
            Throttle
        }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool WatchRoot { get; set; }
        [Parameter] public int Timeout { get; set; } = 1000 * 60 * 15;
        [Parameter] public EventCallback Callback { get; set; }
        [Parameter] public WatchType Type { get; set; }

        public DotNetObjectReference<ActionWatcher> objRef;
        public ElementReference container;
        protected override async ValueTask Init()
        {
            objRef = DotNetObjectReference.Create(this);
            if (WatchRoot)
            {
                await ModuleInvokeVoidAsync("init", objRef, Type, Timeout);
            }
            else
            {
                await ModuleInvokeVoidAsync("init", objRef, Type, Timeout, container);
            }
        }
        [Inject] ILogger<ActionWatcher> Logger { get; set; }
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
}

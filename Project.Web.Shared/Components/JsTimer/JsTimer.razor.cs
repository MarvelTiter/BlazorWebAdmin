using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace BlazorWeb.Shared.Components
{
    public partial class JsTimer
    {
        [Parameter, NotNull] public int? Interval { get; set; }
        [Parameter] public EventCallback Callback { get; set; }

        DotNetObjectReference<JsTimer>? objRef;
        protected override async ValueTask Init()
        {
            objRef = DotNetObjectReference.Create<JsTimer>(this);
            await ModuleInvokeVoidAsync("init", objRef, Interval);
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
}

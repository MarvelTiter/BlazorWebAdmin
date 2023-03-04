using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorWeb.Shared.Utils
{
    public static class JsRuntimeExtension
    {
        public static async Task OpenNewTab(this IJSRuntime runtime, string url)
        {
            await runtime.InvokeVoidAsync("open", url);
        }

        public static async Task DownloadFile(this IJSRuntime runtime, string filename, string extension)
        {
            await runtime.OpenNewTab($"/download/{filename}/{extension}");
        }

        public static ValueTask<T> ElementOperation<T>(this IJSRuntime runtime, ElementReference dom, string functionName, params object[] args)
        {
            return runtime.InvokeAsync<T>("elementOperation", dom, functionName, args);
            //if (args.Length > 0)
            //else
            //    return runtime.InvokeAsync<T>("elementOperation", dom, functionName);
        }

        public static ValueTask ElementOperation(this IJSRuntime runtime, ElementReference dom, string functionName, params object[] args)
        {
            return runtime.InvokeVoidAsync("elementOperation", dom, functionName, args);
        }

        public static ValueTask<T> ElementProperty<T>(this IJSRuntime runtime, ElementReference dom, string prop)
        {
            return runtime.InvokeAsync<T>("elementProperty", dom, prop);
        }
    }
}

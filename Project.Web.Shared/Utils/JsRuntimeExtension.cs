using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Project.Web.Shared.Utils
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
    }
}

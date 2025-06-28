using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Project.Web.Shared.Utils;

public static class JsRuntimeExtension
{
    public static async Task OpenNewTab(this IJSRuntime runtime, string url)
    {
        await runtime.InvokeVoidAsync("open", url);
    }

    public static async Task OpenWindowAsync(this IJSRuntime runtime, string url, double w = 0.7, double h = 0.7, string target = "_blank")
    {
        await runtime.InvokeUtilsAsync("openWindow", url, w, h, target);
    }

    public static async Task DownloadFile(this IJSRuntime runtime, string filename, string extension)
    {
        await runtime.OpenNewTab($"/download/{filename}/{extension}");
    }


    const string UTILS_FUNC_PREFIX = "window.Utils.";
    public static ValueTask InvokeUtilsAsync(this IJSRuntime runtime, string method, params object[] args)
    {
        return runtime.InvokeVoidAsync($"{UTILS_FUNC_PREFIX}{method}", args);
    }
    public static ValueTask<T> InvokeUtilsAsync<T>(this IJSRuntime runtime, string method, params object[] args)
    {
        return runtime.InvokeAsync<T>($"{UTILS_FUNC_PREFIX}{method}", args);
    }
}
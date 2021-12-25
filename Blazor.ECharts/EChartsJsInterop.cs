using Blazor.ECharts.Options;
using Microsoft.JSInterop;

namespace Blazor.ECharts
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class EChartsJsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public EChartsJsInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/Blazor.ECharts/core.js").AsTask());
        }

        public async ValueTask<string> Prompt(string message)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("showPrompt", message);
        }

        public async ValueTask renderChart(string id, string option)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("renderChart", id, option);
        }

        public async ValueTask renderChart<T>(string id, EChartOption<T> option) where T : SeriesBase
        {
            var module = await moduleTask.Value;
            await renderChart(id, option.ToString());
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
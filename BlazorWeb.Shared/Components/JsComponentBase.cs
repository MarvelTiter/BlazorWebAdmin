using BlazorWeb.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace BlazorWeb.Shared.Components
{
    public abstract class JsComponentBase : ComponentBase, IJsComponent, IAsyncDisposable
    {
        private string? id;
        [Inject] protected IJSRuntime Js { get; set; }
        protected IJSObjectReference? Module { get; set; }
        public string Id
        {
            get
            {
                if (id == null)
                    id = $"{GetType().Name}_{Guid.NewGuid():N}";
                return id;
            }
        }

        protected string ModuleName => GetType().Name;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                var path = $"./js/{ModuleName}/{ModuleName}.js".ToLower();
                Module = await Js.InvokeAsync<IJSObjectReference>("import", path);
                await Init();
            }
        }

        protected abstract ValueTask Init();

        protected ValueTask ModuleInvokeVoidAsync(string identifier, params object[] args)
        {
            var arguments = new List<object> { Id };
            arguments.AddRange(args ?? Array.Empty<object>());
            return Module!.InvokeVoidAsync($"{ModuleName}.{identifier}", arguments.ToArray());
        }

        protected ValueTask<T> ModuleInvokeAsync<T>(string identifier, params object[] args)
        {
            var arguments = new List<object> { Id };
            arguments.AddRange(args ?? Array.Empty<object>());
            return Module!.InvokeAsync<T>($"{ModuleName}.{identifier}", arguments.ToArray());
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (Module != null && disposing)
            {
                await Module.InvokeVoidAsync($"{ModuleName}.dispose", Id);
                await Module.DisposeAsync();
                Module = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }
    }
}

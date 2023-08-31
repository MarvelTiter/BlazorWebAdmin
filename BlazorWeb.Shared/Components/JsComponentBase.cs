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

        protected string ModuleName => GetModuleName();
        protected bool IsLibrary { get; set; } = true;
        private string GetModuleName()
        {
            var type = GetType();
            if (type.IsGenericType)
            {
                return type.Name.Split('`')[0];
            }
            return type.Name;
        }

        protected string ProjectName => GetType().Assembly.GetName().Name;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                //var path = 
                var path = IsLibrary
                    ? $"./_content/{ProjectName}/js/{ModuleName}/{ModuleName}.js".ToLower()
                    : $"./js/{ModuleName}/{ModuleName}.js".ToLower(); ;
                Module = await Js.InvokeAsync<IJSObjectReference>("import", path);
                await Init();
            }
        }

        protected abstract ValueTask Init();

        protected ValueTask ModuleInvokeVoidAsync(string identifier, params object?[]? args)
        {
            var arguments = new List<object> { Id };
            arguments.AddRange((args ?? Array.Empty<object>())!);
            return Module?.InvokeVoidAsync($"{ModuleName}.{identifier}", arguments.ToArray()) ?? ValueTask.FromCanceled(CancellationToken.None);
        }

        protected ValueTask<T> ModuleInvokeAsync<T>(string identifier, params object?[]? args)
        {
            var arguments = new List<object> { Id };
            arguments.AddRange((args ?? Array.Empty<object>())!);
            return Module?.InvokeAsync<T>($"{ModuleName}.{identifier}", arguments.ToArray()) ?? ValueTask.FromCanceled<T>(CancellationToken.None);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (Module != null && disposing)
            {
                //try
                //{
                //}
                //finally
                //{
                //}
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

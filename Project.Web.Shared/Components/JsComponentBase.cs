using Project.Web.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Project.Common.Attributes;
using System.Reflection;
using Project.Web.Shared.Basic;

namespace Project.Web.Shared.Components
{
    public abstract class JsComponentBase : BaseUIComponent, IJsComponent, IAsyncDisposable
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
                var i = type.Name.IndexOf('`');
                return type.Name[..i];
            }
            return type.Name;
        }

        protected string ProjectName => GetType().Assembly.GetName().Name;
        protected string RelativePath { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            var attr = GetType().GetCustomAttribute<AutoLoadJsModuleAttribute>();
            RelativePath = attr?.Path ?? $"Components/{ModuleName}";
            if (attr?.IsLibrary == false)
            {
                IsLibrary = false;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                //var path = 
                await LoadJsAsync();
                await Init();
            }
        }

        protected virtual async Task LoadJsAsync()
        {
            //var path = IsLibrary
            //    ? $"./_content/{ProjectName}/js/{ModuleName}/{ModuleName}.js".ToLower()
            //    : $"./js/{ModuleName}/{ModuleName}.js".ToLower();
            //Module = await Js.InvokeAsync<IJSObjectReference>("import", path);

            var path = IsLibrary
               ? $"./_content/{ProjectName}/{RelativePath}/{ModuleName}.razor.js"
               : $"./{RelativePath}/{ModuleName}.razor.js";
            Module = await Js.InvokeAsync<IJSObjectReference>("import", path);
        }

        protected virtual ValueTask Init()
        {
            return ValueTask.CompletedTask;
        }

        protected async ValueTask ModuleInvokeVoidAsync(string identifier, params object?[]? args)
        {
            try
            {
                //await (Module?.InvokeVoidAsync($"{ModuleName}.{identifier}", arguments.ToArray()) ?? ValueTask.FromCanceled(CancellationToken.None));
                await (Module?.InvokeVoidAsync(identifier, [Id, .. args]) ?? ValueTask.FromCanceled(CancellationToken.None));
            }
            catch { }
            finally
            {

            }
        }

        protected async ValueTask<T> ModuleInvokeAsync<T>(string identifier, params object?[]? args)
        {
            var ret = default(T);
            try
            {
                //ret = await (Module?.InvokeAsync<T>($"{ModuleName}.{identifier}", arguments.ToArray()) ?? ValueTask.FromCanceled<T>(CancellationToken.None));
                ret = await (Module?.InvokeAsync<T>(identifier, [Id, .. args]) ?? ValueTask.FromCanceled<T>(CancellationToken.None));
            }
            catch { }
            finally
            {

            }
            return ret;
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (Module != null && disposing)
            {
                // 忽略警告和报错
                try
                {
                    await Module.InvokeVoidAsync($"{ModuleName}.dispose", Id);
                    await Module.DisposeAsync();
                    Module = null;
                }
                catch { }
                finally
                {

                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }
    }
}

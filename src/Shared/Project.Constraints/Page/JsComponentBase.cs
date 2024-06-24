using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;
using Project.Constraints.UI;
using System.Diagnostics.CodeAnalysis;

namespace Project.Constraints.Page
{
    public abstract class JsComponentBase : BasicComponent, IJsComponent, IAsyncDisposable
    {
        private string? id;
        [Inject, NotNull] protected IJSRuntime? Js { get; set; }
        protected IJSObjectReference? Module { get; set; }
        public string Id
        {
            get
            {
                id ??= $"{GetType().Name}_{Guid.NewGuid():N}";
                return id;
            }
        }
        protected bool LoadJs { get; set; } = true;

        protected string ModuleName => GetModuleName();
        protected bool IsLibrary => GetType().Assembly.GetName().FullName != Assembly.GetEntryAssembly()?.GetName().FullName;
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

        protected string? ProjectName => GetType().Assembly.GetName().Name;
        protected string? RelativePath { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender && LoadJs)
            {
                var attr = GetType().GetCustomAttribute<AutoLoadJsModuleAttribute>();
                RelativePath = attr?.Path ?? $"Components/{ModuleName}";
                //var path = 
                await LoadJsAsync();
                await Init();
            }
        }

        protected virtual async Task LoadJsAsync()
        {
            var path = IsLibrary
               ? $"./_content/{ProjectName}/{RelativePath}/{ModuleName}.razor.js"
               : $"./{RelativePath}/{ModuleName}.razor.js";
            Module = await Js.InvokeAsync<IJSObjectReference>("import", AppConst.GetStatisticsFileWithVersion(path));
        }

        protected virtual ValueTask Init()
        {
            return ValueTask.CompletedTask;
        }

        protected async ValueTask InvokeInit(params object?[]? args)
        {
            try
            {
                //await (Module?.InvokeVoidAsync($"{ModuleName}.{identifier}", arguments.ToArray()) ?? ValueTask.FromCanceled(CancellationToken.None));
                await (Module?.InvokeVoidAsync("init", [Id, .. args]) ?? ValueTask.FromCanceled(CancellationToken.None));
            }
            catch { }
            finally
            {

            }
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
            return ret!;
        }

        protected async override ValueTask OnDisposeAsync()
        {
            if (Module != null)
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
    }
}

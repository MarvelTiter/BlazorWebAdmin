using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorTemplate.Constraints.Services;
using BlazorTemplate.Constraints.UI;

namespace BlazorTemplate.Constraints.Page;

public abstract class JsComponentBase : AppComponentBase, IJsComponent, IAsyncDisposable
{
    public const string JS_FUNC_PREFIX = "window.BlazorProject.";
    [Inject, NotNull] protected IJSRuntime? Js { get; set; }
    [Inject, NotNull] IFileService? FileService { get; set; }
    protected IJSObjectReference? Module { get; set; }
    protected bool LoadJs { get; set; } = false;

    public Lazy<string> Id => field ??= new(() => $"{GetType().Name}_{Guid.NewGuid():N}");
    protected Lazy<string> ModuleName => field ??= new Lazy<string>(RewriteModuleName);
    protected string GlobalModuleName => $"{JS_FUNC_PREFIX}{ModuleName.Value}";

    protected bool IsLibrary =>
        GetType().Assembly.GetName().FullName != Assembly.GetEntryAssembly()?.GetName().FullName;

    protected string? ProjectName => GetType().Assembly.GetName().Name;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (LoadJs)
            {
                await LoadJsAsync();
            }

            await Init();
        }
    }

    protected virtual string RewriteJsPath()
    {
        var attr = GetType().GetCustomAttribute<AutoLoadJsModuleAttribute>();
        var relativePath = attr?.Path ?? $"Components/{ModuleName.Value}";
        var fullJsPath = attr?.FullPath;
        if (fullJsPath is not { })
        {
            fullJsPath = IsLibrary
            ? $"{ProjectName}/{relativePath}/{ModuleName.Value}.razor.js"
            : $"{relativePath}/{ModuleName.Value}.razor.js";
        }
        var path = IsLibrary ? $"./_content/{fullJsPath}" : $"./{fullJsPath}";
        return path;
    }

    protected virtual string RewriteModuleName()
    {
        var type = GetType();
        if (type.IsGenericType)
        {
            var i = type.Name.IndexOf('`');
            return type.Name[..i];
        }
        return type.Name;
    }

    protected virtual async Task LoadJsAsync()
    {
        var path = RewriteJsPath();
        var versionPath = await FileService.GetStaticFileWithVersionAsync(path);
        Module = await Js.InvokeAsync<IJSObjectReference>("import", versionPath);
    }

    protected virtual ValueTask Init()
    {
        return ValueTask.CompletedTask;
    }

    protected async ValueTask InvokeInit(params object?[] args)
    {
        try
        {
            //await (Module?.InvokeVoidAsync($"{ModuleName}.{identifier}", arguments.ToArray()) ?? ValueTask.FromCanceled(CancellationToken.None));
            //await (Module?.InvokeVoidAsync("init", [Id, .. args]) ?? ValueTask.FromCanceled(CancellationToken.None));
            await Js.InvokeVoidAsync($"{GlobalModuleName}.init", [Id.Value, .. args]);
        }
        catch
        {
        }
    }

    protected async ValueTask InvokeVoidAsync(string identifier, params object?[] args)
    {
        try
        {
            //await (Module?.InvokeVoidAsync($"{ModuleName}.{identifier}", arguments.ToArray()) ?? ValueTask.FromCanceled(CancellationToken.None));
            //await (Module?.InvokeVoidAsync(identifier, [Id, .. args]) ?? ValueTask.FromCanceled(CancellationToken.None));
            await Js.InvokeVoidAsync($"{GlobalModuleName}.{identifier}", [Id.Value, .. args]);
        }
        catch
        {
        }
    }

    protected async ValueTask<T> InvokeAsync<T>(string identifier, params object?[] args)
    {
        var ret = default(T);
        try
        {
            //ret = await (Module?.InvokeAsync<T>($"{ModuleName}.{identifier}", arguments.ToArray()) ?? ValueTask.FromCanceled<T>(CancellationToken.None));
            //ret = await (Module?.InvokeAsync<T>(identifier, [Id, .. args]) ?? ValueTask.FromCanceled<T>(CancellationToken.None));
            ret = await Js.InvokeAsync<T>($"{GlobalModuleName}.{identifier}", [Id.Value, .. args]);
        }
        catch
        {
        }

        return ret!;
    }

    protected override async ValueTask OnDisposeAsync()
    {
        //if (Module != null)
        //{
        //    // 忽略警告和报错
        //}
        try
        {
            await DisposeModule();
            await DisposeOnGlobal();
        }
        catch
        {
        }

        async ValueTask DisposeModule()
        {
            if (Module != null)
            {
                await Module.InvokeVoidAsync("dispose", Id.Value);
                await Module.DisposeAsync();
            }
        }

        async ValueTask DisposeOnGlobal()
        {
            await Js.InvokeVoidAsync($"{GlobalModuleName}.dispose", Id.Value);
        }
    }
}
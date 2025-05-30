using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Project.Constraints.Services;
using Project.Constraints.UI;

namespace Project.Constraints.Page;

public abstract class JsComponentBase : AppComponentBase, IJsComponent, IAsyncDisposable
{
    public const string JS_FUNC_PREFIX = "window.BlazorProject.";
    [Inject, NotNull] protected IJSRuntime? Js { get; set; }
    [Inject, NotNull] IFileService? FileService { get; set; }
    protected IJSObjectReference? Module { get; set; }
    protected bool LoadJs { get; set; } = false;

    private Lazy<string>? idLazy;
    public Lazy<string> Id => idLazy ??= new(() => $"{GetType().Name}_{Guid.NewGuid():N}");

    private Lazy<string>? moduleName;
    protected Lazy<string> ModuleName => moduleName ??= new Lazy<string>(() =>
    {
        var type = GetType();
        if (type.IsGenericType)
        {
            var i = type.Name.IndexOf('`');
            return type.Name[..i];
        }

        return type.Name;
    });
    protected string GlobalModuleName => $"{JS_FUNC_PREFIX}{ModuleName.Value}";

    protected bool IsLibrary =>
        GetType().Assembly.GetName().FullName != Assembly.GetEntryAssembly()?.GetName().FullName;

    protected string? ProjectName => GetType().Assembly.GetName().Name;
    protected string? RelativePath { get; set; }



    //private Lazy<string> GetModuleName()
    //{
    //    return new Lazy<string>(() =>
    //    {
    //        var type = GetType();
    //        if (type.IsGenericType)
    //        {
    //            var i = type.Name.IndexOf('`');
    //            return type.Name[..i];
    //        }

    //        return type.Name;
    //    });
    //}

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (LoadJs)
            {
                var attr = GetType().GetCustomAttribute<AutoLoadJsModuleAttribute>();
                RelativePath = attr?.Path ?? $"Components/{ModuleName.Value}";
                //var path = 
                await LoadJsAsync();
            }

            await Init();
        }
    }

    protected virtual async Task LoadJsAsync()
    {
        var path = IsLibrary
            ? $"./_content/{ProjectName}/{RelativePath}/{ModuleName.Value}.razor.js"
            : $"./{RelativePath}/{ModuleName.Value}.razor.js";
        var versionPath = await FileService.GetStaticFileWithVersionAsync(path);
        Module = await Js.InvokeAsync<IJSObjectReference>("import", versionPath);
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
            //await (Module?.InvokeVoidAsync("init", [Id, .. args]) ?? ValueTask.FromCanceled(CancellationToken.None));
            await Js.InvokeVoidAsync($"{GlobalModuleName}.init", [Id.Value, .. args]);
        }
        catch
        {
        }
    }

    protected async ValueTask InvokeVoidAsync(string identifier, params object?[]? args)
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

    protected async ValueTask<T> InvokeAsync<T>(string identifier, params object?[]? args)
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
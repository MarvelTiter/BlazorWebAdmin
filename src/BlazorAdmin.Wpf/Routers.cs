using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Project.Web.Shared.Components;
using Project.Web.Shared.Layouts;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace BlazorAdmin.Wpf;

public class Routers : ComponentBase
{
    [Inject, NotNull] LoadingControl? LoadingControl { get; set; }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        LoadingControl.Update = StateHasChanged;
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Render the AppRoot component only when the header is loaded
        builder.OpenComponent<ErrorCatcher>(0);
        builder.AddAttribute(1, "ChildContent", Content);
        //builder.AddAttribute(2, nameof(ErrorCatcher.OnHandleExcetionAsync), ExceptionHandleAsync);
        builder.CloseComponent();
        //if (LoadingControl.HeaderLoaded)
        //{
        //    builder.OpenComponent<AppRoot>(0);
        //    builder.AddAttribute(1, nameof(AppRoot.DefaultLayout), typeof(MainLayout));
        //    builder.CloseComponent();
        //}
    }
    private bool isOccurJsException;
    private Timer? retryTimer;
    private RenderFragment Content => builder =>
    {
        if (LoadingControl.HeaderLoaded && !isOccurJsException)
        {
            builder.OpenComponent<AppRoot>(0);
            builder.AddAttribute(1, nameof(AppRoot.DefaultLayout), typeof(MainLayout));
            builder.CloseComponent();
        }
    };

    private async Task<bool> ExceptionHandleAsync(Exception ex)
    {
        if (ex is JSException jsE && jsE.Message.Contains("Could not find"))
        {
            Debug.WriteLine("加载js未完成");
            isOccurJsException = true;
            await InvokeAsync(StateHasChanged);
            retryTimer ??= new Timer(_ =>
            {
                Debug.WriteLine("重试");
                isOccurJsException = false;
                retryTimer?.Dispose();
                retryTimer = null;
                _ = InvokeAsync(StateHasChanged);
            }, null, 200, Timeout.Infinite);
            return true;
        }
        return false;
    }
}
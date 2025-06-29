using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Project.Constraints.UI;

#pragma warning disable IDE0130
namespace Project.Web.Shared.Components;

public class ErrorCatcher : ErrorBoundaryBase//, IExceptionHandler
{
    /// <summary>
    /// 
    /// </summary>
    [Inject]
    [NotNull]
    private ILogger<ErrorCatcher>? Logger { get; set; }

    [Parameter] public bool ShowMessage { get; set; }
    [Inject, NotNull] IRouterStore? Router { get; set; }


    public event Func<Exception, Task>? OnHandleExcetionAsync;
    public event Action<Exception>? OnHandleExcetion;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ErrorContent ??= RenderException();
    }
    //CrashPage? crashPage;
    private static RenderFragment<Exception> RenderException()
    {
        return ex => new RenderFragment(builder =>
        {
            builder.OpenComponent<CrashPage>(0);
            builder.AddAttribute(1, nameof(CrashPage.Exception), ex);
            //builder.AddComponentReferenceCapture(2, obj =>
            //{
            //    crashPage = obj as CrashPage;
            //    StateHasChanged();
            //});
            builder.CloseComponent();
        });
    }
    //int errorTimes = 0;
    //const int MAX_ERRORTIMES = 3;
    //const int ERROR_OCCUR_INTERVAL = 1000;
    //Stopwatch sw = new Stopwatch();
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (CurrentException != null)
        {
            if (Router.Current == null) return;
            Router.Current.Panic = true;
            Router.Current.Exception = CurrentException;
            Console.WriteLine(CurrentException.StackTrace);
            // 如果是生命周期内发生的异常，不应该Recover，反之需要Recover
            // Recover();
            builder.AddContent(0, ErrorContent!.Invoke(CurrentException));
        }
        else
        {
            builder.AddContent(0, ChildContent);
        }
    }

    /// <summary>
    /// OnParametersSet 方法
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        // Recover();
    }
    //bool rendered;
    //protected override void OnAfterRender(bool firstRender)
    //{
    //    base.OnAfterRender(firstRender);
    //    if (firstRender)
    //    {
    //        var ex = CurrentException;
    //        rendered = true;
    //    }
    //}

    [Inject, NotNull] public IUIService? UI { get; set; }
    /// <summary>
    /// OnErrorAsync 方法
    /// </summary>
    /// <param name="exception"></param>
    protected override Task OnErrorAsync(Exception exception)
    {
        if (exception is not JSException && ShowMessage)
        {
            UI.Notify(MessageType.Error, "程序异常", exception.Message);
            Logger.LogError(exception, "{Message}", exception.Message);
        }
        OnHandleExcetion?.Invoke(exception);
        if (OnHandleExcetionAsync != null)
        {
            return OnHandleExcetionAsync(exception);
        }
        return Task.CompletedTask;
    }

    //public Task HandleExceptionAsync(Exception exception)
    //{
    //    return OnErrorAsync(exception);
    //}
}
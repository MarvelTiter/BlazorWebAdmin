using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Project.Constraints.UI;
using System.Diagnostics.CodeAnalysis;

namespace Project.Web.Shared.Components
{
	public class ErrorCatcher : ErrorBoundaryBase, IExceptionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        [Inject]
        [NotNull]
        private ILogger<ErrorCatcher>? Logger { get; set; }


        [Inject]
        [NotNull]
        private IErrorBoundaryLogger? ErrorBoundaryLogger { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected Exception? Exception { get; set; }

        public event Func<Exception, Task> OnHandleExcetionAsync;
        public event Action<Exception> OnHandleExcetion;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ErrorContent ??= RenderException();
        }

        private RenderFragment<Exception> RenderException()
        {
            return ex => new RenderFragment(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "style", "display:none;");
                builder.AddContent(2, ex.Message);
                builder.CloseElement();
            });
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<IExceptionHandler>>(0);
            builder.AddAttribute(1, nameof(CascadingValue<IExceptionHandler>.Value), this);
            builder.AddAttribute(2, nameof(CascadingValue<IExceptionHandler>.IsFixed), true);

            //if (CurrentException != null)
            //{
            //    if (Caches.Any())
            //    {
            //        var handler = Caches.Last();
            //        handler?.HandleExceptionAsync(CurrentException);
            //    }
            //}

            builder.AddAttribute(3, nameof(CascadingValue<IExceptionHandler>.ChildContent), ChildContent);
            builder.CloseComponent();
        }

        /// <summary>
        /// OnParametersSet 方法
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Exception = null;
            Recover();
        }

        [Inject]
        public IUIService UI { get; set; }
        /// <summary>
        /// OnErrorAsync 方法
        /// </summary>
        /// <param name="exception"></param>
        protected override async Task OnErrorAsync(Exception exception)
        {
            //OnHandleExcetion?.Invoke(exception);
            //if (OnHandleExcetionAsync != null)
            //{
            //    await OnHandleExcetionAsync.Invoke(exception);
            //}

            //var handler = Caches.Last();
            //if (handler != null)
            //    await handler.HandleExceptionAsync(exception);
            if (exception is not JSException)
            {
                UI.Notify(MessageType.Error, "程序异常", exception.Message);
            }
            Logger.LogError(exception, exception.Message);
            await ErrorBoundaryLogger.LogErrorAsync(exception);
        }

        public Task HandleExceptionAsync(Exception exception)
        {
            return OnErrorAsync(exception);
        }
    }
}

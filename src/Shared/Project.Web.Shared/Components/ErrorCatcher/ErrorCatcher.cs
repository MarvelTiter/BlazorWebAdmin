using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Project.Constraints.Store;
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

        [Parameter] public bool ShowMessage { get; set; }
        [Inject, NotNull] IRouterStore? Router { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected Exception? Exception { get; set; }

        public event Func<Exception, Task>? OnHandleExcetionAsync;
        public event Action<Exception>? OnHandleExcetion;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ErrorContent ??= RenderException();
        }

        private RenderFragment<Exception> RenderException()
        {
            return ex => new RenderFragment(builder =>
            {
                builder.OpenComponent<CrashPage>(0);
                builder.AddAttribute(1, nameof(CrashPage.Exception), ex);
                builder.CloseComponent();
            });
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<IExceptionHandler>>(0);
            builder.AddAttribute(1, nameof(CascadingValue<IExceptionHandler>.Value), this);
            builder.AddAttribute(2, nameof(CascadingValue<IExceptionHandler>.IsFixed), true);
            var content = ChildContent;
            if (CurrentException != null)
            {
                if (Router.Current != null)
                {
                    //不保存状态
                    Router.Current.Cache = false;
                }
                content = ErrorContent!.Invoke(CurrentException);
            }
            builder.AddAttribute(3, nameof(CascadingValue<IExceptionHandler>.ChildContent), content);
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
                Logger.LogError(exception, exception.Message);
            }
            OnHandleExcetion?.Invoke(exception);
            if (OnHandleExcetionAsync != null)
            {
                return OnHandleExcetionAsync(exception);
            }
            return Task.CompletedTask;
        }

        public Task HandleExceptionAsync(Exception exception)
        {
            return OnErrorAsync(exception);
        }
    }
}

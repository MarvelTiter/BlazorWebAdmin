using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace BlazorWeb.Shared.Components
{
    public class ErrorCatcher : ErrorBoundaryBase, IError
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
                var index = 0;
                builder.OpenElement(index++, "div");
                builder.AddAttribute(index++, "style", "display:none;");
                builder.AddContent(index++, ex.Message);
                builder.CloseElement();
            });
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var index = 0;
            builder.OpenComponent<CascadingValue<IError>>(index++);
            builder.AddAttribute(index++, nameof(CascadingValue<IError>.Value), this);
            builder.AddAttribute(index++, nameof(CascadingValue<IError>.IsFixed), true);

            //if (CurrentException != null)
            //{
            //    if (Caches.Any())
            //    {
            //        var handler = Caches.Last();
            //        handler?.HandleExceptionAsync(CurrentException);
            //    }
            //}

            builder.AddAttribute(index++, nameof(CascadingValue<IError>.ChildContent), ChildContent);
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
        public MessageService MsgSrv { get; set; }
        [Inject]
        public NotificationService NotificationSrv { get; set; }
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
                _ = NotificationSrv.Error(new NotificationConfig()
                {
                    Message = "程序异常",
                    Description = exception.Message,
                    Placement = NotificationPlacement.BottomRight,
                });
            }
            Logger.LogError(exception, exception.Message);
            await ErrorBoundaryLogger.LogErrorAsync(exception);
        }

        readonly List<IExceptionHandler> Caches = new();

        public void Register<TComponent>(TComponent component) where TComponent : IComponent, IExceptionHandler
        {
            Caches.Add(component);
        }

        public void UnRegister<TComponent>(TComponent component) where TComponent : IComponent, IExceptionHandler
        {
            Caches.Add(component);
        }
    }
}

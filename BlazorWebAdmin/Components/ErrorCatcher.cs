using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;

namespace BlazorWebAdmin.Components
{
    public class ErrorCatcher : ErrorBoundaryBase
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

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (CurrentException == null)
            {
                builder.AddContent(0, ChildContent);
                return;
            }

            if (ErrorContent != null)
            {
                builder.AddContent(1, ErrorContent!(CurrentException));
                return;
            }

            //builder.OpenElement(2, "div");
            //builder.AddAttribute(3, "class", "blazor-error-boundary");
            //builder.CloseElement();
        }

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        //protected override void OnInitialized()
        //{
        //    base.OnInitialized();
        //}

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

        /// <summary>
        /// OnErrorAsync 方法
        /// </summary>
        /// <param name="exception"></param>
        protected override async Task OnErrorAsync(Exception exception)
        {
            await MsgSrv.Error(exception.Message);
            await ErrorBoundaryLogger.LogErrorAsync(exception);
        }
    }
}

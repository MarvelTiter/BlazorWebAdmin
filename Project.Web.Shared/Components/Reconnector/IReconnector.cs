
using Microsoft.AspNetCore.Components;

namespace Project.Web.Shared.Components
{
    public interface IReconnectorProvider
    {
        void UpdateTemplate(IReconnector reconnector);
        void RegisterView(Action<IReconnector> action);
    }

    public class ReconnectorProvider : IReconnectorProvider
    {
        Action<IReconnector>? action;
        public void RegisterView(Action<IReconnector> action)
        {
            this.action = action;
        }

        public void UpdateTemplate(IReconnector reconnector)
        {
            action?.Invoke(reconnector);
        }
    }

    public interface IReconnector
    {
        /// <summary>
        /// 正在重连的模板
        /// </summary>
        RenderFragment? ReconnectingTemplate { get; set; }

        /// <summary>
        /// 重连失败的模板
        /// </summary>
        RenderFragment? ReconnectFailedTemplate { get; set; }

        /// <summary>
        /// 服务器拒绝的模板
        /// </summary>
        RenderFragment? ReconnectRejectedTemplate { get; set; }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorWeb.Shared.Components
{
    /// <summary>
    /// 修改重新连接处理程序 (Blazor Server)
    /// https://learn.microsoft.com/zh-cn/aspnet/core/blazor/fundamentals/signalr?view=aspnetcore-7.0#modify-the-reconnection-handler-blazor-server
    /// </summary>
    public class ReconnectorOutlet : ComponentBase
    {
        /// <summary>
        /// 是否自动重连 (默认 true)
        /// </summary>
        [Parameter]
        public bool AutoReconnect { get; set; } = true;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<ReconnectorContent>(0);
            builder.AddAttribute(1, nameof(ReconnectorContent.AutoReconnect), AutoReconnect);
            builder.CloseComponent();
        }
    }
}

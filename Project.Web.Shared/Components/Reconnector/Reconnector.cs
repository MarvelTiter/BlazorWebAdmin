﻿using Microsoft.AspNetCore.Components;

namespace BlazorWeb.Shared.Components
{
    public class Reconnector : ComponentBase, IReconnector
    {
        /// <summary>
        /// 正在重连的模板
        /// </summary>
        [Parameter]
        public RenderFragment? ReconnectingTemplate { get; set; }

        /// <summary>
        /// 重连失败的模板
        /// </summary>
        [Parameter]
        public RenderFragment? ReconnectFailedTemplate { get; set; }

        /// <summary>
        /// 服务器拒绝的模板
        /// </summary>
        [Parameter]
        public RenderFragment? ReconnectRejectedTemplate { get; set; }
        [Inject] public IReconnectorProvider Provider { get; set; }
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            Provider.UpdateTemplate(this);
        }
    }
}

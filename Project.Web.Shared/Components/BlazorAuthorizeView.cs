using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorWeb.Shared.Components
{
    public class BlazorAuthorizeView : ComponentBase
    {
        /// <summary>
        /// 获得/设置 路由关联上下文
        /// </summary>
        [Parameter]
        [NotNull]
        public Type? Type { get; set; }

        /// <summary>
        /// 获得/设置 路由关联上下文
        /// </summary>
        [Parameter]
        public IReadOnlyDictionary<string, object>? Parameters { get; set; }

        /// <summary>
        /// 获得/设置 NotAuthorized 模板
        /// </summary>
        [Parameter]
        public RenderFragment? NotAuthorized { get; set; }

        /// <summary>
        /// The resource to which access is being controlled.
        /// </summary>
        [Parameter]
        public object? Resource { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        [Inject]
        private IAuthorizationPolicyProvider? AuthorizationPolicyProvider { get; set; }

        [Inject]
        private IAuthorizationService? AuthorizationService { get; set; }

#if NET6_0_OR_GREATER
        [Inject]
        [NotNull]
        private NavigationManager? NavigationManager { get; set; }
#endif

        private bool Authorized { get; set; }

        /// <summary>
        /// OnInitializedAsync 方法
        /// </summary>
        /// <returns></returns>
        protected override Task OnInitializedAsync()
        {
            //Authorized = Type == null
            //    || await Type.IsAuthorizedAsync(AuthenticationState, AuthorizationPolicyProvider, AuthorizationService, Resource);
            Authorized = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// BuildRenderTree 方法
        /// </summary>
        /// <param name="builder"></param>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            // 判断是否开启权限
            if (Authorized && Type != null)
            {
                var index = 0;
                builder.OpenComponent(index++, Type);
                foreach (var kv in Parameters ?? new ReadOnlyDictionary<string, object>(new Dictionary<string, object>()))
                {
                    builder.AddAttribute(index++, kv.Key, kv.Value);
                }
                //BuildQueryParameters();
                builder.CloseComponent();
            }
            else
            {
                builder.AddContent(0, NotAuthorized);
            }

            //void BuildQueryParameters()
            //{
            //    var queryParameterSupplier = QueryParameterValueSupplier.ForType(Type);
            //    if (queryParameterSupplier is not null)
            //    {
            //        // Since this component does accept some parameters from query, we must supply values for all of them,
            //        // even if the querystring in the URI is empty. So don't skip the following logic.
            //        var url = NavigationManager.Uri;
            //        var queryStartPos = url.IndexOf('?');
            //        var query = queryStartPos < 0 ? default : url.AsMemory(queryStartPos);
            //        queryParameterSupplier.RenderParametersFromQueryString(builder, query);
            //    }
            //}
        }
    }
}

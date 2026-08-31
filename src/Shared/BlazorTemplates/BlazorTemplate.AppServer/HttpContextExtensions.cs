#if NET8_0
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.AspNetCore.Components.Routing;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class HttpContextExtensions
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, bool> AcceptsInteractiveRoutingCache = new();
    public static bool AcceptsInteractiveRouting(this HttpContext context)
    {
        //httpContext.GetEndpoint().
        ArgumentNullException.ThrowIfNull(context);

        var pageType = EndpointHttpContextExtensions.GetEndpoint(context)?.Metadata.GetMetadata<ComponentTypeMetadata>()?.Type;

        return pageType is not null
            && AcceptsInteractiveRoutingCache.GetOrAdd(
                pageType,
                static pageType => !pageType.IsDefined(typeof(ExcludeFromInteractiveRoutingAttribute), true));
    }
}
#endif

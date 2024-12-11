#if !NET9_0
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Http;
using Project.Constraints.Common.Attributes;
using System.Collections.Concurrent;

namespace Project.AppCore;

public static class HttpContextExtensions
{
    private static readonly ConcurrentDictionary<Type, bool> AcceptsInteractiveRoutingCache = new();
    public static bool AcceptsInteractiveRouting(this HttpContext context)
    {
        //httpContext.GetEndpoint().
        ArgumentNullException.ThrowIfNull(context);

        var pageType = context.GetEndpoint()?.Metadata.GetMetadata<ComponentTypeMetadata>()?.Type;

        return pageType is not null
            && AcceptsInteractiveRoutingCache.GetOrAdd(
                pageType,
                static pageType => !pageType.IsDefined(typeof(ExcludeFromInteractiveRoutingAttribute), true));
    }
}
#endif

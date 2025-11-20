using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Http;
using Project.Constraints.Common.Attributes;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Project.AppCore;


//public static class HttpContextExtensions
//{
//    private static readonly ConcurrentDictionary<Type, bool> AcceptsInteractiveRoutingCache = new();
//    public static bool AcceptsInteractiveRouting(this HttpContext context)
//    {
//        //httpContext.GetEndpoint().
//        ArgumentNullException.ThrowIfNull(context);

//        var pageType = context.GetEndpoint()?.Metadata.GetMetadata<ComponentTypeMetadata>()?.Type;

//        return pageType is not null
//            && AcceptsInteractiveRoutingCache.GetOrAdd(
//                pageType,
//                static pageType => !pageType.IsDefined(typeof(ExcludeFromInteractiveRoutingAttribute), true));
//    }
//}


public static class ClaimsPrincipalExtensions
{
    public static bool GetCookieClaimsIdentity(this ClaimsPrincipal user, [NotNullWhen(true)] out ClaimsIdentity? identity)
    {
        foreach (var item in user.Identities)
        {
            if (item.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme)
            {
                identity = item;
                return true;
            }
        }
        identity = null;
        return false;
    }
}

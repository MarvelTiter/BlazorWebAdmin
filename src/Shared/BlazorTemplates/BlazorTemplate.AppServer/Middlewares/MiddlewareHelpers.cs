using Microsoft.AspNetCore.Http;

namespace BlazorTemplate.AppServer.Middlewares;

internal static class MiddlewareHelpers
{

    internal static bool RequestFile(HttpContext context)
    {
        var path = context.Request.Path;
        var ext = Path.GetExtension(path);
        return !string.IsNullOrEmpty(ext);
    }
}
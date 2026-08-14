using AutoInjectGenerator;
using Microsoft.AspNetCore.Http;

namespace BlazorTemplate.AppServer.Middlewares;

[AutoInjectSelf(LifeTime = InjectLifeTime.Singleton)]
public class NotFoundRedirect : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        if (context.Response.HasStarted
            || context.Response.StatusCode < 400
            || context.Response.StatusCode >= 600
            || context.Response.ContentLength.HasValue
            || !string.IsNullOrEmpty(context.Response.ContentType))
        {
            return;
        }
        if (context.Response.StatusCode == 404)
        {
            context.Response.Redirect("/notfound");
        }
    }
}

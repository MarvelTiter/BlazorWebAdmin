using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Project.Constraints.Common;
using Project.Constraints.Options;

namespace Project.AppCore.Middlewares
{
    public class CheckBrowserEnabledMiddleware : IMiddleware
    {
        private readonly IOptionsMonitor<AppSetting> options;

        public CheckBrowserEnabledMiddleware(IOptionsMonitor<AppSetting> options)
        {
            this.options = options;
        }
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path == "/unsupport")
            {
                return next(context);
            }
            if (MiddlewareHelpers.RequestFile(context))
            {
                return next(context);
            }
            if (context.Request.Path.StartsWithSegments("/_blazor") || context.Request.Path.StartsWithSegments("/_framework"))
            {
                return next(context);
            }
            //if (context.Request.Cookies["HadCheckedBrowser"] == "1")
            //{
            //    return next(context);
            //}
            //if (context.Request.Cookies["HadCheckedBrowser"] == "0")
            //{
            //    context.Response.Redirect("/unsupport");
            //    return Task.CompletedTask;
            //}
            var agent = context.Request.Headers.UserAgent;
            if (string.IsNullOrEmpty(agent))
            {
                return next(context);
            }
            var info = UserAgentHelper.GetBrowser(agent!);
            if (info.IsSupport(options.CurrentValue.SupportedMajorVersion))
            {
                //context.Response.Cookies.Append("HadCheckedBrowser", "1");
                return next(context);
            }
            else
            {
                //context.Response.Cookies.Append("HadCheckedBrowser", "0");
                context.Response.Redirect("/unsupport");
            }

            return Task.CompletedTask;
        }
    }
}

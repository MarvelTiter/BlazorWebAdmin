using Project.Constraints.Common;

namespace Project.AppCore.Middlewares
{
    public class CheckBrowserEnabledMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Cookies["HadCheckedBrowser"] == "1")
            {
                return next(context);
            }

            var agent = context.Request.Headers.UserAgent;
            if (string.IsNullOrEmpty(agent))
            {
                return next(context);
            }
            var info = UserAgentHelper.GetBrowser(agent!);
            if (info.IsSupport())
            {
                return next(context);
            }
            context.Response.Cookies.Append("HadCheckedBrowser", "1");
            context.Response.Redirect("/unsupport");

            return Task.CompletedTask;
        }
    }
}

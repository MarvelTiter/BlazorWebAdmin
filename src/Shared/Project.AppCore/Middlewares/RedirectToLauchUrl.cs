using AutoInjectGenerator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Project.Constraints.Options;

namespace Project.AppCore.Middlewares
{
    [AutoInject(ServiceType = typeof(RedirectToLauchUrlMiddleware), LifeTime = InjectLifeTime.Singleton)]
    public class RedirectToLauchUrlMiddleware : IMiddleware
    {
        private readonly IOptionsMonitor<AppSetting> setting;

        public RedirectToLauchUrlMiddleware(IOptionsMonitor<AppSetting> setting)
        {
            this.setting = setting;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var redirect = setting.CurrentValue.LauchUrl;
            if (!string.IsNullOrEmpty(redirect) && redirect != "/")
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect(redirect);
                    return Task.CompletedTask;
                }
            }
            return next(context);
        }
    }
}

using AutoInjectGenerator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Project.AppCore.Auth;
using Project.Constraints.Options;
using Project.Constraints.Store;

namespace Project.AppCore.Middlewares;

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

//[AutoInject(ServiceType = typeof(SetUserInfoMiddleware))]
//public class SetUserInfoMiddleware : IMiddleware
//{
//    private readonly IUserStore userStore;

//    public SetUserInfoMiddleware(IUserStore userStore)
//    {
//        this.userStore = userStore;
//    }
//    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//    {
//        if (context.User.Identity?.IsAuthenticated == true)
//        {
//            var u = context.User.GetUserInfo();
//            await userStore.SetUserAsync(u);
//        }
//        await next(context);
//    }
//}
using AutoInjectGenerator;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Project.AppCore.Auth;
using Project.Constraints.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Constraints.Utils;

namespace Project.AppCore.Middlewares;

[AutoInject(ServiceType = typeof(SetUserInfoMiddleware))]
public class SetUserInfoMiddleware : IMiddleware
{
    private readonly IUserStore userStore;

    public SetUserInfoMiddleware(IUserStore userStore)
    {
        this.userStore = userStore;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        //if (context.User.Identity?.IsAuthenticated == true)
        //{
        //    foreach (var identity in context.User.Identities)
        //    {
        //        if (identity.AuthenticationType != CookieAuthenticationDefaults.AuthenticationScheme)
        //            continue;
        //        var u = context.User.GetUserInfo();
        //        userStore.SetUser(u);
        //    }
        //}
        if (context.User.GetCookieClaimsIdentity(out var identity) && identity!.IsAuthenticated == true)
        {
            var u = identity.GetUserInfo();
            userStore.SetUser(u);
        }
        await next(context);
    }
}

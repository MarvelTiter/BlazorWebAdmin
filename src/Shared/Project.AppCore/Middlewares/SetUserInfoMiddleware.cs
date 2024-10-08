using AutoInjectGenerator;
using Microsoft.AspNetCore.Http;
using Project.AppCore.Auth;
using Project.Constraints.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        var u = context.User.GetUserInfo();
        userStore.SetUser(u);
        await next(context);
    }
}

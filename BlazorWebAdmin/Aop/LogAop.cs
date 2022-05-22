using AspectCore.DynamicProxy;
using BlazorWebAdmin.Store;
using Microsoft.AspNetCore.Components.Authorization;
using Project.Common.Attributes;
using Project.Models;
using Project.Models.Entities;
using Project.Services.interfaces;
using System.Reflection;
namespace BlazorWebAdmin.Aop
{
    public class LogAop : AbstractInterceptor
    {
        private readonly IRunLogService logService;
        private readonly UserStore store;
        private readonly IHttpContextAccessor contextAccessor;

        public LogAop(IRunLogService logService, UserStore store, IHttpContextAccessor contextAccessor)
        {
            this.logService = logService;
            this.store = store;
            this.contextAccessor = contextAccessor;
            Console.WriteLine($"LogAop store {store.GetHashCode()}");
        }
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            await next(context);
            await context.Complete();
            Console.WriteLine($"LogAop contextAccessor {contextAccessor.HttpContext?.User.Identity?.Name}");
            return;
            bool isAsync = context.IsAsync();
            var infoAttr = context.ServiceMethod.GetCustomAttribute<LogInfoAttribute>();
            if (infoAttr == null) return;
            object ret;
            if (isAsync)
            {
                ret = await context.UnwrapAsyncReturnValue();
                var result = ret as IQueryResult;
                var userId = GetUserId(context) ?? "获取失败";
                var l = new RunLog()
                {
                    UserId = userId,
                    ActionModule = infoAttr!.Module ?? "",
                    ActionName = infoAttr!.Action ?? "",
                    ActionResult = result?.Success ?? false ? "成功" : "失败",
                    ActionMessage = result?.Message ?? "",
                };
                await logService.Log(l);
            }
            //var msg = $"{result?.Result.Success} {result?.Result.Message} {infoAttr?.Module} {infoAttr?.Action}";
        }

        string? GetUserId(AspectContext context)
        {
            if (context.ServiceMethod.Name == "LoginAsync")
            {
                return context.Parameters.FirstOrDefault()?.ToString();
            }
            return store.UserId;
        }
    }
}

using AspectCore.DynamicProxy;
using Project.Constraints.Models;
using Project.Constraints.Services;
using Project.Constraints.Store;
using Project.Models.Entities;
using System.Reflection;

namespace Project.Constraints.Aop;

public class LogAopAttribute : AbstractInterceptorAttribute
{
    public override async Task Invoke(AspectContext context, AspectDelegate next)
    {
        await next(context);
        var infoAttr = context.ServiceMethod.GetCustomAttribute<LogInfoAttribute>();
        if (infoAttr == null)
        {
            return;
        }
        var result = await GetReturnValue<IQueryResult>(context);
        var store = context.ServiceProvider.GetService<IUserStore>();
        var logService = context.ServiceProvider.GetService<IRunLogService>();
        var userId = store?.UserId ?? GetUserIdFromContext(context);
        if (infoAttr!.Module == "BasicService")
        {
            var type = context.ServiceMethod.DeclaringType!.GetGenericArguments().First();
            infoAttr!.Action = $"[{type.Name}]{infoAttr!.Action}";
        }
        var l = new RunLog()
        {
            UserId = userId,
            ActionModule = infoAttr!.Module ?? "",
            ActionName = infoAttr!.Action ?? "",
            ActionResult = result?.Success ?? false ? "成功" : "失败",
            ActionMessage = result?.Message ?? "",
        };
        await logService!.Log(l);
    }

    private static async Task<T> GetReturnValue<T>(AspectContext context)
    {
        if (context.IsAsync())
        {
            return await context.UnwrapAsyncReturnValue<T>();
        }
        else
        {
            return (T)context.ReturnValue;
        }
    }

    private static string GetUserIdFromContext(AspectContext context)
    {
        return (context.Parameters.FirstOrDefault() as UserInfo)?.UserId ?? context.Parameters.FirstOrDefault()?.ToString();
    }
}


//public class LogAop : Interceptor
//{
//    private readonly IRunLogService logService;
//    private readonly UserStore store;

//    public LogAop(IRunLogService logService, UserStore store)
//    {
//        this.logService = logService;
//        this.store = store;
//    }

//    public override async Task Invoke(LogAopCodeGenerator.AspectContext context)
//    {
//        await context.Proceed();
//        var infoAttr = context.ServiceMethod.GetCustomAttribute<LogInfoAttribute>();
//        var result = context.ReturnValue as IQueryResult;
//        var userId = store?.UserId ?? GetUserIdFromContext(context);
//        if (infoAttr!.Module == "BasicService")
//        {
//            var type = context.ServiceType.GetGenericArguments().First();
//            infoAttr!.Action = $"[{type.Name}]{infoAttr!.Action}";
//        }
//        var l = new RunLog()
//        {
//            UserId = userId,
//            ActionModule = infoAttr!.Module ?? "",
//            ActionName = infoAttr!.Action ?? "",
//            ActionResult = result?.Success ?? false ? "成功" : "失败",
//            ActionMessage = result?.Message ?? "",
//        };
//        await logService.Log(l);
//    }

//    private string GetUserIdFromContext(LogAopCodeGenerator.AspectContext context)
//    {
//        if (context.ServiceType == typeof(ILoginService))
//        {
//            // UpdateLastLoginTimeAsync 和 LoginAsync
//            return (context.Parameters.FirstOrDefault() as UserInfo)?.UserId ?? context.Parameters.FirstOrDefault()?.ToString();
//        }
//        return "Unknow";
//    }

//}

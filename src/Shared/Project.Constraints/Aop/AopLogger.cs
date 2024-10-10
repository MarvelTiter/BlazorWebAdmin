using Project.Constraints.Models.Permissions;
using Project.Constraints.Services;
using Project.Constraints.Store;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using AutoAopProxyGenerator;
using AutoInjectGenerator;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Extensions.Logging;

namespace Project.Constraints.Aop;

[AutoInject(ServiceType = typeof(AopLogger))]
public class AopLogger : IAspectHandler
{
    private readonly IUserStore userStore;
    private readonly ILogger<AopLogger> logger;

    public AopLogger(IUserStore userStore, ILogger<AopLogger> logger)
    {
        this.userStore = userStore;
        this.logger = logger;
    }
    public async Task Invoke(ProxyContext context, Func<Task> process)
    {
        logger.LogInformation("AopLogger called before {Name}", context.ServiceMethod?.Name);
        await process();
        logger.LogInformation("AopLogger called after {Name}", context.ServiceMethod?.Name);
        var infoAttr = context.ServiceMethod?.GetCustomAttribute<LogInfoAttribute>();
        if (infoAttr != null)
        {
            var userId = userStore.UserId ?? GetUserIdFromContext(context);
            var result = context.ReturnValue as QueryResult;
            var l = new MinimalLog()
            {
                UserId = userId,
                Module = infoAttr!.Module ?? "",
                Action = infoAttr!.Action ?? "",
                Result = result?.IsSuccess ?? false ? "成功" : "失败",
                Message = result?.Message ?? "",
            };
        }
    }

    private static string GetUserIdFromContext(ProxyContext context)
    {
        if (context.ServiceMethod?.Name == nameof(IAuthService.SignInAsync))
        {
            var r = context.ReturnValue as QueryResult<UserInfo>;
            return r?.Payload?.UserId ?? "Unknow";
        }
        return "Unknow";
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

using Project.Constraints.Store;
using AutoAopProxyGenerator;
using AutoInjectGenerator;

namespace Project.Constraints.Aop;

[AutoInject(ServiceType = typeof(AopPermissionCheck))]
public class AopPermissionCheck : IAspectHandler
{
    private readonly IUserStore userStore;

    public AopPermissionCheck(IUserStore userStore)
    {
        this.userStore = userStore;
    }
    public Task Invoke(ProxyContext context, Func<Task> process)
    {
        var action = FormattedAction(context);
        if (userStore.UserInfo?.UserPowers == null)
        {
            return process.Invoke();
        }
        if (userStore.UserInfo.UserPowers.Contains(action) == false)
        {
            context.SetReturnValue(new QueryResult() { Success = false, Message = "没有权限" });
            return Task.CompletedTask;
        }
        else
        {
            return process.Invoke();
        }
    }

    private static string FormattedAction(ProxyContext context)
    {
        if (context.ServiceMethod.Name.EndsWith("Async"))
        {
            // 去掉末尾5个字符
            return context.ServiceMethod.Name[..^5];
        }
        else
        {
            return context.ServiceMethod.Name;
        }
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

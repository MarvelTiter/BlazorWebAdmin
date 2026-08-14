using BlazorTemplate.Constraints.Models.Permissions;
using BlazorTemplate.Constraints.Services;
using BlazorTemplate.Constraints.Store;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using AutoAopProxyGenerator;
using AutoInjectGenerator;
using Microsoft.Extensions.Logging;

namespace BlazorTemplate.Constraints.Aop;

// [AutoInject(ServiceType = typeof(AopLogger))]
[AutoInjectSelf]
public class AopLogger(IUserStore userStore, ILogger<AopLogger> logger, IServiceProvider provider) : IAspectHandler
{
    private readonly IRunLogService? runLogService = provider.GetService<IRunLogService>();

    public async Task Invoke(ProxyContext context, Func<Task> process)
    {
        logger.LogDebug("AopLogger called before {Name}", context.ServiceMethod?.Name);
        await process();
        logger.LogDebug("AopLogger called after {Name}", context.ServiceMethod?.Name);
        if (runLogService is null)
            return;
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
                Result = result?.IsSuccess ?? context.Status == ExecuteStatus.Executed ? "成功" : "失败",
                Message = result?.Message ?? "操作成功",
            };
            await runLogService.WriteLog(l);
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

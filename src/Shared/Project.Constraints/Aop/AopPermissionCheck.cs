using Project.Constraints.Store;
using AutoAopProxyGenerator;
using AutoInjectGenerator;
using System.Reflection;

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
            context.SetReturnValue(new QueryResult() { IsSuccess = false, Message = "没有权限" });
            return Task.CompletedTask;
        }
        else
        {
            return process.Invoke();
        }
    }

    private static string FormattedAction(ProxyContext context)
    {
        var related = context.ServiceMethod.GetCustomAttribute<RelatedPermissionAttribute>();
        if (!string.IsNullOrEmpty(related?.PermissionId))
        {
            return related.PermissionId;
        }
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

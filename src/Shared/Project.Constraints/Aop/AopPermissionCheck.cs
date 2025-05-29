using Project.Constraints.Store;
using AutoAopProxyGenerator;
using AutoInjectGenerator;
using System.Reflection;

namespace Project.Constraints.Aop;

// [AutoInject(ServiceType = typeof(AopPermissionCheck))]
[AutoInjectSelf]
public class AopPermissionCheck(IUserStore userStore) : IAspectHandler
{
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
        string? p = !string.IsNullOrEmpty(related?.PermissionId) ? related.PermissionId : context.ServiceMethod.Name;
        return p.EndsWith("Async") ? p[..^5] : p;
    }
}

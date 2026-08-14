using AutoAopProxyGenerator;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTemplate.ClientCore.Aop;

// [AutoInject(ServiceType = typeof(AopPermissionCheck))]
[AutoInjectSelf]
public class AopPermissionCheck(
    IUserStore userStore
    , IServiceProvider services) : IAspectHandler
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    public async Task Invoke(ProxyContext context, Func<Task> process)
    {
        if (userStore.UserInfo is null)
        {
            throw new InvalidOperationException("AopPermissionCheck Exception: userinfo is null");
        }
        var action = FormattedAction(context);
        if (userStore.UserInfo.Permissions is null)
        {
            await _lock.WaitAsync();
            try
            {
                if (userStore.UserInfo.Permissions is null)
                {
                    var permissionService = services.GetRequiredService<IPermissionService>();
#if RELEASE
                    if (permissionService is ITemplatePermissionService)
                    {
                        userStore.UserInfo.Permissions = [];
                    }
                    else
#endif
                    {
                        var permissions = await permissionService.GetUserPermissionsAsync(userStore.UserInfo.UserId);
                        userStore.UserInfo.Permissions = [.. permissions.Payload.Select(p => p.PermissionId)];
                    }
                }
            }
            finally
            {
                _lock.Release();
            }
        }
        if (userStore.UserInfo.Permissions.Contains(action) == false)
        {
            context.SetReturnValue(new QueryResult() { IsSuccess = false, Message = "没有权限" });
            return;
        }
        else
        {
            await process.Invoke();
        }
    }

    private static string FormattedAction(ProxyContext context)
    {
        var related = context.ServiceMethod.GetCustomAttribute<RelatedPermissionAttribute>();
        string? p = !string.IsNullOrEmpty(related?.PermissionId) ? related.PermissionId : context.ServiceMethod.Name;
        return p.EndsWith("Async") ? p[..^5] : p;
    }
}

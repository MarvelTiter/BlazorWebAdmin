using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Web.Shared.Auth;
public class DynamicPermissionRequirement : IAuthorizationRequirement
{

}
[AutoInject(ServiceType = typeof(IAuthorizationHandler))]
public class DynamicPermissionHandler(IUserStore userStore
    , IAppSession session
    , IServiceProvider serviceProvider) : AuthorizationHandler<DynamicPermissionRequirement>
{
    private readonly SemaphoreSlim locker = new(1, 1);
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicPermissionRequirement requirement)
    {
        if (!session.Loaded)
        {
            return;
        }
        if (userStore.UserInfo is null)
        {
            return;
        }
        if (context.User.Identity?.IsAuthenticated == false)
        {
            return;
        }
        if (userStore.UserInfo.Permissions is null)
        {
            await locker.WaitAsync();
            try
            {
                if (userStore.UserInfo.Permissions is null)
                {
                    var permissionService = serviceProvider.GetService<IPermissionService>();
                    if (permissionService is null)
                    {
                        return;
                    }
                    var permissions = await permissionService.GetUserPermissionsAsync(userStore.UserInfo.UserId);
                    userStore.UserInfo.Permissions = [.. permissions.Payload.Select(p => p.PermissionId)];
                }
            }
            finally
            {
                locker.Release();
            }
        }
        var current = session.RouterStore.Current?.RouteId;
        if (userStore.UserInfo.Permissions?.Contains(current) == true)
        {
            context.Succeed(requirement);
        }
    }
}

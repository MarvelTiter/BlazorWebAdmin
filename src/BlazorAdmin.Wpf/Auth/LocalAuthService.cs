using LightORM;
using Microsoft.Extensions.DependencyInjection;
using Project.AppCore.Services;
using Project.Constraints.Models;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Utils;
namespace BlazorAdmin.Wpf.Auth;
#if (ExcludeDefaultService)
#else
public class LocalAuthService(IServiceProvider services) : DefaultAuthenticationService(services)
{
    public override async Task<QueryResult> CheckUserPasswordAsync(UserPwd pwd)
    {
        var context = Services.GetRequiredService<IExpressionContext>();
        var old = await context.Select<User>()
            .Where(u => u.UserId == pwd.UserId)
            .FirstAsync();
        return old?.Password == pwd.OldPassword;
    }

    public override async Task<bool> CheckUserStatusAsync(UserInfo? userInfo)
    {
        if (userInfo is null) return false;
        var context = Services.GetRequiredService<IExpressionContext>();
        //var config = Services.GetService<IOptionsMonitor<Token>>()!;
        var u = await context.Select<User>().Where(u => u.UserId == userInfo.UserId).FirstAsync();

        var passwordEqual = u?.Password.ToHash() == userInfo.PasswordHash;

        var roles = await context.Select<UserRole>().Where(ur => ur.UserId == userInfo.UserId).ToListAsync(r => r.RoleId);

        var rolesChanged = roles.Count != userInfo.Roles.Length || roles.Except(userInfo.Roles).Any();

        var permissions = await context.Select<Permission>()
            .Distinct()
            .InnerJoin<RolePermission>((p, r) => p.PermissionId == r.PermissionId)
            .InnerJoin<UserRole>((_, r, u) => r.RoleId == u.RoleId)
            .Where((_, _, u) => u.UserId == userInfo.UserId)
            .ToListAsync(u => u.Tb1.PermissionId);
        bool permissionsChanged = false;
        if (userInfo.Permissions is not null)
        {
            permissionsChanged = permissions.Count != userInfo.Permissions.Length || permissions.Except(userInfo.Permissions).Any();
        }

        return passwordEqual && !rolesChanged && !permissionsChanged;
    }

    public override async Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd)
    {
        var context = Services.GetRequiredService<IExpressionContext>();
        var r = await context.Update<User>().Set(u => u.Password, pwd.Password)
            .Where(u => u.UserId == pwd.UserId)
            .ExecuteAsync();
        return r > 0;
    }

    protected override async Task<(bool Success, string Message, UserInfo Payload)> CreateUserInfoAsync(LoginFormModel loginForm)
    {
        var context = Services.GetService<IExpressionContext>();
        ArgumentNullException.ThrowIfNull(context);
        var username = loginForm.UserName;
        var password = loginForm.Password;
        var u = await context.Select<User>().Where(u => u.UserId == username).FirstAsync();
        var userInfo = new UserInfo
        {
            UserId = username,
            UserName = u?.UserName ?? "",
        };

        if (u is null)
        {
            return (false, $"用户：{username} 不存在", userInfo);
        }

        if (u!.Password != password)
        {
            return (false, "密码错误", userInfo);
        }

        return (true, "", userInfo);
    }

    protected override async Task<IList<string>> GetUserRolesAsync(UserInfo userInfo)
    {
        var context = Services.GetRequiredService<IExpressionContext>();
        var roles = await context.Select<UserRole>().Where(ur => ur.UserId == userInfo.UserId).ToListAsync(r => r.RoleId);
        return roles;
    }
}
#endif

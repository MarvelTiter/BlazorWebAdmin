using LightORM;
using Microsoft.Extensions.DependencyInjection;
using BlazorTemplate.AppServer.Services;
namespace BlazorAdmin.Wpf.Auth;
#if (ExcludeDefaultService)
#else
public class LocalAuthService(IServiceProvider services) : DefaultAuthenticationService(services)
{
    public override async Task<QueryResult> CheckUserPasswordAsync(UserPwd pwd)
    {
        var context = Services.GetRequiredService<IExpressionContext>();
        var old = await context.Select<TemplateUser>()
            .Where(u => u.UserId == pwd.UserId)
            .FirstAsync();
        return old?.Password == pwd.OldPassword;
    }

    public override async Task<bool> CheckUserStatusAsync(UserInfo? userInfo)
    {
        if (userInfo is null) return false;
        var context = Services.GetRequiredService<IExpressionContext>();
        //var config = Services.GetService<IOptionsMonitor<Token>>()!;
        var u = await context.Select<TemplateUser>().Where(u => u.UserId == userInfo.UserId).FirstAsync();

        var passwordEqual = u?.Password.ToHash() == userInfo.PasswordHash;

        var roles = await context.Select<TemplateUserRole>().Where(ur => ur.UserId == userInfo.UserId).ToListAsync(r => r.RoleId);

        var rolesChanged = roles.Count != userInfo.Roles.Length || roles.Except(userInfo.Roles).Any();

        var permissions = await context.Select<TemplatePermission>()
            .Distinct()
            .InnerJoin<TemplateRolePermission>((p, r) => p.PermissionId == r.PermissionId)
            .InnerJoin<TemplateUserRole>((_, r, u) => r.RoleId == u.RoleId)
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
        var r = await context.Update<TemplateUser>().Set(u => u.Password, pwd.Password)
            .Where(u => u.UserId == pwd.UserId)
            .ExecuteAsync();
        return r > 0;
    }

    protected override async Task<QueryResult<UserInfo>> CreateUserInfoAsync(LoginFormModel loginForm)
    {
        var context = Services.GetService<IExpressionContext>();
        ArgumentNullException.ThrowIfNull(context);
        var username = loginForm.UserName;
        var password = loginForm.Password;
        var u = await context.Select<TemplateUser>().Where(u => u.UserId == username).FirstAsync();
        var userInfo = new UserInfo
        {
            UserId = username,
            UserName = u?.UserName ?? "",
        };

        if (u is null)
        {
            return userInfo.Result(false).SetMessage($"用户：{username} 不存在");
        }

        if (u!.Password != password)
        {
            return userInfo.Result(false).SetMessage("密码错误");
        }

        return userInfo.Result();
    }

    protected override async Task<IList<string>> GetUserRolesAsync(UserInfo userInfo)
    {
        var context = Services.GetRequiredService<IExpressionContext>();
        var roles = await context.Select<TemplateUserRole>().Where(ur => ur.UserId == userInfo.UserId).ToListAsync(r => r.RoleId);
        return roles;
    }
}
#endif

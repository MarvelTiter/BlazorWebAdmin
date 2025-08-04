using AutoAopProxyGenerator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Options;
using Project.Constraints.Utils;
using Project.Web.Shared.Pages;

namespace Project.AppCore.Services;

public abstract class DefaultAuthenticationService(IServiceProvider services) : IAuthService
{
    protected IServiceProvider Services { get; } = services;

    public virtual async Task<QueryResult<UserInfo>> SignInAsync(LoginFormModel loginForm)
    {
        var (success, message, userInfo) = await CreateUserInfoAsync(loginForm);
        if (!success)
        {
            return QueryResult.Fail<UserInfo>(message);
        }
        var roles = await GetUserRolesAsync(userInfo);
        userInfo.Roles = [.. roles];
        userInfo.PasswordHash = loginForm.Password.ToHash();
        // var projSetting = Services.GetService<IProjectSettingService>();
        // var powers = await projSetting!.GetUserPowersAsync(userInfo);
        // userInfo.UserPowers = [.. powers.Where(p => p.PowerType != PowerType.Page).Select(p => p.PowerId)];
        // userInfo.UserPages = [.. powers.Where(p => p.PowerType == PowerType.Page).Select(p => p.PowerId)];
        return QueryResult.Success<UserInfo>(message).SetPayload(userInfo);
    }

    /// <summary>
    /// 用户登录验证，为<see cref="SignInAsync(LoginFormModel)"/>提供<see cref="UserInfo"/>
    /// </summary>
    /// <param name="loginForm"></param>
    /// <returns></returns>
    protected abstract Task<(bool Success, string Message, UserInfo Payload)> CreateUserInfoAsync(LoginFormModel loginForm);

    /// <summary>
    /// 根据<paramref name="userInfo"/>查询用户的角色信息
    /// </summary>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    protected abstract Task<IList<string>> GetUserRolesAsync(UserInfo userInfo);

    public abstract Task<QueryResult> CheckUserPasswordAsync(UserPwd pwd);

    public abstract Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd);

    public abstract Task<bool> CheckUserStatusAsync(UserInfo? userInfo);

    public virtual TimeSpan RevalidationInterval { get; } = TimeSpan.FromMinutes(5);

    public virtual async Task SignOutAsync()
    {
        var httpContextAccessor = Services.GetService<IHttpContextAccessor>();
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        var ctx = httpContextAccessor.HttpContext;
        if (ctx == null)
        {
            return;
        }

        // var ctx = httpContextAccessor.HttpContext.User;
        await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        // 解码了
        var redirect = ctx.Request.Query["Redirect"];
        // 重新编码
        var encoded = Uri.EscapeDataString(redirect!); // System.Web.HttpUtility.HtmlEncode(redirect);
        ctx.Response.Redirect($"/account/login?Redirect={encoded}");
    }
}
#if (!ExcludeDefaultService)
[GenAspectProxy]
public class BlazorAdminAuthenticationService(IServiceProvider services) : DefaultAuthenticationService(services)
{
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
        var context = Services.GetService<IExpressionContext>()!;
        var roles = await context.Select<UserRole>().Where(ur => ur.UserId == userInfo.UserId).ToListAsync(r => r.RoleId);
        return roles;
    }

    public override async Task<bool> CheckUserStatusAsync(UserInfo? userInfo)
    {
        if (userInfo is null) return false;
        var context = Services.GetService<IExpressionContext>()!;
        //var config = Services.GetService<IOptionsMonitor<Token>>()!;
        var u = await context.Select<User>().Where(u => u.UserId == userInfo.UserId).FirstAsync();
        return u?.Password.ToHash() == userInfo.PasswordHash;
    }

    public override async Task<QueryResult> CheckUserPasswordAsync(UserPwd pwd)
    {
        var context = Services.GetService<IExpressionContext>();
        ArgumentNullException.ThrowIfNull(context);
        var old = await context.Select<User>()
            .Where(u => u.UserId == pwd.UserId)
            .FirstAsync();
        return old?.Password == pwd.OldPassword;
    }

    public override async Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd)
    {
        var context = Services.GetService<IExpressionContext>();
        ArgumentNullException.ThrowIfNull(context);
        var r = await context.Update<User>().Set(u => u.Password, pwd.Password)
            .Where(u => u.UserId == pwd.UserId)
            .ExecuteAsync();
        return r > 0;
    }
}
#endif

public class FreeAuthenticationService(IServiceProvider services) : DefaultAuthenticationService(services)
{
    public override Task<QueryResult> CheckUserPasswordAsync(UserPwd pwd)
    {
        return Task.FromResult<QueryResult>(true);
    }

    public override Task<bool> CheckUserStatusAsync(UserInfo? userInfo)
    {
        return Task.FromResult(true);
    }

    public override Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd)
    {
        return Task.FromResult<QueryResult>(true);
    }

    protected override Task<(bool Success, string Message, UserInfo Payload)> CreateUserInfoAsync(LoginFormModel loginForm)
    {
        return Task.FromResult((true, "", UserInfo.Visitor));
    }

    protected override Task<IList<string>> GetUserRolesAsync(UserInfo userInfo)
    {
        return Task.FromResult<IList<string>>([]);
    }
}
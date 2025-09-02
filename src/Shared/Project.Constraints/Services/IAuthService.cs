using AutoAopProxyGenerator;
using AutoInjectGenerator;
using AutoWasmApiGenerator;
using MT.Generators.Abstraction;
using Project.Constraints.Aop;
using Project.Constraints.Models.Permissions;

namespace Project.Constraints.Services;

[WebController(Route = "account")]
[AddAspectHandler(AspectType = typeof(AopLogger))]
public interface IAuthService
{
    /// <summary>
    /// 登录接口
    /// </summary>
    /// <param name="loginForm"></param>
    /// <returns></returns>
    [WebMethod(Route = "login")]
    [LogInfo(Module = "登录模块", Action = "用户登录")]
    Task<QueryResult<UserInfo>> SignInAsync(LoginFormModel loginForm);

    /// <summary>
    /// 不能直接调用。退出登录请使用<see cref="IAuthenticationStateProvider.ClearState"/>
    /// </summary>
    /// <returns></returns>
    [WebMethod(Method = WebMethod.Get, Route = "logout")]
    [LogInfo(Module = "登录模块", Action = "用户登出")]
    Task SignOutAsync();

    /// <summary>
    /// 校验用户密码
    /// </summary>
    /// <param name="pwd"></param>
    /// <returns></returns>
    Task<QueryResult> CheckUserPasswordAsync(UserPwd pwd);

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="pwd"></param>
    /// <returns></returns>
    [LogInfo(Module = "登录模块", Action = "修改密码")]
    Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd);

    /// <summary>
    /// 检查用户状态，是否修改过密码，是否修改过角色，是否修改过权限等
    /// </summary>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    Task<bool> CheckUserStatusAsync(UserInfo? userInfo);
        
}
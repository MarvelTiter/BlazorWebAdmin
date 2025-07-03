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
    [WebMethod(Route = "login")]
    [LogInfo(Module = "登录模块", Action = "用户登录")]
    Task<QueryResult<UserInfo>> SignInAsync(LoginFormModel loginForm);

    [WebMethod(Method = WebMethod.Get, Route = "logout")]
    [LogInfo(Module = "登录模块", Action = "用户登出")]
    Task SignOutAsync();

    Task<QueryResult> CheckUserPasswordAsync(UserPwd pwd);

    [LogInfo(Module = "登录模块", Action = "修改密码")]
    Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd);

    Task<bool> CheckUserStatusAsync(UserInfo? userInfo);
        
}
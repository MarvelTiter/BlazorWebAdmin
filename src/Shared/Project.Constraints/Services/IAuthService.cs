using AutoInjectGenerator;
using AutoWasmApiGenerator;
using MT.Generators.Abstraction;
using Project.Constraints.Models.Permissions;

namespace Project.Constraints.Services
{
    [WebController(Route = "account")]
    [ApiInvokerGenerate(typeof(AutoInjectAttribute))]
    [AttachAttributeArgument(typeof(ApiInvokerGenerateAttribute), typeof(AutoInjectAttribute), "Group", "WASM")]
    public interface IAuthService
    {
        [WebMethod(Route = "login")]
        Task<QueryResult<UserInfo>> SignInAsync(LoginFormModel loginForm);
        [WebMethod(Method = WebMethod.Get, Route = "logout")]
        Task SignOutAsync();
        Task<QueryResult> CheckUserPasswordAsync(UserPwd pwd);
        Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd);
    }
}

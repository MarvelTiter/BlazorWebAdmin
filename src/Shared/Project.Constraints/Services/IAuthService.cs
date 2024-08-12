using AutoInjectGenerator;
using AutoWasmApiGenerator;
using MT.Generators.Abstraction;

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
    }
}

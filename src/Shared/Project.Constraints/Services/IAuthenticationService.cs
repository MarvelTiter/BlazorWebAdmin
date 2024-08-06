using AutoInjectGenerator;
using AutoWasmApiGenerator;
using MT.Generators.Abstraction;

namespace Project.Constraints.Services
{
    [WebController(Route = "auth")]
    [ApiInvokerGenerate(typeof(AutoInjectAttribute))]
    [AttachAttributeArgument(typeof(ApiInvokerGenerateAttribute), typeof(AutoInjectAttribute), "Group", "WASM")]
    public interface IAuthenticationService
    {
        Task<QueryResult<UserInfo>> SignInAsync(LoginFormModel loginForm);
        Task<QueryResult> SignOutAsync(string? token);
    }
}

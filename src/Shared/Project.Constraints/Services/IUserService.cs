using AutoInjectGenerator;
using AutoWasmApiGenerator;
using MT.Generators.Abstraction;
using Project.Constraints.Aop;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services
{
    //[Aspectable(AspectHandleType = typeof(LogAop))]
    [LogAop]
    public partial interface IUserService<TUser> where TUser : IUser
    {
        Task<QueryCollectionResult<TUser>> GetUserListAsync(GenericRequest<TUser> req);
        [LogInfo(Action = "新增用户", Module = "权限控制")]
        Task<QueryResult> InsertUserAsync(TUser user);
        [LogInfo(Action = "修改用户", Module = "权限控制")]
        Task<QueryResult> UpdateUserAsync(TUser user);
        [LogInfo(Action = "删除用户", Module = "权限控制")]
        Task<QueryResult> DeleteUserAsync(TUser user);
        [LogInfo(Action = "修改密码", Module = "权限控制")]
        Task<TUser?> GetUserAsync(string id);
    }

    [WebController(Route = "user")]
    [ApiInvokerGenerate(typeof(AutoInjectAttribute))]
    [AttachAttributeArgument(typeof(ApiInvokerGenerateAttribute), typeof(AutoInjectAttribute), "Group", "WASM")]
    public interface IUserService
    {
        Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd);
    }

    [WebController(Route = "user")]
    [ApiInvokerGenerate(typeof(AutoInjectAttribute))]
    [AttachAttributeArgument(typeof(ApiInvokerGenerateAttribute), typeof(AutoInjectAttribute), "Group", "WASM")]
    public interface IStandardUserService : IUserService<User>
    {

    }
}

using AutoAopProxyGenerator;
using AutoInjectGenerator;
using AutoWasmApiGenerator;
using MT.Generators.Abstraction;
using Project.Constraints.Aop;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services
{
    [AddAspectHandler(AspectType = typeof(AopLogger))]
    [AddAspectHandler(AspectType = typeof(AopPermissionCheck))]
    public partial interface IUserService<TUser> where TUser : IUser
    {
        [IgnoreAspect]
        Task<QueryCollectionResult<TUser>> GetUserListAsync(GenericRequest<TUser> req);

        [LogInfo(Action = "新增用户", Module = "权限控制")]
        Task<QueryResult> InsertUserAsync(TUser user);
        [LogInfo(Action = "修改用户角色", Module = "权限控制")]
        [RelatedPermission(PermissionId = nameof(UpdateUserAsync))]
        Task<QueryResult> SaveUserWithRolesAsync(TUser user);

        [LogInfo(Action = "修改用户", Module = "权限控制")]
        Task<QueryResult> UpdateUserAsync(TUser user);

        [LogInfo(Action = "删除用户", Module = "权限控制")]
        Task<QueryResult> DeleteUserAsync(TUser user);

        [IgnoreAspect]
        Task<TUser?> GetUserAsync(string id);
    }

#if (ExcludeDefaultService)
#else
    // [WebController(Route = "user", Authorize = true)]
    // [ApiInvokerGenerate(typeof(AutoInjectAttribute))]
    // [AttachAttributeArgument(typeof(ApiInvokerGenerateAttribute), typeof(AutoInjectAttribute), "Group", "WASM")]
    // public interface IUserService
    // {
    //     Task<QueryResult> CheckUserPasswordAsync(string oldPassword);
    //     Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd);
    // }

    [WebController(Route = "user", Authorize = true)]
    [ApiInvokerGenerate]
    public interface IStandardUserService : IUserService<User>
    {

    }
#endif
}

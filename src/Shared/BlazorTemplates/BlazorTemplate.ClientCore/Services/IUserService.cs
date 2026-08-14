using AutoAopProxyGenerator;
using AutoWasmApiGenerator;
using BlazorTemplate.ClientCore.Aop;

namespace BlazorTemplate.ClientCore.Services;

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


[WebController(Route = "template/user", Authorize = true)]
[AddAspectHandler(AspectType = typeof(AopLogger))]
[AddAspectHandler(AspectType = typeof(AopPermissionCheck))]
public interface ITemplateUserService : IUserService<TemplateUser>
{
    [RelatedPermission(PermissionId = nameof(UpdateUserAsync))]
    Task<QueryResult> SavePropertyAsync(TemplateUser user, string property);


    [RelatedPermission(PermissionId = nameof(UpdateUserAsync))]
    Task<QueryResult> SavePropertiesAsync(TemplateUser user, string[] property);
}

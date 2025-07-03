using AutoAopProxyGenerator;
using AutoInjectGenerator;
using AutoWasmApiGenerator;
using MT.Generators.Abstraction;
using Project.Constraints.Aop;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services;

//[WebController(Route = "permission", Authorize = true)]
//[ApiInvokerGenerate]
public interface IPermissionService
{
    /// <summary>
    /// 初始化用户菜单，不使用IPermission，主要是考虑生成对应接口时，调用接口反序列化需要具体的类型
    /// </summary>
    /// <param name="usrId"></param>
    /// <returns></returns>
    [IgnoreAspect]
    Task<QueryCollectionResult<MinimalPermission>> GetUserPermissionsAsync(string usrId);
    // Task<QueryCollectionResult<IPermission>> GetUserPermissionsAsync(string usrId);
}

public interface IPermissionService<TPermission, TRole> : IPermissionService
    where TPermission : IPermission
    where TRole : IRole
{
    [IgnoreAspect]
    Task<QueryCollectionResult<TPermission>> GetPermissionListAsync(GenericRequest<TPermission> req);

    [WebMethod(Method = WebMethod.Get)]
    [IgnoreAspect]
    Task<QueryCollectionResult<TPermission>> GetAllPermissionAsync();

    [IgnoreAspect]
    Task<QueryCollectionResult<TRole>> GetRoleListAsync(GenericRequest<TRole> req);

    [IgnoreAspect]
    Task<QueryCollectionResult<TRole>> GetAllRoleAsync();

    [IgnoreAspect]
    Task<QueryCollectionResult<TPermission>> GetPermissionListByUserIdAsync(string usrId);

    [IgnoreAspect]
    Task<QueryCollectionResult<TPermission>> GetPermissionListByRoleIdAsync(string roleId);

    [IgnoreAspect]
    Task<QueryCollectionResult<TRole>> GetUserRolesAsync(string usrId);

    [LogInfo(Action = "修改角色权限", Module = "权限控制")]
    [RelatedPermission(PermissionId = nameof(UpdateRoleAsync))]
    Task<QueryResult> SaveRoleWithPermissionsAsync(TRole role);

    [LogInfo(Action = "更新权限信息", Module = "权限控制")]
    Task<QueryResult> UpdatePermissionAsync(TPermission Permission);

    [LogInfo(Action = "新增权限信息", Module = "权限控制")]
    Task<QueryResult> InsertPermissionAsync(TPermission Permission);

    [LogInfo(Action = "删除权限信息", Module = "权限控制")]
    Task<QueryResult> DeletePermissionAsync(TPermission Permission);

    [LogInfo(Action = "修改角色信息", Module = "权限控制")]
    Task<QueryResult> UpdateRoleAsync(TRole role);

    [LogInfo(Action = "新增角色", Module = "权限控制")]
    Task<QueryResult> InsertRoleAsync(TRole role);

    [LogInfo(Action = "删除角色", Module = "权限控制")]
    Task<QueryResult> DeleteRoleAsync(TRole role);
}

#if (ExcludeDefaultService)
#else
[WebController(Route = "default/permission", Authorize = true)]
[AddAspectHandler(AspectType = typeof(AopLogger))]
[AddAspectHandler(AspectType = typeof(AopPermissionCheck))]
public interface IStandardPermissionService : IPermissionService<Permission, Role>
{
}
#endif
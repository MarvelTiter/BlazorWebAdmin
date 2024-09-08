using AutoAopProxyGenerator;
using AutoInjectGenerator;
using AutoWasmApiGenerator;
using MT.Generators.Abstraction;
using Project.Constraints.Aop;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services;

[WebController(Route = "permission", Authorize = true)]
[ApiInvokerGenerate]
public interface IPermissionService
{
    Task<QueryCollectionResult<MinimalPower>> GetPowerListByUserIdAsync(string usrId);
}



[AddAspectHandler(AspectType = typeof(AopLogger))]
public interface IPermissionService<TPower, TRole>
    where TPower : IPower
    where TRole : IRole
{
    [IgnoreAspect]
    Task<QueryCollectionResult<TPower>> GetPowerListAsync(GenericRequest<TPower> req);

    [WebMethod(Method = WebMethod.Get)]
    [IgnoreAspect]
    Task<QueryCollectionResult<TPower>> GetAllPowerAsync();

    [IgnoreAspect]
    Task<QueryCollectionResult<TRole>> GetRoleListAsync(GenericRequest<TRole> req);

    [IgnoreAspect]
    Task<QueryCollectionResult<TRole>> GetAllRoleAsync();

    [IgnoreAspect]
    Task<QueryCollectionResult<TPower>> GetPowerListByUserIdAsync(string usrId);

    [IgnoreAspect]
    Task<QueryCollectionResult<TPower>> GetPowerListByRoleIdAsync(string roleId);

    [IgnoreAspect]
    Task<QueryCollectionResult<TRole>> GetUserRolesAsync(string usrId);

    [LogInfo(Action = "修改用户角色", Module = "权限控制")]
    Task<QueryResult> SaveUserRoleAsync(KeyRelations<string, string> relations);

    [LogInfo(Action = "修改角色权限", Module = "权限控制")]
    Task<QueryResult> SaveRolePowerAsync(KeyRelations<string, string> relations);

    [LogInfo(Action = "更新权限信息", Module = "权限控制")]
    Task<QueryResult> UpdatePowerAsync(TPower power);

    [LogInfo(Action = "新增权限信息", Module = "权限控制")]
    Task<QueryResult> InsertPowerAsync(TPower power);

    [LogInfo(Action = "删除权限信息", Module = "权限控制")]
    Task<QueryResult> DeletePowerAsync(TPower power);

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
[ApiInvokerGenerate]
public interface IStandardPermissionService : IPermissionService<Power, Role>
{
}
#endif
using Project.Constraints.Aop;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services
{
    public interface IPermissionService
    {
        Task<IQueryCollectionResult<IPower>> GetPowerListByUserIdAsync(string usrId);
    }
    //[Aspectable(AspectHandleType = typeof(LogAop))]
    [LogAop]
    [AutoInject]
    public partial interface IPermissionService<TPower, TRole> : IPermissionService
        where TPower : IPower
        where TRole : IRole
    {
        Task<IQueryCollectionResult<TPower>> GetPowerListAsync(GenericRequest<TPower> req);
        Task<IQueryCollectionResult<TPower>> GetPowerListAsync();
        Task<IQueryCollectionResult<TRole>> GetRoleListAsync(GenericRequest<TRole> req);
        Task<IQueryCollectionResult<TRole>> GetRoleListAsync();
        new Task<IQueryCollectionResult<TPower>> GetPowerListByUserIdAsync(string usrId);
        Task<IQueryCollectionResult<TPower>> GetPowerListByRoleIdAsync(string roleId);
        Task<IQueryCollectionResult<TRole>> GetUserRolesAsync(string usrId);
        [LogInfo(Action = "修改用户角色", Module = "权限控制")]
        Task<IQueryResult<bool>> SaveUserRoleAsync(string usrId, params string[] roles);
        [LogInfo(Action = "修改角色权限", Module = "权限控制")]
        Task<IQueryResult<bool>> SaveRolePowerAsync(string roleId, params string[] powers);
        [LogInfo(Action = "更新权限信息", Module = "权限控制")]
        Task<IQueryResult<bool>> UpdatePowerAsync(TPower power);
        [LogInfo(Action = "新增权限信息", Module = "权限控制")]
        Task<IQueryResult<bool>> InsertPowerAsync(TPower power);
        [LogInfo(Action = "删除权限信息", Module = "权限控制")]
        Task<IQueryResult<bool>> DeletePowerAsync(TPower power);
        [LogInfo(Action = "修改角色信息", Module = "权限控制")]
        Task<IQueryResult<bool>> UpdateRoleAsync(TRole role);
        [LogInfo(Action = "新增角色", Module = "权限控制")]
        Task<IQueryResult<bool>> InsertRoleAsync(TRole role);
        [LogInfo(Action = "删除角色", Module = "权限控制")]
        Task<IQueryResult<bool>> DeleteRoleAsync(TRole role);
    }

    
}

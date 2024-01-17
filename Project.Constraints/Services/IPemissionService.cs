using Project.Constraints.Aop;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services
{
    //[Aspectable(AspectHandleType = typeof(LogAop))]
    [LogAop]
    [AutoInject]
    public partial interface IPermissionService
    {
        Task<IQueryCollectionResult<Power>> GetPowerListAsync(GenericRequest<Power> req);
        Task<IQueryCollectionResult<Power>> GetPowerListAsync();
        Task<IQueryCollectionResult<Role>> GetRoleListAsync(GenericRequest<Role> req);
        Task<IQueryCollectionResult<Role>> GetRoleListAsync();
        Task<IQueryCollectionResult<Power>> GetPowerListByUserIdAsync(string usrId);
        Task<IQueryCollectionResult<Power>> GetPowerListByRoleIdAsync(string roleId);
        Task<IQueryCollectionResult<Role>> GetUserRolesAsync(string usrId);
        [LogInfo(Action = "修改用户角色", Module = "权限控制")]
        Task<IQueryResult<bool>> SaveUserRole(string usrId, params string[] roles);
        [LogInfo(Action = "修改角色权限", Module = "权限控制")]
        Task<IQueryResult<bool>> SaveRolePower(string roleId, params string[] powers);
        [LogInfo(Action = "更新权限信息", Module = "权限控制")]
        Task<IQueryResult<bool>> UpdatePowerAsync(Power power);
        [LogInfo(Action = "新增权限信息", Module = "权限控制")]
        Task<IQueryResult<bool>> InsertPowerAsync(Power power);
        [LogInfo(Action = "删除权限信息", Module = "权限控制")]
        Task<IQueryResult<bool>> DeletePowerAsync(Power power);
        [LogInfo(Action = "修改角色信息", Module = "权限控制")]
        Task<IQueryResult<bool>> UpdateRoleAsync(Role role);
        [LogInfo(Action = "新增角色", Module = "权限控制")]
        Task<IQueryResult<bool>> InsertRoleAsync(Role role);
        [LogInfo(Action = "删除角色", Module = "权限控制")]
        Task<IQueryResult<bool>> DeleteRoleAsync(Role role);
    }
}

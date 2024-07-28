using Project.Constraints.Aop;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services
{
    public interface IPermissionService
    {
        Task<QueryCollectionResult<IPower>> GetPowerListByUserIdAsync<TPower, TRolePower, TUserRole>(string usrId)
            where TPower : class, IPower, new()
            where TRolePower : class, IRolePower, new()
            where TUserRole : class, IUserRole, new();
    }

    public interface IStandardPermissionService : IPermissionService<Power, Role>
    {

    }

    //[Aspectable(AspectHandleType = typeof(LogAop))]
    [LogAop]
    public partial interface IPermissionService<TPower, TRole>
        where TPower : IPower
        where TRole : IRole
    {
        Task<QueryCollectionResult<TPower>> GetPowerListAsync(GenericRequest<TPower> req);
        Task<QueryCollectionResult<TPower>> GetAllPowerAsync();
        Task<QueryCollectionResult<TRole>> GetRoleListAsync(GenericRequest<TRole> req);
        Task<QueryCollectionResult<TRole>> GetAllRoleAsync();
        Task<QueryCollectionResult<TPower>> GetPowerListByUserIdAsync(string usrId);
        Task<QueryCollectionResult<TPower>> GetPowerListByRoleIdAsync(string roleId);
        Task<QueryCollectionResult<TRole>> GetUserRolesAsync(string usrId);
        [LogInfo(Action = "修改用户角色", Module = "权限控制")]
        Task<QueryResult<bool>> SaveUserRoleAsync(KeyRelations<string, string> relations);
        [LogInfo(Action = "修改角色权限", Module = "权限控制")]
        Task<QueryResult<bool>> SaveRolePowerAsync(KeyRelations<string, string> relations);
        [LogInfo(Action = "更新权限信息", Module = "权限控制")]
        Task<QueryResult<bool>> UpdatePowerAsync(TPower power);
        [LogInfo(Action = "新增权限信息", Module = "权限控制")]
        Task<QueryResult<bool>> InsertPowerAsync(TPower power);
        [LogInfo(Action = "删除权限信息", Module = "权限控制")]
        Task<QueryResult<bool>> DeletePowerAsync(TPower power);
        [LogInfo(Action = "修改角色信息", Module = "权限控制")]
        Task<QueryResult<bool>> UpdateRoleAsync(TRole role);
        [LogInfo(Action = "新增角色", Module = "权限控制")]
        Task<QueryResult<bool>> InsertRoleAsync(TRole role);
        [LogInfo(Action = "删除角色", Module = "权限控制")]
        Task<QueryResult<bool>> DeleteRoleAsync(TRole role);
    }


}

using Project.Constraints.Aop;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services
{
    //[Aspectable(AspectHandleType = typeof(LogAop))]
    [LogAop]
    [IgnoreAutoInject]
    public partial interface IUserService<TUser> : IUserService where TUser : IUser
    {
        Task<IQueryCollectionResult<TUser>> GetUserListAsync(GenericRequest<TUser> req);
        [LogInfo(Action = "新增用户", Module = "权限控制")]
        Task<IQueryResult> InsertUserAsync(TUser user);
        [LogInfo(Action = "修改用户", Module = "权限控制")]
        Task<IQueryResult> UpdateUserAsync(TUser user);
        [LogInfo(Action = "删除用户", Module = "权限控制")]
        Task<IQueryResult> DeleteUserAsync(TUser user);
        [LogInfo(Action = "修改密码", Module = "权限控制")]
        Task<TUser> GetUserAsync(string id);
    }

    [IgnoreAutoInject]
    public interface IUserService
    {
        Task<IQueryResult> ModifyUserPasswordAsync(string uid, string old, string pwd);
    }
}

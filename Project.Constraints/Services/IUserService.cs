using Project.Constraints.Aop;
using Project.Constraints.Models.Request;
using Project.Models.Entities;

namespace Project.Constraints.Services
{
    //[Aspectable(AspectHandleType = typeof(LogAop))]
    [LogAop]
    [AutoInject]
    public partial interface IUserService
    {
        Task<IQueryCollectionResult<User>> GetUserListAsync(GenericRequest<User> req);
        [LogInfo(Action = "新增用户", Module = "权限控制")]
        Task<IQueryResult> InsertUserAsync(User user);
        [LogInfo(Action = "修改用户", Module = "权限控制")]
        Task<IQueryResult> UpdateUserAsync(User user);
        [LogInfo(Action = "删除用户", Module = "权限控制")]
        Task<IQueryResult> DeleteUserAsync(User user);
        [LogInfo(Action = "修改密码", Module = "权限控制")]
        Task<IQueryResult> ModifyUserPasswordAsync(string uid, string old, string pwd);
        Task<User> GetUserAsync(string id);
    }
}

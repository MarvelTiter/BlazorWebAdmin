using LogAopCodeGenerator;
using Project.AppCore.Aop;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Services
{
    [Aspectable(AspectHandleType = typeof(LogAop))]
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
        Task<IQueryResult> ModifyUserPasswordAsync(string uid, string pwd);
    }
}

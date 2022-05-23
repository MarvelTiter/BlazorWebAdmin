using LogAopCodeGenerator;
using Project.AppCore.Aop;
using Project.Models;
using Project.Models.Permissions;

namespace Project.AppCore.Services
{
    [Aspectable(AspectHandleType = typeof(LogAop))]
    public interface ILoginService
    {
        [LogInfo(Action = "用户登录", Module = "登录模块")]
        Task<IQueryResult<UserInfo>> LoginAsync(string username, string password);
        Task<bool> CheckUser(UserInfo info);
        Task<bool> LogoutAsync();
    }
}

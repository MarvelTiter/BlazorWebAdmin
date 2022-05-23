using LogAopCodeGenerator;
using Project.Models;

namespace Project.Services.interfaces
{
    [Aspectable]
    public interface ILoginService
    {
        [LogInfo(Action = "用户登录", Module = "登录模块")]
        Task<IQueryResult<UserInfo>> LoginAsync(string username, string password);
        Task<bool> CheckUser(UserInfo info);
        Task<bool> LogoutAsync();
    }
}

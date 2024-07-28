using Project.Constraints.Aop;
using Project.Constraints.Models;

namespace Project.Constraints.Services
{
    //[Aspectable(AspectHandleType = typeof(LogAop))]
    [LogAop]
    public partial interface ILoginService
    {
        [LogInfo(Action = "用户登录", Module = "登录模块")]
        Task<QueryResult<UserInfo>> LoginAsync(string username, string password);
        [LogInfo(Action = "用户登录[缓存]", Module = "登录模块")]
        Task<QueryResult<bool>> UpdateLastLoginTimeAsync(UserInfo info);
        Task<bool> CheckUser(UserInfo info);
        [LogInfo(Action = "用户登出", Module = "登录模块")]
        Task<QueryResult<bool>> LogoutAsync();
    }
}

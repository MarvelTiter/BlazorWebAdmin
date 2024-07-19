using Project.Constraints.Aop;
using Project.Constraints.Models;

namespace Project.Constraints.Services
{
    //[Aspectable(AspectHandleType = typeof(LogAop))]
    [LogAop]
    [AutoInject]
    [WebApiGenerator.Attributes.WebController]
    public partial interface ILoginService
    {
        [LogInfo(Action = "用户登录", Module = "登录模块")]
        Task<IQueryResult<UserInfo>> LoginAsync(string username, string password);
        [LogInfo(Action = "用户登录[缓存]", Module = "登录模块")]
        Task<IQueryResult<bool>> UpdateLastLoginTimeAsync(UserInfo info);
        Task<bool> CheckUser(UserInfo info);
        [LogInfo(Action = "用户登出", Module = "登录模块")]
        Task<IQueryResult<bool>> LogoutAsync();
    }
}

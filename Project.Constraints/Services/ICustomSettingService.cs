using Project.Constraints.Store.Models;

namespace Project.Constraints.Services
{
    [AutoInject]
    public interface ICustomSettingService
    {
        Type? GetUserPageType();
        Type? GetDashboardType();
        Type? GetPermissionPageType();
        Type? GetRolePermissionPageType();
        Type? GetRunLogPageType();
        Task<IQueryResult<UserInfo>> GetUserInfoAsync(string username, string password);
        Task<int> UpdateLoginInfo(UserInfo info);
        /// <summary>
        /// 登录成功钩子
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        Task<bool> OnLoginSuccessAsync(IQueryResult<UserInfo> result);
        /// <summary>
        /// 导航钩子
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        Task<bool> RouterChangingAsync(TagRoute route);
        /// <summary>
        /// 初始化菜单钩子
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        Task<bool> RouteMetaFilterAsync(RouterMeta meta);
        /// <summary>
        /// 初次显示主页
        /// </summary>
        /// <returns></returns>
        Task AfterWebApplicationAccessed();
    }
}

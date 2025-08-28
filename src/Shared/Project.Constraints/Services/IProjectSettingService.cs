using Project.Constraints.Models.Permissions;
using Project.Constraints.Store.Models;

namespace Project.Constraints.Services;

public interface IProjectSettingService
{
    /// <summary>
    /// 用户登录逻辑
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    //Task<QueryResult<UserInfo>> GetUserInfoAsync(string username, string password);
    /// <summary>
    /// 更新登录信息
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    //Task<int> UpdateLoginInfo(UserInfo info);

    /// <summary>
    /// 登录成功钩子
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    Task LoginSuccessAsync(UserInfo info);

    /// <summary>
    /// 用户登录成功后与主页显示前
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    Task<QueryResult> LoginInterceptorAsync(UserInfo info);

    /// <summary>
    /// 初始化用户可用菜单
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    Task<IEnumerable<MinimalPermission>> GetUserPowersAsync(UserInfo info);

    /// <summary>
    /// 导航钩子
    /// </summary>
    /// <param name="route"></param>
    /// <returns></returns>
    Task<bool> RouterChangingAsync(RouteTag route);

    /// <summary>
    /// 初始化菜单钩子
    /// </summary>
    /// <param name="meta"></param>
    /// <returns></returns>
    Task<bool> RouteMetaFilterAsync(RouteMeta meta);

    /// <summary>
    /// 初次显示主页
    /// </summary>
    /// <returns></returns>
    Task AfterWebApplicationAccessed();
        
    TimeSpan RevalidationInterval { get; }
}
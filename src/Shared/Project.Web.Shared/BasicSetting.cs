using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Project.Constraints.Models;
using Project.Constraints.Options;
using Project.Constraints.Store.Models;

namespace Project.Web.Shared;

/// <summary>
/// 提供基本设置功能的服务类，实现自定义用户信息和权限处理
/// </summary>
public class BasicSetting : IProjectSettingService //, IDisposable
{
    /// <summary>
    /// 服务提供者，用于解析其他服务
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    protected IUserStore UserStore { get; }
    protected IOptionsMonitor<AppSetting> AppSetting { get; }

    /// <summary>
    /// 构造函数，接收服务提供者
    /// </summary>
    /// <param name="services">服务提供者</param>
    public BasicSetting(IServiceProvider services)
    {
        ServiceProvider = services;
        UserStore = services.GetRequiredService<IUserStore>();
        AppSetting = services.GetRequiredService<IOptionsMonitor<AppSetting>>();
    }

    public virtual TimeSpan RevalidationInterval => TimeSpan.FromMinutes(5);

    /// <summary>
    /// 在登录成功后调用此方法以设置当前用户信息
    /// </summary>
    /// <param name="result">登录成功的用户信息</param>
    /// <returns>完成任务</returns>
    public virtual Task LoginSuccessAsync(UserInfo result) => Task.Delay(500);

    /// <summary>
    /// 登录拦截器，用于在登录后执行自定义逻辑
    /// </summary>
    /// <param name="result">登录成功的用户信息</param>
    /// <returns>查询结果任务</returns>
    public virtual Task<QueryResult> LoginInterceptorAsync(UserInfo result) => Task.FromResult(QueryResult.Success());

    /// <summary>
    /// 在Web应用程序被访问后调用，用于执行自定义逻辑
    /// </summary>
    /// <returns>完成任务</returns>
    public virtual Task AfterWebApplicationAccessed() => Task.CompletedTask;

    /// <summary>
    /// 获取用户的权限
    /// </summary>
    /// <param name="info">用户信息</param>
    /// <returns>用户权限的集合</returns>
    /// <exception cref="NotImplementedException">如果未实现权限服务</exception>
    public virtual async Task<IEnumerable<MinimalPermission>> GetUserPowersAsync(UserInfo info)
    {
        var permissionService = ServiceProvider.GetService<IPermissionService>();
        if (permissionService == null)
        {
            return [];
        }

        var result = await permissionService.GetUserPermissionsAsync(info.UserId);
        MinimalPermission[] powers = [.. result.Payload];
        info.UserPowers = [.. powers.Where(p => p.PermissionType != PermissionType.Page).Select(p => p.PermissionId)];
        info.UserPages = [.. powers.Where(p => p.PermissionType == PermissionType.Page).Select(p => p.PermissionId)];
        return result.Payload;
    }

    /// <summary>
    /// 在路由更改时调用，用于执行自定义逻辑
    /// </summary>
    /// <param name="route">即将访问的路由</param>
    /// <returns>是否允许更改路由</returns>
    public virtual Task<bool> RouterChangingAsync(TagRoute route) => Task.FromResult(true);

    /// <summary>
    /// 过滤路由元数据，决定是否显示路由
    /// </summary>
    /// <param name="meta">路由元数据</param>
    /// <returns>是否允许显示路由</returns>
    public virtual Task<bool> RouteMetaFilterAsync(RouterMeta meta)
    {
        //if (AppSetting.CurrentValue.LoadPageFromDatabase)
        //{
        //    var has = Array.IndexOf(UserStore.UserInfo?.UserPages ?? [], meta.RouteId) > -1;
        //    return Task.FromResult(has);
        //}
        //else
        //{
        //    return Task.FromResult(true);
        //}
        return Task.FromResult(true);
    }
}
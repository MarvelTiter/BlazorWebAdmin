﻿using Microsoft.Extensions.DependencyInjection;
using Project.Constraints.Store.Models;

namespace Project.Web.Shared;

/// <summary>
/// 提供基本设置功能的服务类，实现自定义用户信息和权限处理
/// </summary>
public class BasicSetting : IProjectSettingService//, IDisposable
{
    /// <summary>
    /// 服务提供者，用于解析其他服务
    /// </summary>
    private readonly IServiceProvider services;

    /// <summary>
    /// 当前用户信息
    /// </summary>
    protected UserInfo? CurrentUser { get; set; }

    /// <summary>
    /// 构造函数，接收服务提供者
    /// </summary>
    /// <param name="services">服务提供者</param>
    public BasicSetting(IServiceProvider services)
    {
        this.services = services;
    }

    /// <summary>
    /// 在登录成功后调用此方法以设置当前用户信息
    /// </summary>
    /// <param name="result">登录成功的用户信息</param>
    /// <returns>完成任务</returns>
    public virtual Task LoginSuccessAsync(UserInfo result)
    {
        CurrentUser = result;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 登录拦截器，用于在登录后执行自定义逻辑
    /// </summary>
    /// <param name="result">登录成功的用户信息</param>
    /// <returns>查询结果任务</returns>
    public virtual Task<QueryResult> LoginInterceptorAsync(UserInfo result)
    {
        return Task.FromResult(QueryResult.Success());
    }

    /// <summary>
    /// 在Web应用程序被访问后调用，用于执行自定义逻辑
    /// </summary>
    /// <returns>完成任务</returns>
    public virtual Task AfterWebApplicationAccessed()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取用户的权限
    /// </summary>
    /// <param name="info">用户信息</param>
    /// <returns>用户权限的集合</returns>
    /// <exception cref="NotImplementedException">如果未实现权限服务</exception>
    public virtual async Task<IEnumerable<MinimalPower>> GetUserPowersAsync(UserInfo info)
    {
        var permissionService = services.GetService<IPermissionService>();
        if (permissionService == null)
        {
            return [];
        }
        var result = await permissionService.GetUserPowersAsync(info.UserId);
        return result.Payload;
    }

    /// <summary>
    /// 在路由更改时调用，用于执行自定义逻辑
    /// </summary>
    /// <param name="route">即将访问的路由</param>
    /// <returns>是否允许更改路由</returns>
    public virtual Task<bool> RouterChangingAsync(TagRoute route)
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// 过滤路由元数据，决定是否显示路由
    /// </summary>
    /// <param name="meta">路由元数据</param>
    /// <returns>是否允许显示路由</returns>
    public virtual Task<bool> RouteMetaFilterAsync(RouterMeta meta)
    {
        return Task.FromResult(true);
    }
}

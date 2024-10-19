using Microsoft.Extensions.DependencyInjection;
using Project.Constraints.Store.Models;

namespace Project.Web.Shared;

public class BasicSetting : IProjectSettingService//, IDisposable
{
    private readonly IServiceProvider services;

    protected UserInfo? CurrentUser { get; set; }

    //public abstract Task<QueryResult<UserInfo>> GetUserInfoAsync(string username, string password);
    //public abstract Task<int> UpdateLoginInfo(UserInfo info);
    public BasicSetting(IServiceProvider services)
    {
        this.services = services;
    }
    public virtual Task LoginSuccessAsync(UserInfo result)
    {
        CurrentUser = result;
        return Task.CompletedTask;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public virtual Task<QueryResult> LoginInterceptorAsync(UserInfo result)
    {
        return Task.FromResult(QueryResult.Success());
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual Task AfterWebApplicationAccessed()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="route"></param>
    /// <returns></returns>
    public virtual Task<bool> RouterChangingAsync(TagRoute route)
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="meta"></param>
    /// <returns></returns>
    public virtual Task<bool> RouteMetaFilterAsync(RouterMeta meta)
    {
        return Task.FromResult(true);
    }
}

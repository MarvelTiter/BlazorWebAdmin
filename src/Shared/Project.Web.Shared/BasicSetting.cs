using Project.Constraints.Store.Models;

namespace Project.Web.Shared;

public class BasicSetting : IProjectSettingService//, IDisposable
{
    protected UserInfo? CurrentUser { get; set; }

    //public abstract Task<QueryResult<UserInfo>> GetUserInfoAsync(string username, string password);
    //public abstract Task<int> UpdateLoginInfo(UserInfo info);
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
        return Task.FromResult(Result.Success());
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
    public virtual Task<IEnumerable<MinimalPower>> GetUserPowersAsync(UserInfo info) => Task.FromResult<IEnumerable<MinimalPower>>([]);

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

using BlazorTemplate.ClientCore.Store.Models;

namespace BlazorTemplate.ClientCore.Services;

public interface IAddtionalInterceptor
{
    public Task LoginSuccessAsync(UserInfo result) => Task.CompletedTask;
    public Task<bool> RouterChangingAsync(RouteMeta route) => Task.FromResult(true);
    public Task<bool> RouteMetaFilterAsync(RouteMeta meta) => Task.FromResult(true);
    public Task AfterWebApplicationAccessedAsync() => Task.CompletedTask;
}
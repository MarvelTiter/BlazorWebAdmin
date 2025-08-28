using Project.Constraints.Store.Models;

namespace Project.Constraints.Services;

public interface IAddtionalInterceptor
{
    public Task LoginSuccessAsync(UserInfo result) => Task.CompletedTask;
    public Task<bool> RouterChangingAsync(RouteTag route) => Task.FromResult(true);
    public Task<bool> RouteMetaFilterAsync(RouteMeta meta) => Task.FromResult(true);
    public Task AfterWebApplicationAccessedAsync() => Task.CompletedTask;
}
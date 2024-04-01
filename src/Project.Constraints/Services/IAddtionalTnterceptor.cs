using Project.Constraints.Store.Models;

namespace Project.Constraints.Services
{
    public interface IAddtionalInterceptor
    {
        public Task LoginSuccessAsync(UserInfo result) => Task.CompletedTask;
        public Task<bool> RouterChangingAsync(TagRoute route) => Task.FromResult(true);
        public Task<bool> RouteMetaFilterAsync(RouterMeta meta) => Task.FromResult(true);
        public Task AfterWebApplicationAccessedAsync() => Task.CompletedTask;
    }
}

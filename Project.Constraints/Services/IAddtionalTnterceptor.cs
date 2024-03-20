using Project.Constraints.Store.Models;

namespace Project.Constraints.Services
{
    public interface IAddtionalTnterceptor
    {
        public Task LoginSuccessAsync(IQueryResult<UserInfo> result)
        {
            return Task.CompletedTask;
        }

        public Task RouterChangingAsync(TagRoute route)
        {
            return Task.CompletedTask;
        }
    }
}

using Project.Constraints.Store.Models;

namespace Project.Constraints.Store
{
    [AutoInject]
    public interface IProtectedLocalStorage
    {
        ValueTask<StorageResult<TValue>> GetAsync<TValue>(string key);
        ValueTask SetAsync(string key, object value);
        ValueTask DeleteAsync(string key);
    }
}

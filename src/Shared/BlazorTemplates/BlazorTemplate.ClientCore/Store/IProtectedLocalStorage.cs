using BlazorTemplate.ClientCore.Store.Models;

namespace BlazorTemplate.ClientCore.Store;

public interface IProtectedLocalStorage
{
    ValueTask<StorageResult<TValue>> GetAsync<TValue>(string key);
    ValueTask Clear();
    ValueTask SetAsync(string key, object value);
    ValueTask DeleteAsync(string key);
}
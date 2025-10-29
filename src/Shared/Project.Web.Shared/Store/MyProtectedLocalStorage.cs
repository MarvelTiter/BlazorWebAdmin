using Microsoft.JSInterop;
using Project.Constraints.Store.Models;
using System.Text;
using System.Text.Json;

namespace Project.Web.Shared.Store;

/// <summary>
/// Constructs an instance of <see cref="ProtectedBrowserStorage"/>.
/// </summary>
/// <param name="storeName">The name of the store in which the data should be stored.</param>
/// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
[AutoInject]
/// <summary>
/// https://source.dot.net/#Microsoft.AspNetCore.Components.Server/ProtectedBrowserStorage/ProtectedLocalStorage.cs,2ced327c1fc4a5f4
/// </summary>
public class MyProtectedLocalStorage(IJSRuntime jsRuntime, ILogger<MyProtectedLocalStorage> logger) : IProtectedLocalStorage
{

    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    private readonly IJSRuntime _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));

    //public ValueTask SetAsync(string key, object value)
    //    => SetAsync(CreatePurposeFromKey(key), key, value);

    public ValueTask SetAsync(string key, object value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        return SetProtectedJsonAsync(key, Protect(value));
    }

    //public ValueTask<StorageResult<TValue>> GetAsync<TValue>(string key)
    //    => GetAsync<TValue>(key);

    public async ValueTask<StorageResult<TValue>> GetAsync<TValue>(string key)
    {
        try
        {
            var protectedJson = await GetProtectedJsonAsync(key);
            return protectedJson == null ?
                new StorageResult<TValue>(false, default) :
                new StorageResult<TValue>(true, Unprotect<TValue>(protectedJson));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Messgae}", ex.Message);
            return new(false, default);
        }
    }

    public ValueTask DeleteAsync(string key) => _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);

    private static string Protect(object value)
    {
        var json = JsonSerializer.Serialize(value, options: Options);
        //var protector = GetOrCreateCachedProtector(purpose);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        //return protector.Protect(json);
    }

    private static TValue Unprotect<TValue>(string protectedJson)
    {
        //var protector = GetOrCreateCachedProtector(purpose);
        //var json = protector.Unprotect(protectedJson);
        var bytes = Convert.FromBase64String(protectedJson);
        var json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<TValue>(json, options: Options)!;


    }

    private ValueTask SetProtectedJsonAsync(string key, string protectedJson)
        => _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, protectedJson);

    private ValueTask<string?> GetProtectedJsonAsync(string key)
        => _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);

    public ValueTask Clear() => _jsRuntime.InvokeVoidAsync("localStorage.clear");

    //private IDataProtector GetOrCreateCachedProtector(string purpose)
    //    => _cachedDataProtectorsByPurpose.GetOrAdd(
    //        purpose,
    //        _dataProtectionProvider.CreateProtector);

    //private string CreatePurposeFromKey(string key)
    //    => $"{GetType().FullName}:localStorage:{key}";
}
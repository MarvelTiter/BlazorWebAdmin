using AutoInjectGenerator;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.JSInterop;
using Project.Constraints.Store.Models;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace Project.Constraints.Store
{
    [AutoInject]
    /// <summary>
    /// https://source.dot.net/#Microsoft.AspNetCore.Components.Server/ProtectedBrowserStorage/ProtectedLocalStorage.cs,2ced327c1fc4a5f4
    /// </summary>
    public class MyProtectedLocalStorage : IProtectedLocalStorage
    {

        public static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };

        private readonly IJSRuntime _jsRuntime;
        //private readonly IDataProtectionProvider _dataProtectionProvider;
        //private readonly ConcurrentDictionary<string, IDataProtector> _cachedDataProtectorsByPurpose
        //    = new ConcurrentDictionary<string, IDataProtector>(StringComparer.Ordinal);

        /// <summary>
        /// Constructs an instance of <see cref="ProtectedBrowserStorage"/>.
        /// </summary>
        /// <param name="storeName">The name of the store in which the data should be stored.</param>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
        public MyProtectedLocalStorage(IJSRuntime jsRuntime)
        {
            //// Performing data protection on the client would give users a false sense of security, so we'll prevent this.
            //if (OperatingSystem.IsBrowser())
            //{
            //    throw new PlatformNotSupportedException($"{GetType()} cannot be used when running in a browser.");
            //}

            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
            //_dataProtectionProvider = dataProtectionProvider ?? throw new ArgumentNullException(nameof(dataProtectionProvider));
        }

        //public ValueTask SetAsync(string key, object value)
        //    => SetAsync(CreatePurposeFromKey(key), key, value);

        public ValueTask SetAsync(string key, object value)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            return SetProtectedJsonAsync(key, Protect(value));
        }

        //public ValueTask<StorageResult<TValue>> GetAsync<TValue>(string key)
        //    => GetAsync<TValue>(key);

        public async ValueTask<StorageResult<TValue>> GetAsync<TValue>( string key)
        {
            var protectedJson = await GetProtectedJsonAsync(key);

            return protectedJson == null ?
                new StorageResult<TValue>(false, default) :
                new StorageResult<TValue>(true, Unprotect<TValue>( protectedJson));
        }

        public ValueTask DeleteAsync(string key) => _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);

        private string Protect(object value)
        {
            var json = JsonSerializer.Serialize(value, options: Options);
            //var protector = GetOrCreateCachedProtector(purpose);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            //return protector.Protect(json);
        }

        private TValue Unprotect<TValue>(string protectedJson)
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
}

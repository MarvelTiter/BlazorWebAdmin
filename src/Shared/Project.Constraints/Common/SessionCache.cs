using System.Collections.Concurrent;

namespace Project.Constraints.Common;

//[AutoInject]
//public class SessionCache<T>
//{
//    readonly ConcurrentDictionary<string, T> caches = [];
//    public T GetOrAdd(string key, Func<T> func)
//    {
//        return caches.GetOrAdd(key, func.Invoke());
//    }

//    public T? GetCache(string key)
//    {
//        if (caches.TryGetValue(key, out var cache)) { return cache; }
//        return default;
//    }

//    public void AddCache(string key, T value)
//    {
//        _ = caches.GetOrAdd(key, value);
//    }

//    public static T GetOrAddStatic(string key, Func<T> func)
//    {
//        return StaticCache<T>.GetOrAdd(key, func);
//    }

//    public static T GetCacheStatic(string key)
//    {
//        return StaticCache<T>.GetCache(key);
//    }


//}
using System.Collections.Concurrent;

namespace Project.Constraints.Common;

public static class StaticCache<T>
{
    static readonly ConcurrentDictionary<string, T> caches = new();

    public static T GetCache(string key)
    {
        if (caches.TryGetValue(key, out var cache)) { return cache; }
        return default!;
    }

    public static void AddOrUpdate(string key, T value, Func<string, T, T>? update = null)
    {
        update ??= (k, v) => value;
        caches.AddOrUpdate(key, value, update);
    }

    public static T GetOrAdd(string key, Func<T> func)
    {
        return caches.GetOrAdd(key, func.Invoke());
    }
}

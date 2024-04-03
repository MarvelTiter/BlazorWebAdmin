using System.Collections.Concurrent;

namespace Project.Constraints.Common
{
    public static class StaticCache<T>
    {
        static readonly ConcurrentDictionary<string, T> caches = new();

        public static T GetOrAdd(string key, Func<T> func)
        {
            return caches.GetOrAdd(key, func.Invoke());
        }
    }
}

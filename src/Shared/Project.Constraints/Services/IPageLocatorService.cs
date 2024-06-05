using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Services
{
    public interface IPageLocatorService
    {
        void SetPage<T>(string key);
        void SetPage(string key, Type type);
        Type? GetPage(string key);
    }

    public class PageLocatorService : IPageLocatorService
    {
        private readonly ConcurrentDictionary<string, Type> pages = new();

        public Type? GetPage(string key)
        {
            if (pages.TryGetValue(key, out var type)) return type;
            return null;
        }

        public void SetPage<T>(string key)
        {
            SetPage(key, typeof(T));
        }

        public void SetPage(string key, Type type)
        {
            pages.TryRemove(key, out var _);
            pages.TryAdd(key, type);
        }
    }
}

using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;

namespace BlazorWebAdmin.Components
{
    internal static class SvgIconDataCache
    {
        private static readonly ConcurrentDictionary<string, string> _cache = new();
        public static async Task<string> GetIconDataByName(string name)
        {
            if (!_cache.TryGetValue(name, out var result))
            {
                result = await ReadIconData(name);
                _cache[name] = result; // Safe race - doesn't matter if it overwrites
            }
            return result;
        }

        private static async Task<string> ReadIconData(string name)
        {
            var iconFile = $"wwwroot/icons/{name}.svg";
            if (File.Exists(iconFile))
            {
                var svgContent = await File.ReadAllTextAsync(iconFile);
                return svgContent;
            }
            else
            {
                return "";
            }
        }
    }
}

using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;

namespace BlazorWeb.Shared.Components
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
                //FileInfo f = new FileInfo(iconFile);
                //var files = Directory.EnumerateFiles($"./_content", "*.svg", SearchOption.AllDirectories);

                var svgContent = await File.ReadAllTextAsync(iconFile);
                return svgContent;
            }
            else
            {
                return "";
            }
        }
        static bool loaded;
        public static async Task LoadAllIcons()
        {
            if (loaded) return;
            var files = Directory.EnumerateFiles($"wwwroot/icons", "*.svg", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                var name = Path.GetFileNameWithoutExtension(f);
                _ = await GetIconDataByName(name);
            }
            loaded = true;
        }

        public static List<string> GetIcons()
        {
            return _cache.Keys.ToList();
        }
    }
}

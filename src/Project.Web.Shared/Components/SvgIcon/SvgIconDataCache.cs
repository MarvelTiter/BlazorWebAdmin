using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;

namespace Project.Web.Shared.Components
{
    internal static class SvgIconDataCache
    {
        private static readonly ConcurrentDictionary<string, string> _contentCache = new();
        private static readonly ConcurrentDictionary<string, string> _nameCache = new();
        public static async Task<string> GetIconDataByName(string name)
        {
            if (!_contentCache.TryGetValue(name, out var result))
            {
                result = await ReadIconData(name);
            }
            return result;
        }

        private static async Task<string> ReadIconData(string name)
        {
            if (!_nameCache.TryGetValue(name, out var iconFile))
            {
                if (!loaded)
                {
                    LoadAllIcons();
                    return await ReadIconData(name);
                }
            }
            if (iconFile != null)
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
        public static void LoadAllIcons()
        {
            if (loaded) return;
            var path = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
            path = new DirectoryInfo(path).Parent!.Parent!.Parent!.Parent!.FullName;
#endif
            var files = Directory.EnumerateFiles(path, "*.svg", SearchOption.AllDirectories).Where(f => f.Contains("wwwroot") && !f.Contains("Release"));
            foreach (var f in files)
            {
                var name = Path.GetFileNameWithoutExtension(f);
                _nameCache.TryAdd(name, f);
            }
            loaded = true;
        }

        public static List<string> GetIcons()
        {
            LoadAllIcons();
            return _nameCache.Keys.ToList();
        }
    }
}

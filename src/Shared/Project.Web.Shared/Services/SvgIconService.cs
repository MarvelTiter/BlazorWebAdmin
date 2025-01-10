using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Project.Web.Shared.Services
{
    [AutoInject(Group = "SERVER", LifeTime = InjectLifeTime.Singleton)]
    public class SvgIconService : ISvgIconService
    {
        private readonly IHostEnvironment environment;
        private static readonly ConcurrentDictionary<string, string> _contentCache = new();
        private static readonly ConcurrentDictionary<string, string> _nameCache = new();
        public SvgIconService(IHostEnvironment environment)
        {
            this.environment = environment;
        }
        public Task<QueryCollectionResult<string>> GetAllIcon()
        {
            var names = _nameCache.Keys;
            return Task.FromResult(names.CollectionResult());
        }
        public async Task<QueryResult<string>> GetIcon(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }
            if (!_contentCache.TryGetValue(name, out var result))
            {
                result = await ReadIconData(name, environment);
                _contentCache.TryAdd(name, result);
            }
            return result;
        }


        static bool loaded;

        //private static string IconFullName(string name) => name.StartsWith(AppConst.CUSTOM_SVG_PREFIX) ? name : $"{AppConst.CUSTOM_SVG_PREFIX}{name}";

        private static async Task<string> ReadIconData(string name, IHostEnvironment environment)
        {
            if (!_nameCache.TryGetValue(name, out var iconFile))
            {
                if (!loaded)
                {
                    LoadAllIcons(environment);
                    return await ReadIconData(name, environment);
                }
            }
            if (iconFile != null)
            {
                var svgContent = await File.ReadAllTextAsync(iconFile);
                return svgContent;
            }
            else
            {
                return "";
            }
        }

        private static void LoadAllIcons(IHostEnvironment environment)
        {
            if (loaded) return;
            var path = AppDomain.CurrentDomain.BaseDirectory;
            if (environment.IsDevelopment())
            {
                path = new DirectoryInfo(path).Parent!.Parent!.Parent!.Parent!.FullName;
            }
            var files = Directory.EnumerateFiles(path, "*.svg", SearchOption.AllDirectories).Where(f => f.Contains("wwwroot"));
            //var files = Directory.EnumerateFiles(path, "*.svg", SearchOption.AllDirectories).Where(f => f.Contains("SvgAssets", StringComparison.CurrentCultureIgnoreCase));
            foreach (var f in files)
            {
                var name = Path.GetFileNameWithoutExtension(f);
                if (!name.StartsWith(AppConst.CUSTOM_SVG_PREFIX)) continue;
                _nameCache.TryAdd(name, f);
            }
            loaded = true;
        }
    }
}

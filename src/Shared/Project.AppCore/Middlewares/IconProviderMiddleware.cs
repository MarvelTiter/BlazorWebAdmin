using AutoInjectGenerator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Project.Constraints;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Project.AppCore.Middlewares
{
    [AutoInject(ServiceType = typeof(IconProviderMiddleware), LifeTime = InjectLifeTime.Singleton)]
    public class IconProviderMiddleware : IMiddleware
    {
        private readonly IHostEnvironment environment;
        private static readonly ConcurrentDictionary<string, string> _contentCache = new();
        private static readonly ConcurrentDictionary<string, string> _nameCache = new();
        public IconProviderMiddleware(IHostEnvironment environment)
        {
            this.environment = environment;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path;
            var name = Path.GetFileNameWithoutExtension(path);
            if (!_contentCache.TryGetValue(name, out var result))
            {
                result = await ReadIconData(name, environment);
                _contentCache.TryAdd(name, result);
            }
            await context.Response.WriteAsync(result);
        }
        static bool loaded;

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
                _nameCache.TryAdd(name, f);
            }
            loaded = true;
        }
    }
}

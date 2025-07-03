using Project.Constraints.Common.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Project.Web.Shared.Components.Camera;

namespace Project.Web.Shared.Locales.EmbeddedJson;

internal class EmbeddedJsonLocalizerFactory : IStringLocalizerFactory
{
    private static readonly ConcurrentDictionary<string, IStringLocalizer> caches = [];
    private readonly ConcurrentDictionary<string, string> AllJsons = [];
    public EmbeddedJsonLocalizerFactory()
    {
        var all = Assembly.GetEntryAssembly()?.GetReferencedAssemblies().Select(Assembly.Load).SelectMany(LoadJson);
        foreach (var (Name, Content) in all ?? [])
        {
            AllJsons[Name] = Content;
        }
    }

    IEnumerable<(string Name, string Content)> LoadJson(Assembly a)
    {
        var files = a.GetManifestResourceNames().Where(f => f.Contains("Langs") && f.EndsWith(".json"));
        var assemblyName = a.GetName().Name!;
        foreach (var item in files)
        {
            using var stream = a.GetManifestResourceStream(item);
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();
                yield return (item[item.IndexOf("Langs")..], content);
            }
        }
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        var cultureInfo = CultureInfo.CurrentUICulture;
        var typeInfo = resourceSource.GetTypeInfo();
        return CreateLocalizer(FindLangName(resourceSource) ?? typeInfo.Name, cultureInfo);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        throw new NotImplementedException();
    }

    private IStringLocalizer CreateLocalizer(string resourceName, CultureInfo cultureInfo)
    {
        var cacheKey = GetCacheKey(resourceName, cultureInfo);
        return caches.GetOrAdd(cacheKey, new EmbeddedJsonLocalizer(resourceName, AllJsons, cultureInfo));
    }

    private static string GetCacheKey(string resourceName, CultureInfo cultureInfo) => resourceName + ';' + cultureInfo.Name;

    private static string? FindLangName(Type resource)
    {
        var a = resource.GetCustomAttribute<LangNameAttribute>() ?? resource.GetInterfaces().Select(t => t.GetCustomAttribute<LangNameAttribute>()).Where(t => t is not null).FirstOrDefault();
        return a?.Name;
    }
}
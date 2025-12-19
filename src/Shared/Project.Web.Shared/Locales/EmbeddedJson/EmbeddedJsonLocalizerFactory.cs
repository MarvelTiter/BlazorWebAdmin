using Project.Constraints.Common.Attributes;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace Project.Web.Shared.Locales.EmbeddedJson;

internal class LocalizerItems(string name, ILogger logger)
{
    public string Name { get; } = name;
    public void TryParseItems(Stream? stream)
    {
        if (stream is null) return;
        using var json = JsonDocument.Parse(stream);
        var root = json.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            return;
        }
        ScanJsonProperties(root, "");

        void ScanJsonProperties(JsonElement rootElement, string prefix)
        {
            foreach (var item in rootElement.EnumerateObject())
            {
                if (item.Value.ValueKind == JsonValueKind.Object)
                {
                    ScanJsonProperties(item.Value, $"{prefix}{item.Name}.");
                    continue;
                }
                if (item.Value.ValueKind != JsonValueKind.String)
                {
                    continue;
                }
                var value = item.Value.GetString();
                if (value is not null)
                {
                    var key = $"{prefix}{item.Name}";
                    if (!items.TryAdd(key, value))
                    {
                        logger.LogWarning("{key}已存在, 添加失败", key);
                    }
                    //items.TryAdd(key, value)
                }
            }
        }
    }

    private readonly ConcurrentDictionary<string, string> items = [];
    public bool TryGetValue(string key, string typeName, [NotNullWhen(true)] out string? value)
    {
        return items.TryGetValue(key, out value);
    }
}

internal class EmbeddedJsonLocalizerFactory : IStringLocalizerFactory
{
    private static readonly ConcurrentDictionary<string, IStringLocalizer> caches = [];
    private readonly ConcurrentDictionary<string, string> AllJsons = [];
    private readonly ConcurrentDictionary<string, LocalizerItems> items = [];
    private readonly ILogger<EmbeddedJsonLocalizerFactory> logger;

    public EmbeddedJsonLocalizerFactory(ILogger<EmbeddedJsonLocalizerFactory> logger)
    {
        var entry = Assembly.GetEntryAssembly();
        //var all = entry?.GetReferencedAssemblies().Concat([entry.GetName()]).Select(Assembly.Load).SelectMany(LoadJson);
        //foreach (var (Name, Content) in all ?? [])
        //{
        //    AllJsons[Name] = Content;
        //}
        var all = entry?.GetReferencedAssemblies().Concat([entry.GetName()]).Select(Assembly.Load);
        foreach (var a in all ?? [])
        {
            FlattenJson(a);
        }

        this.logger = logger;
    }
    void FlattenJson(Assembly a)
    {
        var files = a.GetManifestResourceNames().Where(f => f.Contains("Langs") && f.EndsWith(".json")).ToArray();
        var assemblyName = a.GetName().Name!;
        foreach (var file in files)
        {
            using var stream = a.GetManifestResourceStream(file);
            var name = file[file.IndexOf("Langs")..];
            if (!items.TryGetValue(name, out var value))
            {
                value = new(name, logger);
                items[name] = value;
            }
            value.TryParseItems(stream);
        }
    }
    //IEnumerable<(string Name, string Content)> LoadJson(Assembly a)
    //{
    //    var files = a.GetManifestResourceNames().Where(f => f.Contains("Langs") && f.EndsWith(".json")).ToArray();
    //    var assemblyName = a.GetName().Name!;
    //    foreach (var item in files)
    //    {
    //        using var stream = a.GetManifestResourceStream(item);
    //        var name = item[item.IndexOf("Langs")..];
    //        if (!items.TryGetValue(name, out var value))
    //        {
    //            value = new(name);
    //            items[name] = value;
    //        }
    //        value.TryParseItems(stream);
    //        if (stream != null)
    //        {
    //            using var reader = new StreamReader(stream);
    //            var content = reader.ReadToEnd();
    //            yield return (name, content);
    //        }
    //    }
    //}

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
        Console.WriteLine($"IStringLocalizer -> {resourceName}");
        return caches.GetOrAdd(cacheKey, new EmbeddedJsonLocalizerOld(resourceName, AllJsons, cultureInfo));
    }

    private static string GetCacheKey(string resourceName, CultureInfo cultureInfo) => resourceName + ';' + cultureInfo.Name;

    private static string? FindLangName(Type resource)
    {
        var a = resource.GetCustomAttribute<LangNameAttribute>() ?? resource.GetInterfaces().Select(t => t.GetCustomAttribute<LangNameAttribute>()).Where(t => t is not null).FirstOrDefault();
        return a?.Name;
    }
}
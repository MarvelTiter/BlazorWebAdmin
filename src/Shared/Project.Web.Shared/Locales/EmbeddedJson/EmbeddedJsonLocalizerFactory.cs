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
        // 尝试1：直接查找传入的key
        if (items.TryGetValue(key, out value))
            return true;

        // 如果typeName为空，直接返回失败
        if (string.IsNullOrEmpty(typeName))
        {
            value = null;
            return false;
        }

        // 检查key是否已经是完整路径
        bool hasTypeNamePrefix = key.StartsWith(typeName + ".", StringComparison.OrdinalIgnoreCase);
        bool hasDot = key.Contains('.');

        // 尝试2：如果key有typeName前缀，去掉前缀查找
        if (hasTypeNamePrefix)
        {
            var keyWithoutPrefix = key[(typeName.Length + 1)..];
            if (items.TryGetValue(keyWithoutPrefix, out value))
                return true;
        }
        if (!hasDot && !hasTypeNamePrefix)
        {
            var keyWithPrefix = typeName + "." + key;
            if (items.TryGetValue(keyWithPrefix, out value))
                return true;
        }

        // 尝试4：通配符查找（最后的手段）
        // 查找所有以key结尾的键
        foreach (var kvp in items)
        {
            if (kvp.Key.EndsWith("." + key, StringComparison.OrdinalIgnoreCase))
            {
                value = kvp.Value;
                return true;
            }
        }

        value = null;
        return false;
    }
}

internal class EmbeddedJsonLocalizerFactory : IStringLocalizerFactory
{
    private static readonly ConcurrentDictionary<string, IStringLocalizer> caches = [];
    //private readonly ConcurrentDictionary<string, string> AllJsons = [];
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

    //private static bool IsValidCultureInfo(string cultureName)
    //{
    //    if (string.IsNullOrWhiteSpace(cultureName))
    //        return false;

    //    try
    //    {
    //        var culture = CultureInfo.GetCultureInfo(cultureName);
    //        return culture != null;
    //    }
    //    catch (CultureNotFoundException)
    //    {
    //        return false;
    //    }
    //}

    void FlattenJson(Assembly a)
    {
        var files = a.GetManifestResourceNames().Where(f => f.Contains("Langs") && f.EndsWith(".json")).ToArray();
        var assemblyName = a.GetName().Name!;
        foreach (var file in files)
        {
            using var stream = a.GetManifestResourceStream(file);
            // 去头掐尾，文件名保留 CultureName + resourceName
            var name = file[(file.IndexOf("Langs") + 6)..^5];
            if (name.EndsWith(".index"))
            {
                name = name.Replace(".index", "");
            }
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
        //Console.WriteLine($"IStringLocalizer -> {resourceName}");
        return caches.GetOrAdd(cacheKey, k =>
        {
            LocalizerItems? specific = null;
            var cultureName = cultureInfo.Name.Replace('-', '_');
            items.TryGetValue(cultureName, out var fallback);
            if (resourceName != "Object")
            {
                items.TryGetValue($"{cultureName}.{resourceName}", out specific);
            }

            return new EmbeddedJsonLocalizer(resourceName, specific, fallback);
        });

    }

    private static string GetCacheKey(string resourceName, CultureInfo cultureInfo) => resourceName + ';' + cultureInfo.Name;

    private static string? FindLangName(Type resource)
    {
        var a = resource.GetCustomAttribute<LangNameAttribute>() ?? resource.GetInterfaces().Select(t => t.GetCustomAttribute<LangNameAttribute>()).Where(t => t is not null).FirstOrDefault();
        return a?.Name;
    }
}
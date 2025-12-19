using Microsoft.Extensions.Logging;
using Project.Web.Shared.Locales.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Web.Shared.Locales.EmbeddedJson;

class InteractiveLocalizer<T> : IStringLocalizer<T>
{
    private readonly IStringLocalizerFactory factory;
    private readonly ILanguageService languageService;
    private IStringLocalizer localizer;
    public InteractiveLocalizer(IStringLocalizerFactory factory, ILanguageService languageService)
    {
        this.factory = factory;
        this.languageService = languageService;
        this.languageService.LanguageChanged += LanguageService_LanguageChanged;
        localizer = factory.Create(typeof(T));
    }

    private void LanguageService_LanguageChanged(CultureInfo obj)
    {
        localizer = factory.Create(typeof(T));
    }

    public LocalizedString this[string name] => localizer[name];

    public LocalizedString this[string name, params object[] arguments] => localizer[name, arguments];

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];
}

internal class EmbeddedJsonLocalizerOld : IStringLocalizer
{
    private readonly ConcurrentDictionary<string, JsonDocument?> documentCache = new();
    private static readonly ConcurrentDictionary<string, JsonDocument> allJsonFiles = new();
    private static readonly ConcurrentDictionary<string, JsonDocument> fallbackJsonFiles = new();
    private readonly ConcurrentDictionary<string, JsonInfo> infos = new();
    private readonly string typedName;
    private readonly ConcurrentDictionary<string, string> allJsonFile;
    private readonly CultureInfo cultureInfo;
    private string? searchedLocation;
    public LocalizedString this[string name]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(name);
            var value = GetStringSafely(name);
            return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: searchedLocation);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(name);
            var format = GetStringSafely(name);
            var value = string.Format(format ?? name, arguments);
            return new LocalizedString(name, value ?? name, resourceNotFound: format == null, searchedLocation: searchedLocation);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];

    public EmbeddedJsonLocalizerOld(string resourceName, ConcurrentDictionary<string, string> allJsonFile, CultureInfo cultureInfo)
    {
        this.typedName = resourceName;
        this.allJsonFile = allJsonFile;
        this.cultureInfo = cultureInfo;
    }
    private string? GetStringSafely(string name)
    {
        var info = GetJsonDocument(CultureInfo.CurrentUICulture.Name.Replace('-', '_'));
        if (!info.TryGetValueFromMain(name, typedName, out var value))
        {
            info.TryGetValueFromFallback(name, typedName, out value);
        }
        return value;
    }


    private JsonInfo GetJsonDocument(string culture)
    {
        if (!infos.TryGetValue(culture, out var info))
        {
            var useTypedName = false;
            var fallback = GetFallbackJsonDocument(culture);
            var doc = documentCache.GetOrAdd(culture, lang =>
            {
                JsonDocument? value = null;
                // Langs/zh-CN/resourceName/index.json
                if (CheckFileExits(true, true, true, lang, out var path))
                {
                    LoadJsonDocumentFromPath(path, out value);
                }
                // Langs/zh-CN/resourceName.json
                else if (CheckFileExits(true, false, true, lang, out path))
                {
                    LoadJsonDocumentFromPath(path, out value);
                }
                // Langs/resourceName/zh-CN.json
                else if (CheckFileExits(false, true, true, lang, out path))
                {
                    LoadJsonDocumentFromPath(path, out value);
                }
                // Langs/resourceName.zh-CN.json
                else if (CheckFileExits(false, false, true, lang, out path))
                {
                    LoadJsonDocumentFromPath(path, out value);
                }
                useTypedName = value != null;
                return value;
            });
            info = new JsonInfo
            {
                Fallback = fallback,
                Main = doc,
                UseTypedName = typedName != "Object",
                SearchedLocation = searchedLocation ?? ""
            };
            infos[culture] = info;
        }
        return info;
    }

    private string ConstructJsonFilePath(bool useCultureFolder, bool useTypedFolder, bool useTypedName, string culture)
    {
        var paths = new List<string>() { "Langs" };
        if (useCultureFolder)
            paths.Add(culture);
        if (useTypedFolder)
            paths.Add(typedName);

        if (useCultureFolder)
        {
            if (useTypedName)
            {
                if (useTypedFolder)
                {
                    paths.Add("index.json");
                }
                else
                {
                    paths.Add($"{typedName}.json");
                }
            }
            else
            {
                paths.Add("index.json");
            }
        }
        else
        {
            if (useTypedName)
            {
                if (useTypedFolder)
                {
                    paths.Add($"{culture}.json");
                }
                else
                {
                    paths.Add($"{typedName}.{culture}.json");
                }
            }
            else
            {
                paths.Add($"{culture}.json");
            }
        }

        return string.Join(".", paths);
    }

    private void LoadJsonDocumentFromPath(string path, out JsonDocument? value)
    {
        searchedLocation = path;
        if (allJsonFiles.TryGetValue(searchedLocation, out value))
        {
            return;
        }
        allJsonFile.TryGetValue(searchedLocation, out var content);
        if (!string.IsNullOrWhiteSpace(content))
        {
            try
            {
                value = JsonDocument.Parse(content);
                allJsonFiles.TryAdd(searchedLocation, value);
                return;
            }
            catch
            {
            }
        }
        value = null;
    }

    private bool CheckFileExits(bool useCultureFolder, bool useTypedFolder, bool useTypedName, string culture, out string path)
    {
        path = ConstructJsonFilePath(useCultureFolder, useTypedFolder, useTypedName, culture);
        return allJsonFile.ContainsKey(path);
    }

    private JsonDocument? GetFallbackJsonDocument(string culture)
    {
        if (!fallbackJsonFiles.TryGetValue(culture, out var fallback))
        {
            // Langs/zh-CN/index.json
            if (CheckFileExits(true, false, false, culture, out var path))
            {
                LoadJsonDocumentFromPath(path, out fallback);
            }
            // Langs/zh-CN.json
            else if (CheckFileExits(false, false, false, culture, out path))
            {
                LoadJsonDocumentFromPath(path, out fallback);
            }

            if (fallback != null && !fallbackJsonFiles.ContainsKey(culture))
                fallbackJsonFiles.TryAdd(culture, fallback);
        }
        return fallback;
    }
}

internal class EmbeddedJsonLocalizer(string resourceName, LocalizerItems? specific, LocalizerItems fallback) : IStringLocalizer
{
    private readonly ConcurrentDictionary<string, string> caches = [];
    private readonly ConcurrentDictionary<string, bool> missingItems = [];
    public LocalizedString this[string name]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(name);
            var value = GetStringSafely(name);
            return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: resourceName);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(name);
            var format = GetStringSafely(name);
            var value = string.Format(format ?? name, arguments);
            return new LocalizedString(name, value ?? name, resourceNotFound: format == null, searchedLocation: resourceName);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];

    private string GetStringSafely(string name)
    {
        if (caches.TryGetValue(name, out var cachedValue))
        {
            return cachedValue;
        }
        if (missingItems.ContainsKey(name))
        {
            return name;
        }
        return specific?.TryGetValue(name, out var value) == true
            ? value
            : fallback.TryGetValue(name, out value) ? value : name;
    }
}
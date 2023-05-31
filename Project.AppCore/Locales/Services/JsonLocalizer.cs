using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Project.AppCore.Locales.Services
{
    public class JsonLocalizer : IStringLocalizer
    {
        private readonly ConcurrentDictionary<string, JsonDocument> documentCache = new();
        private static readonly ConcurrentDictionary<string, JsonDocument> allJsonFiles = new();
        private static readonly ConcurrentDictionary<string, JsonDocument> fallbackJsonFiles = new();
        private readonly string typedName;
        private readonly ILogger logger;
        private string? searchedLocation;
        public JsonLocalizer()
        {

        }
        public JsonLocalizer(JsonLocalizationOptions options, string resourceName, ILogger logger)
        {
            this.typedName = resourceName;
            this.logger = logger ?? NullLogger.Instance;
        }
        public LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var value = GetStringSafely(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: searchedLocation);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var format = GetStringSafely(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value ?? name, resourceNotFound: format == null, searchedLocation: searchedLocation);
            }
        }

        private string? GetStringSafely(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            //var doc = GetJsonDocument(CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "zh-CN");
            var (first, fallback) = GetJsonDocument(CultureInfo.CurrentUICulture.Name);
            if (first == null && fallback == null) return null;
            string? value = null;
            if (first != null)
                value = SolveJsonPath(first.RootElement, name);
            if (value == null && fallback != null)
                value = SolveJsonPath(fallback.RootElement, name);
            return value;
        }
        /// <summary>
        /// 判断获取的Json文件，是不是按resourceName区分文件夹
        /// </summary>
        bool useResourceName = false;
        private string? SolveJsonPath(JsonElement root, string name)
        {
            var node = root;
            if (name.IndexOf('.') > -1)
            {
                var paths = new Queue<string>(name.Split('.'));
                while (paths.Count > 0)
                {
                    var p = paths.Dequeue();
                    if (p == typedName && useResourceName) continue;
                    if (!node.TryGetProperty(p, out node))
                    {
                        return null;
                    }
                }
                return node.GetString();
            }
            else
            {
                if (node.TryGetProperty(name, out node) && node.ValueKind == JsonValueKind.String)
                {
                    return node.GetString();
                }
                return null;
            }
        }
        private (JsonDocument? First, JsonDocument? Fallback) GetJsonDocument(string culture)
        {
            var fallback = GetFallbackJsonDocument(culture);
            var doc = documentCache.GetOrAdd(culture, lang =>
            {
                JsonDocument? value = null;
                // Locales/Langs/zh-CN/resourceName/index.json
                if (CheckFileExits(true, true, true, lang, out var path))
                {
                    LoadJsonDocumentFromPath(path, out value);
                }
                // Locales/Langs/zh-CN/resourceName.json
                else if (CheckFileExits(true, false, true, lang, out path))
                {
                    LoadJsonDocumentFromPath(path, out value);
                }
                // Locales/Langs/resourceName/zh-CN.json
                else if (CheckFileExits(false, true, true, lang, out path))
                {
                    LoadJsonDocumentFromPath(path, out value);
                }
                // Locales/Langs/resourceName.zh-CN.json
                else if (CheckFileExits(false, false, true, lang, out path))
                {
                    LoadJsonDocumentFromPath(path, out value);
                }
                useResourceName = value != null;
                return value;
            });
            return (doc, fallback);
        }

        private string ConstructJsonFilePath(bool useCultureFolder, bool useTypedFolder, bool useTypedName, string culture)
        {
            var paths = new List<string>() { AppDomain.CurrentDomain.BaseDirectory, "Locales", "Langs" };
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

            return Path.Combine(paths.ToArray());
        }

        private void LoadJsonDocumentFromPath(string path, out JsonDocument? value)
        {
            searchedLocation = path;
            if (allJsonFiles.TryGetValue(searchedLocation, out value))
            {
                return;
            }
            var content = File.ReadAllText(searchedLocation, System.Text.Encoding.UTF8);
            if (!string.IsNullOrWhiteSpace(content))
            {
                try
                {
                    value = JsonDocument.Parse(content);
                    allJsonFiles.TryAdd(searchedLocation, value);
                    return;
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, $"invalid json content, path: {searchedLocation}, content: {content}");
                }
            }
            value = null;
        }

        private bool CheckFileExits(bool useCultureFolder, bool useTypedFolder, bool useTypedName, string culture, out string path)
        {
            path = ConstructJsonFilePath(useCultureFolder, useTypedFolder, useTypedName, culture);
            return File.Exists(path);
        }

        private JsonDocument? GetFallbackJsonDocument(string culture)
        {
            if (!fallbackJsonFiles.TryGetValue(culture, out var fallback))
            {
                // Locales/Langs/zh-CN/index.json
                if (CheckFileExits(true, false, false, culture, out var path))
                {
                    LoadJsonDocumentFromPath(path, out fallback);
                }
                // Locales/Langs/zh-CN.json
                else if (CheckFileExits(false, false, false, culture, out path))
                {
                    LoadJsonDocumentFromPath(path, out fallback);
                }

                if (fallback != null && !fallbackJsonFiles.ContainsKey(culture))
                    fallbackJsonFiles.TryAdd(culture, fallback);
            }
            return fallback;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Enumerable.Empty<LocalizedString>();
        }
    }
}

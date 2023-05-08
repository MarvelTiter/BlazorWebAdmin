using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Project.AppCore.Locales.Services
{
    public class JsonLocalizer : IStringLocalizer
    {
        private readonly ConcurrentDictionary<string, JsonDocument> documentCache = new();
        private readonly string resourcesPath;
        private readonly string resourceName;
        private readonly ILogger logger;
        private string? searchedLocation;
        public JsonLocalizer()
        {

        }
        public JsonLocalizer(JsonLocalizationOptions options, string resourceName, ILogger logger)
        {
            this.resourceName = resourceName;
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
                return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: searchedLocation);
            }
        }

        private string? GetStringSafely(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var doc = GetJsonDocument(CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "zh-CN");
            return SolveJsonPath(doc.RootElement, name);
        }

        private string? SolveJsonPath(JsonElement root, string name)
        {
            if (name.IndexOf('.') > -1)
            {
                var paths = new Queue<string>(name.Split('.'));
                while (paths.Count > 0)
                {
                    if (!root.TryGetProperty(paths.Dequeue(), out root))
                    {
                        return "";
                    }
                }
                return root.GetString();
            }
            else
            {
                return root.GetProperty(name).GetString();
            }
        }

        private JsonDocument GetJsonDocument(string culture)
        {
            return documentCache.GetOrAdd(culture, lang =>
            {
                var resourceFile = $"{lang}.json";
                searchedLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Locales", "Langs", resourceName, resourceFile);
                if (!LoadJsonDocumentFromPath(out var value))
                {
                    searchedLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Locales", "Langs", resourceFile);
                    LoadJsonDocumentFromPath(out value);
                }
                return value;
            });
        }

        private bool LoadJsonDocumentFromPath(out JsonDocument? value)
        {
            if (File.Exists(searchedLocation))
            {
                var content = File.ReadAllText(searchedLocation, System.Text.Encoding.UTF8);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    try
                    {
                        value = JsonDocument.Parse(content);
                        return true;
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e, $"invalid json content, path: {searchedLocation}, content: {content}");
                    }
                }
            }
            value = null;
            return false;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Enumerable.Empty<LocalizedString>();
        }
    }
}

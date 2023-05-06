using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

namespace Project.AppCore.Locales.Services
{
    public class JsonLocalizer : IStringLocalizer
    {
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> resourcesCache = new();
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

        private string GetStringSafely(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string value = null;

            var resources = GetResources(CultureInfo.CurrentUICulture.Name);
            if (resources?.ContainsKey(name) ?? false)
            {
                value = resources[name];
            }

            return value;
        }

        private Dictionary<string, string> GetResources(string culture)
        {
            return resourcesCache.GetOrAdd(culture, _ =>
            {
                var resourceFile = $"{culture}.json";
                searchedLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Locales", "Langs", resourceFile);
                Dictionary<string, string> value = null;
                if (File.Exists(searchedLocation))
                {
                    var content = File.ReadAllText(searchedLocation, System.Text.Encoding.UTF8);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        try
                        {
                            value = JsonSerializer.Deserialize<Dictionary<string, string>>(content) ?? new();
                        }
                        catch (Exception e)
                        {
                            logger.LogWarning(e, $"invalid json content, path: {searchedLocation}, content: {content}");
                        }
                    }
                }
                return value;
            });
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Enumerable.Empty<LocalizedString>();
        }
    }
}

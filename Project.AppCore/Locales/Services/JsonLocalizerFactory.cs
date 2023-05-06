using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

namespace Project.AppCore.Locales.Services
{
    public class JsonLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ConcurrentDictionary<string, JsonLocalizer> caches = new();
        private readonly ILoggerFactory loggerFactory;
        private readonly JsonLocalizationOptions options;

        public JsonLocalizerFactory(IOptions<JsonLocalizationOptions> options, ILoggerFactory loggerFactory)
        {
            this.options = options.Value;
            this.loggerFactory = loggerFactory;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }
            var resourceName = TrimPrefix(resourceSource.FullName!, "");
            return CreateJsonLocalizer(resourceName);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            var resourceName = TrimPrefix(baseName, location + ".");
            return CreateJsonLocalizer(resourceName);
        }

        private JsonLocalizer CreateJsonLocalizer(string resourceName)
        {
            ILogger<JsonLocalizer> logger = loggerFactory.CreateLogger<JsonLocalizer>();
            return caches.GetOrAdd(resourceName, resName => new JsonLocalizer(
                options,
                resName,
                logger));
        }

        private static string TrimPrefix(string name, string prefix)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
            {
                return name.Substring(prefix.Length);
            }

            return name;
        }
    }
}

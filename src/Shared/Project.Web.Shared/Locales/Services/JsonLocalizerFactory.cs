using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

namespace Project.Web.Shared.Locales.Services
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
            return CreateJsonLocalizer(resourceSource.Name);
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

            return CreateJsonLocalizer(location);
        }

        private JsonLocalizer CreateJsonLocalizer(string resourceName)
        {
            ILogger<JsonLocalizer> logger = loggerFactory.CreateLogger<JsonLocalizer>();
            return caches.GetOrAdd(resourceName, resName => new JsonLocalizer(
                options,
                resName,
                logger));
        }

    }
}

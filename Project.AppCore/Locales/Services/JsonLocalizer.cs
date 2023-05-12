using Microsoft.AspNetCore.Authentication;
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
			var doc = GetJsonDocument(CultureInfo.CurrentUICulture.Name);
			if (doc == null) return null;
			return SolveJsonPath(doc.RootElement, name);
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
					if (p == resourceName && useResourceName) continue;
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
		private JsonDocument GetJsonDocument(string culture)
		{
			return documentCache.GetOrAdd(culture, lang =>
			{
				var resourceFile = $"{lang}.json";
				searchedLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Locales", "Langs", resourceName, resourceFile);
				if (LoadJsonDocumentFromPath(out var value))
				{
					useResourceName = true;
				}
				else
				{
					searchedLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Locales", "Langs", resourceFile);
					LoadJsonDocumentFromPath(out value);
					useResourceName = false;
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

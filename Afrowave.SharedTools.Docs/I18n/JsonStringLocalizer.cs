using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace Afrowave.SharedTools.Docs.I18n;

/// <summary>
/// Provides localization using JSON files stored in a distributed cache.
/// </summary>
public class JsonStringLocalizer : IStringLocalizer
{
	private readonly IDistributedCache _cache;
	private readonly string _localesPath;

	/// <summary>
	/// Initializes a new instance of the <see cref="JsonStringLocalizer"/> class.
	/// </summary>
	/// <param name="cache">Distributed cache instance.</param>
	public JsonStringLocalizer(IDistributedCache cache)
	{
		_cache = cache;
		string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0];
		_localesPath = Path.Combine(projectPath, "Locales");
	}

	/// <inheritdoc/>
	public LocalizedString this[string name]
	{
		get
		{
			string? value = GetString(name);
			return new LocalizedString(name, value ?? name, value == null);
		}
	}

	/// <inheritdoc/>
	public LocalizedString this[string name, params object[] arguments]
	{
		get
		{
			LocalizedString localized = this[name];
			return localized.ResourceNotFound
				 ? localized
				 : new LocalizedString(name, string.Format(localized.Value, arguments), false);
		}
	}

	/// <inheritdoc/>
	public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
	{
		string filePath = GetLocaleFilePath();
		if(!File.Exists(filePath))
		{
			throw new FileNotFoundException("Localization file not found.");
		}

		using FileStream stream = File.OpenRead(filePath);
		using JsonDocument document = JsonDocument.Parse(stream);

		foreach(JsonProperty prop in document.RootElement.EnumerateObject())
		{
			yield return new LocalizedString(prop.Name, prop.Value.GetString() ?? string.Empty, false);
		}
	}

	private string? GetString(string key)
	{
		string filePath = GetLocaleFilePath();
		string cacheKey = $"locale_{CultureInfo.CurrentUICulture.Name}_{key}";
		string? cachedValue = _cache.GetString(cacheKey);

		if(!string.IsNullOrEmpty(cachedValue))
		{
			return cachedValue;
		}

		string? value = GetValueFromJson(key, filePath);
		if(!string.IsNullOrEmpty(value))
		{
			_cache.SetString(cacheKey, value);
		}

		return value;
	}

	private string GetLocaleFilePath()
	{
		string culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
		string filePath = Path.Combine(_localesPath, $"{culture}.json");
		return File.Exists(filePath) ? filePath : Path.Combine(_localesPath, "en.json");
	}

	private static string? GetValueFromJson(string key, string filePath)
	{
		if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
		{
			return default;
		}

		try
		{
			using FileStream stream = File.OpenRead(filePath);
			using JsonDocument document = JsonDocument.Parse(stream);

			if(document.RootElement.TryGetProperty(key, out JsonElement element))
			{
				return element.GetString();
			}
		}
		catch
		{
			// ignore error
		}
		return default;
	}
}
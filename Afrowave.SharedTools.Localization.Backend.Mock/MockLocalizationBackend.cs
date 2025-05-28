using Afrowave.SharedTools.Localization.Common.Models.Backend;
using Afrowave.SharedTools.Localization.Interfaces;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Backend.Mock
{
	/// <summary>
	/// A mock implementation of <see cref="ILocalizationBackend"/> for testing and simulation.
	/// </summary>
	public class MockLocalizationBackend : ILocalizationBackend
	{
		/// <summary>
		/// Stores translation data for each language and key.
		/// </summary>
		private readonly Dictionary<string, Dictionary<string, string>> _data = new Dictionary<string, Dictionary<string, string>>();

		/// <summary>
		/// The name of this backend instance.
		/// </summary>
		private readonly string _name;

		/// <summary>
		/// The default language used by this backend.
		/// </summary>
		private readonly string _defaultLanguage;

		/// <summary>
		/// If true, the backend will throw an exception on <see cref="GetValueAsync"/>.
		/// </summary>
		public bool ThrowOnGet { get; set; } = false;

		/// <summary>
		/// If true, returned values will include a warning flag.
		/// </summary>
		public bool SimulateWarning { get; set; } = false;

		/// <summary>
		/// If true, <see cref="GetValueAsync"/> will return a successful response with an empty string instead of failure.
		/// </summary>
		public bool SimulateEmptySuccess { get; set; } = false;

		/// <summary>
		/// Tracks the number of <see cref="GetValueAsync"/> calls made.
		/// </summary>
		public int CallCount { get; private set; } = 0;

		/// <summary>
		/// Gets the unique name of the backend implementation.
		/// </summary>
		public string Name => _name;

		/// <summary>
		/// Gets or sets the capabilities and metadata of the backend.
		/// </summary>
		public LocalizationBackendCapabilities Capabilities { get; set; } = new LocalizationBackendCapabilities
		{
			BackendType = "Mock",
			Description = "Mock backend for unit testing and simulation",
			Author = "Afrowave Team",
			Version = "1.0.0",
			CanRead = true,
			CanWrite = true,
			CanBulkRead = true,
			CanBulkWrite = true,
			SupportsLanguagesListing = true,
			UsesKeyAsDefaultValue = false
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="MockLocalizationBackend"/> class.
		/// </summary>
		/// <param name="name">The name of the backend instance.</param>
		/// <param name="defaultLanguage">The default language code.</param>
		public MockLocalizationBackend(string name = "MockBackend", string defaultLanguage = "en")
		{
			_name = name;
			_defaultLanguage = defaultLanguage;
		}

		/// <summary>
		/// Sets a translation value for a given language and key.
		/// </summary>
		/// <param name="language">The language code.</param>
		/// <param name="key">The translation key.</param>
		/// <param name="value">The translation value.</param>
		public void SetValue(string language, string key, string value)
		{
			if(!_data.ContainsKey(language))
				_data[language] = new Dictionary<string, string>();

			_data[language][key] = value;
		}

		/// <summary>
		/// Loads a localized value for a given language and key.
		/// </summary>
		/// <param name="language">The language code.</param>
		/// <param name="key">The translation key.</param>
		/// <returns>A response containing the translation or an error/warning.</returns>
		public Task<Response<string>> GetValueAsync(string language, string key)
		{
			CallCount++;

			if(ThrowOnGet)
				throw new InvalidOperationException("Simulated backend failure.");

			if(SimulateEmptySuccess)
			{
				return Task.FromResult(Response<string>.SuccessResponse(string.Empty, "Simulated empty result"));
			}

			if(_data.TryGetValue(language, out var dict) && dict.TryGetValue(key, out var value))
			{
				return Task.FromResult(SimulateWarning
					 ? Response<string>.SuccessWithWarning(value, "Simulated warning")
					 : Response<string>.SuccessResponse(value, "Simulated success"));
			}

			return Task.FromResult(Response<string>.Fail("Key not found in backend."));
		}

		/// <summary>
		/// Sets a localized value for a given language and key.
		/// </summary>
		/// <param name="language">The language code.</param>
		/// <param name="key">The translation key.</param>
		/// <param name="value">The translation value.</param>
		/// <returns>True if the value was stored successfully; false otherwise.</returns>
		public Task<bool> SetValueAsync(string language, string key, string value)
		{
			SetValue(language, key, value);
			return Task.FromResult(true);
		}

		/// <summary>
		/// Gets all key-value pairs for a given language.
		/// </summary>
		/// <param name="language">The language code.</param>
		/// <returns>A dictionary of translations.</returns>
		public Task<IDictionary<string, string>> GetAllValuesAsync(string language)
		{
			if(_data.TryGetValue(language, out var dict))
				return Task.FromResult((IDictionary<string, string>)dict);

			return Task.FromResult((IDictionary<string, string>)new Dictionary<string, string>());
		}

		/// <summary>
		/// Sets multiple translations for a given language.
		/// </summary>
		/// <param name="language">The language code.</param>
		/// <param name="values">The dictionary of keys and values to store.</param>
		/// <returns>True if all values were written successfully.</returns>
		public Task<bool> SetAllValuesAsync(string language, IDictionary<string, string> values)
		{
			foreach(var kv in values)
				SetValue(language, kv.Key, kv.Value);

			return Task.FromResult(true);
		}

		/// <summary>
		/// Gets the list of all languages available in this backend.
		/// </summary>
		/// <returns>A list of language codes (e.g. "en", "cs", "fr").</returns>
		public Task<IList<string>> GetAvailableLanguagesAsync()
		{
			return Task.FromResult<IList<string>>(new List<string>(_data.Keys));
		}
	}
}
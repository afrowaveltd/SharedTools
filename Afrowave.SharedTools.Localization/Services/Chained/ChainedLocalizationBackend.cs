using Afrowave.SharedTools.Localization.Interfaces;
using Afrowave.SharedTools.Models.Localization;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Services.Chained
{
	/// <summary>
	/// A localization backend that attempts resolution using a prioritized chain of backend instances.
	/// </summary>
	public class ChainedLocalizationBackend : ILocalizationBackend
	{
		/// <summary>
		/// The chain of localization backend instances used for resolution.
		/// </summary>
		private readonly IList<ILocalizationBackend> _chain;

		/// <summary>
		/// Gets the unique name of the backend implementation.
		/// </summary>
		public string Name => "ChainedLocalizationBackend";

		/// <summary>
		/// Gets the capabilities and metadata of the backend, aggregated from the chain.
		/// </summary>
		public LocalizationBackendCapabilities Capabilities =>
			 new LocalizationBackendCapabilities
			 {
				 BackendType = "Composite",
				 Description = "Chains multiple localization backends with fallback logic",
				 CanRead = _chain.Any(b => b.Capabilities.CanRead),
				 CanWrite = _chain.Any(b => b.Capabilities.CanWrite),
				 CanBulkRead = _chain.Any(b => b.Capabilities.CanBulkRead),
				 CanBulkWrite = _chain.Any(b => b.Capabilities.CanBulkWrite),
				 SupportsLanguagesListing = _chain.Any(b => b.Capabilities.SupportsLanguagesListing),
				 UsesKeyAsDefaultValue = false,
				 Author = "Afrowave",
				 Version = "1.0.0"
			 };

		/// <summary>
		/// Initializes a new instance of the <see cref="ChainedLocalizationBackend"/> class with the specified backends.
		/// </summary>
		/// <param name="backends">The localization backends to chain together.</param>
		public ChainedLocalizationBackend(params ILocalizationBackend[] backends)
		{
			_chain = backends.ToList();
		}

		/// <summary>
		/// Loads a localized value for a given language and key, searching the chain in order.
		/// </summary>
		/// <param name="language">The language code (e.g. "en").</param>
		/// <param name="key">The translation key.</param>
		/// <returns>The translated value, or the key as a fallback if not found.</returns>
		public async Task<Response<string>> GetValueAsync(string language, string key)
		{
			if(string.IsNullOrWhiteSpace(key))
				return Response<string>.SuccessResponse(string.Empty, "Key is empty");

			foreach(var backend in _chain)
			{
				if(!backend.Capabilities.CanRead)
					continue;

				try
				{
					var response = await backend.GetValueAsync(language, key);
					if(response.Success && !string.IsNullOrWhiteSpace(response.Data))
						return response;
				}
				catch(Exception ex)
				{
					// Optional: log the failure of this backend
				}
			}

			// If nothing was found, return the key itself as a fallback
			return Response<string>.SuccessWithWarning(key, "No translation found; returning key as fallback");
		}

		/// <summary>
		/// Sets a localized value for a given language and key using the first writable backend in the chain.
		/// </summary>
		/// <param name="language">The language code.</param>
		/// <param name="key">The translation key.</param>
		/// <param name="value">The translation value.</param>
		/// <returns>True if the value was stored successfully; false otherwise.</returns>
		public async Task<bool> SetValueAsync(string language, string key, string value)
		{
			var writer = _chain.FirstOrDefault(b => b.Capabilities.CanWrite);
			return writer != null && await writer.SetValueAsync(language, key, value);
		}

		/// <summary>
		/// Gets all key-value pairs for a given language by aggregating results from all backends that support bulk read.
		/// </summary>
		/// <param name="language">The language code.</param>
		/// <returns>A dictionary of translations.</returns>
		public async Task<IDictionary<string, string>> GetAllValuesAsync(string language)
		{
			var result = new Dictionary<string, string>();
			foreach(var backend in _chain)
			{
				if(!backend.Capabilities.CanBulkRead) continue;

				try
				{
					var values = await backend.GetAllValuesAsync(language);
					foreach(var kv in values)
						result[kv.Key] = kv.Value;
				}
				catch
				{
					// Optional: log
				}
			}
			return result;
		}

		/// <summary>
		/// Sets multiple translations for a given language using the first backend that supports bulk write.
		/// </summary>
		/// <param name="language">The language code.</param>
		/// <param name="values">The dictionary of keys and values to store.</param>
		/// <returns>True if all values were written successfully.</returns>
		public async Task<bool> SetAllValuesAsync(string language, IDictionary<string, string> values)
		{
			var writer = _chain.FirstOrDefault(b => b.Capabilities.CanBulkWrite);
			return writer != null && await writer.SetAllValuesAsync(language, values);
		}

		/// <summary>
		/// Gets the list of all languages available in the chain by aggregating results from all backends that support language listing.
		/// </summary>
		/// <returns>A list of language codes (e.g. "en", "cs", "fr").</returns>
		public async Task<IList<string>> GetAvailableLanguagesAsync()
		{
			var result = new HashSet<string>();
			foreach(var backend in _chain)
			{
				if(!backend.Capabilities.SupportsLanguagesListing) continue;

				try
				{
					var langs = await backend.GetAvailableLanguagesAsync();
					foreach(var lang in langs)
						result.Add(lang);
				}
				catch
				{
					// Optional: log
				}
			}
			return result.ToList();
		}
	}
}
using Afrowave.SharedTools.Localization.Common.Models.Backend;

using Afrowave.SharedTools.Models.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Interfaces
{
	/// <summary>
	/// Represents a localization backend implementation that provides dictionary operations.
	/// </summary>
	public interface ILocalizationBackend
	{
		/// <summary>
		/// Gets the unique name of the backend implementation.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the capabilities and metadata of the backend.
		/// </summary>
		LocalizationBackendCapabilities Capabilities { get; }

		/// <summary>
		/// Loads a localized value for a given language and key.
		/// </summary>
		/// <param name="language">The language code (e.g. "en").</param>
		/// <param name="key">The translation key.</param>
		/// <returns>The translated value, or null if not found.</returns>
		Task<Response<string>> GetValueAsync(string language, string key);

		/// <summary>
		/// Sets a localized value for a given language and key.
		/// </summary>
		/// <param name="language">The language code.</param>
		/// <param name="key">The translation key.</param>
		/// <param name="value">The translation value.</param>
		/// <returns>True if the value was stored successfully; false otherwise.</returns>
		Task<bool> SetValueAsync(string language, string key, string value);

		/// <summary>
		/// Gets all key-value pairs for a given language.
		/// </summary>
		/// <param name="language">The language code.</param>
		/// <returns>A dictionary of translations.</returns>
		Task<IDictionary<string, string>> GetAllValuesAsync(string language);

		/// <summary>
		/// Sets multiple translations for a given language.
		/// </summary>
		/// <param name="language">The language code.</param>
		/// <param name="values">The dictionary of keys and values to store.</param>
		/// <returns>True if all values were written successfully.</returns>
		Task<bool> SetAllValuesAsync(string language, IDictionary<string, string> values);

		/// <summary>
		/// Gets the list of all languages available in this backend.
		/// </summary>
		/// <returns>A list of language codes (e.g. "en", "cs", "fr").</returns>
		Task<IList<string>> GetAvailableLanguagesAsync();
	}
}
using Afrowave.SharedTools.I18N.EventHandler;
using Afrowave.SharedTools.I18N.Models;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.I18N.Interfaces
{
	/// <summary>
	/// Defines the contract for asynchronous storage and retrieval of language translation dictionaries, including
	/// management operations and capability queries.
	/// </summary>
	/// <remarks>Implementations of this interface provide methods to save, load, list, and delete translation
	/// dictionaries for different languages, as well as to check for dictionary existence and storage mutability. The
	/// interface also exposes a delegate for handling dictionary change notifications and a method to query supported
	/// storage capabilities. All operations are asynchronous and may return result or response types indicating success,
	/// failure, or additional data. Thread safety and error handling depend on the specific implementation.</remarks>
	public interface IDataStorage
	{
		/// <summary>
		/// Asynchronously saves a set of translation key-value pairs for the specified language.
		/// </summary>
		/// <param name="languageCode">The language code that identifies the target language for the translations. Must be a non-empty string, such as
		/// "en" for English or "fr" for French.</param>
		/// <param name="translations">A dictionary containing translation key-value pairs to be saved. Keys represent resource identifiers; values are
		/// the corresponding translated strings. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous save operation. The result contains information about the success or
		/// failure of the operation.</returns>
		public Task<Result> SaveDictionaryAsync(string languageCode, Dictionary<string, string> translations);

		/// <summary>
		/// Asynchronously loads a dictionary of localized strings for the specified language code.
		/// </summary>
		/// <param name="languageCode">The language code identifying which set of localized strings to load. Must be a valid ISO language code; for
		/// example, "en" for English or "fr" for French.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a response with a dictionary mapping
		/// string keys to their localized values for the specified language. The dictionary will be empty if no entries are
		/// found for the given language code.</returns>
		public Task<Response<Dictionary<string, string>>> LoadDictionaryAsync(string languageCode);

		/// <summary>
		/// Asynchronously retrieves a list of language codes that are available for use with the service.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with a list of supported language codes. The list will be empty if no languages are available.</returns>
		public Task<Response<List<string>>> ListAvailableLanguagesAsync();

		/// <summary>
		/// Asynchronously deletes the dictionary associated with the specified language code.
		/// </summary>
		/// <param name="languageCode">The language code identifying the dictionary to delete. Must be a valid, non-empty string representing a supported
		/// language.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating
		/// whether the dictionary was successfully deleted.</returns>
		public Task<Result> DeleteDictionaryAsync(string languageCode);

		/// <summary>
		/// Asynchronously determines whether a dictionary exists for the specified language code.
		/// </summary>
		/// <param name="languageCode">The language code to check for an existing dictionary. Must be a valid ISO language code and cannot be null or
		/// empty.</param>
		/// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if a dictionary
		/// exists for the specified language code; otherwise, <see langword="false"/>.</returns>
		public Task<bool> DictionaryExistsAsync(string languageCode);

		/// <summary>
		/// Asynchronously determines whether the underlying data source is in read-only mode.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the data source is
		/// read-only; otherwise, <see langword="false"/>.</returns>
		public Task<bool> IsReadOnlyAsync();

		/// <summary>
		/// Retrieves the localized translation string for the specified language and key asynchronously.
		/// </summary>
		/// <param name="languageCode">The language code representing the target language for the translation. Must be a valid ISO language code and
		/// cannot be null or empty.</param>
		/// <param name="key">The key identifying the text to be translated. Cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{string}"/> with
		/// the translated string if found; otherwise, the response may indicate an error or missing translation.</returns>
		public Task<Response<string>> GetTranslation(string languageCode, string key);

		/// <summary>
		/// Retrieves the capabilities supported by the data storage provider.
		/// </summary>
		/// <returns>A <see cref="DataStorageCapabilities"/> object describing the features and limitations of the data storage
		/// provider.</returns>
		public DataStorageCapabilities GetCapabilities();

		/// <summary>
		/// Represents the method that will handle an event when the dictionary's language changes.
		/// </summary>
		/// <param name="sender">The source of the event, typically the dictionary instance that triggered the change.</param>
		/// <param name="e">The event object</param>

		public delegate void DictionaryChangedEventHandler(object sender, DictionaryChangedEventArgs e);
	}
}
using Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Models;
using Afrowave.SharedTools.Models.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Common.Interfaces
{
	/// <summary>
	/// Defines the contract for a localization backend that provides methods for reading, writing,  updating, and removing
	/// localization data in both flat and structured formats.
	/// </summary>
	/// <remarks>This interface is designed to support localization systems by enabling interaction with
	/// language-specific data. It provides methods for managing translations in flat key-value  pairs or hierarchical
	/// structures, as well as operations for handling available languages. Implementations of this interface should ensure
	/// thread safety and handle any necessary  persistence or caching mechanisms.</remarks>
	public interface ILocalizationBackend
	{
		/// <summary>
		/// Asynchronously reads a flat dictionary of key-value pairs for the specified language code.
		/// </summary>
		/// <remarks>The returned dictionary is flat, meaning it does not contain nested structures. This method is
		/// typically used for retrieving localized content for a specific language.</remarks>
		/// <param name="langCode">The language code for which the dictionary should be retrieved. Must be a valid ISO 639-1 language code.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with a dictionary of key-value pairs, where the keys are strings representing identifiers and the values are
		/// strings representing localized content.</returns>
		Task<Response<Dictionary<string, string>>> ReadFlatAsync(string langCode);

		/// <summary>
		/// Asynchronously reads a flat dictionary of key-value pairs for the specified language code.
		/// </summary>
		/// <param name="langCode">Language Code</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with a structured translation tree for the specified language.</returns>
		Task<Response<TranslationTree>> ReadStructuredAsync(string langCode);

		/// <summary>
		/// Asynchronously reads a structured translation tree for the specified language code.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with a structured translation tree for the specified language.</returns>
		Task<Response<List<string>>> GetAvailableLanguagesAsync();

		/// <summary>
		/// Writes the provided key-value data to a flat file format for the specified language code.
		/// </summary>
		/// <remarks>This method performs asynchronous I/O operations and may involve writing to disk or other
		/// storage. Ensure that the caller has appropriate permissions for the target location.</remarks>
		/// <param name="langCode">The language code representing the target language for the data.  Must be a valid ISO 639-1 code.</param>
		/// <param name="data">A dictionary containing the key-value pairs to be written.  Keys represent identifiers, and values represent the
		/// corresponding translations or data. Cannot be null or empty.</param>
		/// <returns>A <see cref="Response{T}"/> object containing a boolean value indicating whether the operation  was successful
		/// (<see langword="true"/>) or not (<see langword="false"/>).</returns>
		Task<Response<bool>> WriteFlatAsync(string langCode, Dictionary<string, string> data);

		/// <summary>
		/// Writes a structured translation tree to the underlying storage asynchronously.
		/// </summary>
		/// <remarks>This method performs the write operation asynchronously and may involve network or I/O
		/// operations. Ensure that the provided <paramref name="langCode"/> is valid and the <paramref name="tree"/> contains
		/// the necessary data for the operation.</remarks>
		/// <param name="langCode">The language code associated with the translation tree. Must be a valid ISO 639-1 code.</param>
		/// <param name="tree">The translation tree containing the structured data to be written. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> indicating
		/// whether the operation succeeded (<see langword="true"/>) or failed (<see langword="false"/>).</returns>
		Task<Response<bool>> WriteStructuredAsync(string langCode, TranslationTree tree);

		/// <summary>
		/// Asynchronously updates a flat key-value pair for the specified language code.
		/// </summary>
		/// <param name="langCode">The language code associated with the translation tree. Must be a valid ISO 639-1 code</param>
		/// <param name="key">Key</param>
		/// <param name="value">Translated value</param>
		/// <param name="createIfMissing">The boolean value deciding if the missing keys should be created if translation layer is supported</param>
		/// <returns></returns>
		Task<Response<bool>> UpdateFlatKeyAsync(string langCode, string key, string value, bool createIfMissing = true);

		/// <summary>
		/// Asynchronously updates a structured key in the translation tree for the specified language code.
		/// </summary>
		/// <param name="langCode">The language code associated with the translation tree. Must be a valid ISO 639-1 code.</param>
		/// <param name="path">The path to the key in the structured translation tree. Cannot be null or empty.</param>
		/// <param name="value">Updated value for the specified key in the structured translation tree. Cannot be null.</param>
		/// <param name="createIfMissing"></param>
		/// <returns></returns>
		Task<Response<bool>> UpdateStructuredKeyAsync(string langCode, string[] path, string value, bool createIfMissing = true);

		/// <summary>
		/// Removes a flat key from the specified language code asynchronously.
		/// </summary>
		/// <param name="langCode">The language code associated with the key to be removed. Must be a valid, non-null, and non-empty string.</param>
		/// <param name="key">The flat key to be removed. Must be a valid, non-null, and non-empty string.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// indicating whether the key was successfully removed (<see langword="true"/>) or not (<see langword="false"/>).</returns>
		Task<Response<bool>> RemoveFlatKeyAsync(string langCode, string key);

		/// <summary>
		/// Removes a structured key from the specified language code asynchronously.
		/// </summary>
		/// <param name="langCode">The language code associated with the key to be removed. Must be a valid, non-null, and non-empty string.</param>
		/// <param name="path"></param>
		/// <returns></returns>
		Task<Response<bool>> RemoveStructuredKeyAsync(string langCode, string[] path);

		/// <summary>
		/// Removes a language from the system based on its language code.
		/// </summary>
		/// <param name="langCode">The language code of the language to be removed. Must be a valid ISO 639-1 code.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a  <see cref="Response{T}"/> object
		/// indicating whether the operation was successful.</returns>
		Task<Response<bool>> RemoveLanguageAsync(string langCode);

		/// <summary>
		/// Asynchronously adds an empty language entry to the system.
		/// </summary>
		/// <param name="langCode">The language code for the new language entry. Must be a valid ISO 639-1 code.</param>
		/// <returns>
		/// The Asynchronous task that represents the operation. The task result contains a <see cref="Response{T}"/> object
		/// </returns>
		Task<Response<bool>> AddEmptyLanguageAsync(string langCode);
	}
}
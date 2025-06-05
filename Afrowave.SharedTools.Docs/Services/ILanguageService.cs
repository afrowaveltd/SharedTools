namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Defines methods for retrieving and managing language-related information.
	/// </summary>
	/// <remarks>This interface provides functionality to retrieve language details by code,  fetch a list of
	/// available languages, determine if a language is right-to-left (RTL),  and retrieve a list of required languages
	/// asynchronously.</remarks>
	public interface ILanguageService
	{
		/// <summary>
		/// Asynchronously retrieves all available dictionaries as a translation tree.
		/// </summary>
		/// <remarks>The returned translation tree organizes the dictionaries hierarchically, allowing for structured
		/// traversal of available translations. This method does not take any parameters and is intended to provide a
		/// complete view of all dictionaries.</remarks>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see
		/// cref="Response{TranslationTree}"/> object, which includes the translation tree representing all dictionaries.</returns>
		Task<Response<TranslationTree>> GetAllDictionariesAsync();

		/// <summary>
		/// Asynchronously retrieves a dictionary of key-value pairs based on the specified code.
		/// </summary>
		/// <remarks>The returned dictionary may vary depending on the provided code. Ensure the code is valid  and
		/// corresponds to an existing dictionary in the underlying data source.</remarks>
		/// <param name="code">A string representing the code used to identify the dictionary to retrieve.  This parameter cannot be null or
		/// empty.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a  <see cref="Response{T}"/> object
		/// wrapping a dictionary of key-value pairs, where the keys  and values are strings. If no dictionary is found for
		/// the specified code, the dictionary  will be empty.</returns>
		Task<Response<Dictionary<string, string>>> GetDictionaryAsync(string code);

		/// <summary>
		/// Retrieves a language by its ISO 639-1 code.
		/// </summary>
		/// <param name="code">The ISO 639-1 code of the language to retrieve. This value must be a non-null, non-empty string.</param>
		/// <returns>A <see cref="Response{T}"/> containing the <see cref="Language"/> object that matches the specified code, or <see
		/// langword="null"/> if no matching language is found.</returns>
		Response<Language>? GetLanguageByCode(string code);

		/// <summary>
		/// Retrieves a list of all available languages.
		/// </summary>
		/// <returns>A list of <see cref="Language"/> objects representing the available languages.  The list will be empty if no
		/// languages are available.</returns>
		List<Language> GetLanguages();

		/// <summary>
		/// Retrieves the most recently stored key-value pairs.
		/// </summary>
		/// <remarks>The returned dictionary contains the latest stored data, where each key represents a unique
		/// identifier and the corresponding value represents the associated data. If no data is available, the dictionary
		/// will be empty.</remarks>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with a dictionary of key-value pairs representing the last stored data.</returns>
		Task<Response<Dictionary<string, string>>> GetLastStored();

		/// <summary>
		/// Asynchronously retrieves a list of languages that are required for the current operation or context.
		/// </summary>
		/// <remarks>The returned list represents the languages deemed necessary based on the current operation's
		/// requirements. Callers should check the response for success and handle any potential errors
		/// appropriately.</remarks>
		/// <returns>A <see cref="Response{T}"/> containing a list of <see cref="Language"/> objects.  The list will be empty if no
		/// required languages are identified.</returns>
		Response<List<Language>> GetRequiredLanguagesAsync();

		/// <summary>
		/// Determines whether the specified language code represents a right-to-left (RTL) language.
		/// </summary>
		/// <remarks>This method evaluates the language code to determine if it is associated with a right-to-left
		/// writing system. Common right-to-left languages include Arabic ("ar") and Hebrew ("he").</remarks>
		/// <param name="code">The language code to evaluate, typically in ISO 639-1 format (e.g., "ar" for Arabic or "he" for Hebrew).</param>
		/// <returns><see langword="true"/> if the specified language code corresponds to a right-to-left language; otherwise, <see
		/// langword="false"/>.</returns>
		bool IsRtl(string code);

		/// <summary>
		/// Asynchronously saves a dictionary of key-value pairs associated with a specified code.
		/// </summary>
		/// <remarks>The method ensures that the provided dictionary is saved in association with the specified code.
		/// The operation is asynchronous and returns a response indicating success or failure.</remarks>
		/// <param name="code">A unique identifier used to associate the dictionary. Must not be <see langword="null"/> or empty.</param>
		/// <param name="data">The dictionary containing key-value pairs to be saved. Keys and values must not be <see langword="null"/>.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// indicating whether the save operation was successful.</returns>
		Task<Response<bool>> SaveDictionaryAsync(string code, Dictionary<string, string> data);

		/// <summary>
		/// Saves a collection of key-value pairs representing old translations asynchronously.
		/// </summary>
		/// <param name="data">A dictionary containing the translations to save, where the key represents the original text and the value
		/// represents the corresponding translation. Cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a  <see cref="Response{T}"/> object
		/// with a boolean value indicating whether the save operation  was successful.</returns>
		Task<Response<bool>> SaveOldTranslationAsync(Dictionary<string, string> data);

		/// <summary>
		/// Saves the translations from the specified translation tree asynchronously.
		/// </summary>
		/// <remarks>This method processes the provided translation tree and attempts to save each translation.  The
		/// result indicates the success or failure of saving each translation.</remarks>
		/// <param name="translationTree">The <see cref="TranslationTree"/> containing the translations to be saved.  This parameter cannot be <see
		/// langword="null"/>.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a  <see cref="Response{T}"/> object
		/// with a dictionary where the keys are translation  identifiers and the values indicate whether each translation was
		/// saved successfully.</returns>
		Task<Response<Dictionary<string, bool>>> SaveTranslationsAsync(TranslationTree translationTree);

		/// <summary>
		/// Retrieves the list of translations currently presented to the user.
		/// </summary>
		/// <returns>An array of strings containing the presented translations. The array will be empty if no translations are
		/// available.</returns>
		string[] TranslationsPresented();
	}
}
namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

/// <summary>
/// Defines methods for interacting with the LibreTranslate service to perform text and file translations and retrieve
/// available languages.
/// </summary>
/// <remarks>This interface provides asynchronous methods for translating text and files between languages and for
/// retrieving the list of supported languages from the LibreTranslate service.</remarks>
public interface ILibreTranslateService
{
	/// <summary>
	/// Asynchronously retrieves a list of available languages supported by the system.
	/// </summary>
	/// <remarks>The returned list contains the language codes in a standardized format (e.g., "en" for English,
	/// "fr" for French). The caller can use these codes to configure language-specific operations or display
	/// options.</remarks>
	/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object with
	/// an array of language codes. If no languages are available, the array will be empty.</returns>
	Task<Response<string[]>> GetAvailableLanguagesAsync();

	/// <summary>
	/// Translates the content of a file from its original language to the specified target language.
	/// </summary>
	/// <remarks>The method automatically detects the source language of the file and translates it to the specified
	/// target language. Ensure that the file format is supported by the translation service.</remarks>
	/// <param name="fileStream">The stream containing the file to be translated. The stream must be readable and positioned at the beginning.</param>
	/// <param name="targetLanguage">The language code of the target language (e.g., "en" for English, "es" for Spanish). Must not be null or empty.</param>
	/// <param name="fileName">The name of the file being translated. Used for processing and may affect the translation context.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object with
	/// the translation result as a <see cref="LibreTranslateFileResult"/>.</returns>
	Task<Response<LibreTranslateFileResult>> TranslateFileFromAnyLanguageAsync(Stream fileStream, string targetLanguage, string fileName);

	/// <summary>
	/// Translates the specified text from the source language to the target language.
	/// </summary>
	/// <remarks>This method performs an asynchronous translation operation using the specified source and target
	/// languages.  Ensure that the language codes provided are supported by the translation service.</remarks>
	/// <param name="text">The text to be translated. Cannot be null or empty.</param>
	/// <param name="sourceLanguage">The language code of the source text (e.g., "en" for English). Cannot be null or empty.</param>
	/// <param name="targetLanguage">The language code of the target text (e.g., "es" for Spanish). Cannot be null or empty.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
	/// with the translation result as a <see cref="LibreTranslationResult"/>. The response may include additional
	/// metadata.</returns>
	Task<Response<LibreTranslationResult>> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage);

	/// <summary>
	/// Translates the specified text into the target language using the LibreTranslate API.
	/// </summary>
	/// <remarks>This method performs an asynchronous call to the LibreTranslate API to translate the input text.
	/// Ensure that the target language is supported by the API.</remarks>
	/// <param name="text">The text to be translated. Cannot be null or empty.</param>
	/// <param name="targetLanguage">The language code of the target language (e.g., "en" for English, "es" for Spanish).  Must be a valid ISO 639-1
	/// language code.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains a  <see cref="Response{T}"/> object
	/// wrapping a <see cref="LibreTranslationResult"/>  with the translation details.</returns>
	Task<Response<LibreTranslationResult>> TranslateTextFromAnyLanugageAsync(string text, string targetLanguage);
}
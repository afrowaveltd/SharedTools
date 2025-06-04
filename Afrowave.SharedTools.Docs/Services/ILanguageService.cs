using Afrowave.SharedTools.Docs.Models;

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
	}
}
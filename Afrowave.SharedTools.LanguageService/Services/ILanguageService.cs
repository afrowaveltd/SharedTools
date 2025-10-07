using Afrowave.SharedTools.Models.Localization;
using Afrowave.SharedTools.Models.Results;
using System.Collections.Generic;

namespace Afrowave.SharedTools.I18N.Services
{
    /// <summary>
    /// Defines operations for managing languages including retrieval, addition, removal,
    /// and updates. Methods provide both raw models and standardized responses.
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>
        /// Adds a new <see cref="Language"/> to the collection and persists the change.
        /// </summary>
        /// <param name="language">The language to add.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        Result AddLanguageToList(Language language);

        /// <summary>
        /// Gets a language by its ISO code wrapped in a standardized response.
        /// </summary>
        /// <param name="code">The language ISO code to search for.</param>
        /// <returns>A <see cref="Response{T}"/> containing the matching <see cref="Language"/> or error information.</returns>
        Response<Language> GetLanguageByCode(string code);

        /// <summary>
        /// Gets all available languages wrapped in a standardized response.
        /// </summary>
        /// <returns>A <see cref="Response{T}"/> containing a <see cref="List{T}"/> of <see cref="Language"/>.</returns>
        Response<List<Language>> GetLanguages();

        /// <summary>
        /// Gets languages that match the provided ISO codes wrapped in a standardized response.
        /// </summary>
        /// <param name="codes">The list of ISO codes to search for.</param>
        /// <returns>A <see cref="Response{T}"/> with the matching languages. May include a warning if some codes are missing.</returns>
        Response<List<Language>> GetLanguagesByCodes(List<string> codes);

        /// <summary>
        /// Gets a language by its ISO code as a raw object.
        /// </summary>
        /// <param name="code">The language ISO code to search for.</param>
        /// <returns>The matching <see cref="Language"/>, or a default instance if not found.</returns>
        Language GetRawLanguageByCode(string code);

        /// <summary>
        /// Gets all available languages as a raw list.
        /// </summary>
        /// <returns>A new <see cref="List{T}"/> containing the current <see cref="Language"/> entries.</returns>
        List<Language> GetRawLanguages();

        /// <summary>
        /// Gets languages that match the provided ISO codes as a raw list.
        /// </summary>
        /// <param name="codes">The list of ISO codes to search for.</param>
        /// <returns>A <see cref="List{T}"/> of matching <see cref="Language"/>.</returns>
        List<Language> GetRawLanguagesByCodes(List<string> codes);

        /// <summary>
        /// Removes a language specified by its ISO code and persists the change.
        /// </summary>
        /// <param name="languageCode">The ISO code of the language to remove.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        Result RemoveLanguageFromList(string languageCode);

        /// <summary>
        /// Updates a language identified by its ISO code and persists the change.
        /// </summary>
        /// <param name="code">The current ISO code of the language to update.</param>
        /// <param name="language">The new values to apply to the language.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        Result UpdateLanguageByCode(string code, Language language);

        /// <summary>
        /// Updates a language identified by its name and persists the change.
        /// </summary>
        /// <param name="name">The current name of the language to update.</param>
        /// <param name="language">The new values to apply to the language.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        Result UpdateLanguageByName(string name, Language language);
    }
}
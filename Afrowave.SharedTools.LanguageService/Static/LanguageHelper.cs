using Afrowave.SharedTools.Models.Localization;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.I18N.Static
{
    /// <summary>
    /// Static helper for working with language metadata backed by a JSON file.
    /// Provides functionality similar to <see cref="Afrowave.SharedTools.I18N.Services.ILanguageService"/>,
    /// but without dependency injection and instance lifetime management.
    /// </summary>
    /// <remarks>
    /// Languages are loaded from a JSON file located at <c>Jsons/languages.json</c> under the application's base directory.
    /// All write operations update the in-memory list and persist the changes back to the JSON file.
    /// </remarks>
    public static class LanguageHelper
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        private static readonly string FilePath =
             Path.Combine(AppContext.BaseDirectory, "Jsons", "languages.json");

        private static List<Language> _languages = new List<Language>();

        static LanguageHelper()
        {
            if(File.Exists(FilePath))
            {
                UpdateLanguagesFromJson();
            }
        }

        // ---------------------------
        // 🧩 READ METHODS
        // ---------------------------

        /// <summary>
        /// Gets a shallow copy of all languages currently loaded in memory.
        /// </summary>
        /// <returns>A new list containing the current <see cref="Language"/> entries.</returns>
        public static List<Language> GetRawLanguages()
        {
            return new List<Language>(_languages);
        }

        /// <summary>
        /// Gets all languages wrapped in a standardized response object.
        /// </summary>
        /// <returns>A <see cref="Response{List{Language}}"/> containing the languages and status information.</returns>
        public static Response<List<Language>> GetLanguages()
        {
            var response = new Response<List<Language>>
            {
                Data = new List<Language>(_languages),
                Success = true,
                Message = "OK"
            };

            if(response.Data == null || response.Data.Count == 0)
            {
                response.Success = false;
                response.Message = "No languages found.";
            }
            return response;
        }

        /// <summary>
        /// Finds a language by its ISO code and returns the raw object.
        /// </summary>
        /// <param name="code">The language ISO code to search for.</param>
        /// <returns>The matching <see cref="Language"/>, or a new empty instance if not found or the input is invalid.</returns>
        public static Language GetRawLanguageByCode(string code)
        {
            if(string.IsNullOrWhiteSpace(code))
                return new Language();

            var language = _languages.Find(lang =>
                 string.Equals(lang.Code, code, StringComparison.OrdinalIgnoreCase));

            return language;
        }

        /// <summary>
        /// Finds a language by its ISO code and returns a standardized response.
        /// </summary>
        /// <param name="code">The language ISO code to search for.</param>
        /// <returns>A <see cref="Response{Language}"/> with the found language or an error message if not found.</returns>
        public static Response<Language> GetLanguageByCode(string code)
        {
            var response = new Response<Language>
            {
                Data = GetRawLanguageByCode(code),
                Success = true,
                Message = "OK"
            };

            if(response.Data == null || string.IsNullOrWhiteSpace(response.Data.Code))
            {
                response.Success = false;
                response.Message = $"Language with code '{code}' not found.";
            }

            return response;
        }

        /// <summary>
        /// Finds all languages matching any of the provided ISO codes and returns a raw list.
        /// </summary>
        /// <param name="codes">The list of language ISO codes to search for.</param>
        /// <returns>A list of matching <see cref="Language"/> entries; empty when none match or no codes are provided.</returns>
        public static List<Language> GetRawLanguagesByCodes(List<string> codes)
        {
            if(codes == null || codes.Count == 0)
                return new List<Language>();

            return _languages.FindAll(lang =>
                 codes.Exists(code => string.Equals(lang.Code, code, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finds languages matching the provided ISO codes and returns a standardized response.
        /// </summary>
        /// <param name="codes">The list of language ISO codes to search for.</param>
        /// <returns>A <see cref="Response{List{Language}}"/> with the matching languages. Includes a warning if some codes are missing.</returns>
        public static Response<List<Language>> GetLanguagesByCodes(List<string> codes)
        {
            var response = new Response<List<Language>>
            {
                Data = GetRawLanguagesByCodes(codes),
                Success = true,
                Message = "OK"
            };

            if(response.Data == null || response.Data.Count == 0)
            {
                response.Success = false;
                response.Message = "No languages found for the provided codes.";
            }

            if(response.Data.Count != codes.Count)
            {
                var foundCodes = new HashSet<string>(
                     response.Data.ConvertAll(lang => lang.Code),
                     StringComparer.OrdinalIgnoreCase);

                var missingCodes = codes.FindAll(code => !foundCodes.Contains(code));
                var missingCodesTxt = string.Join(", ", missingCodes);
                response.Message += $" Missing codes: {missingCodesTxt}";
                response.Warning = true;
            }

            return response;
        }

        // ---------------------------
        // ✍️ WRITE METHODS
        // ---------------------------

        /// <summary>
        /// Adds a new language to the list and persists the change.
        /// </summary>
        /// <param name="language">The language to add.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        public static Result AddLanguageToList(Language language)
        {
            if(language == null)
                return Result.Fail("Language object can't be empty");

            if(string.IsNullOrEmpty(language.Code))
                return Result.Fail("Language code can't be empty");

            if(string.IsNullOrEmpty(language.Name))
                language.Name = language.Code;

            if(_languages.Any(s => s.Code.Equals(language.Code, StringComparison.OrdinalIgnoreCase)))
                return Result.Fail("Language code already exists in the database");

            try
            {
                _languages.Add(language);
                StoreLanguagesToJson();
                return Result.Ok("Language added");
            }
            catch(Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Removes a language identified by its ISO code and persists the change.
        /// </summary>
        /// <param name="languageCode">The ISO code of the language to remove.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        public static Result RemoveLanguageFromList(string languageCode)
        {
            if(string.IsNullOrEmpty(languageCode))
                return Result.Fail("Can't delete the language as the code is empty");

            var toDelete = _languages.FirstOrDefault(s => s.Code.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
            if(toDelete == null)
                return Result.Fail("Language code not found");

            try
            {
                _languages.Remove(toDelete);
                StoreLanguagesToJson();
                return Result.Ok("Language removed");
            }
            catch(Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Updates a language found by its ISO code and persists the change.
        /// </summary>
        /// <param name="code">The current ISO code of the language to update.</param>
        /// <param name="language">The new values to apply.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        public static Result UpdateLanguageByCode(string code, Language language)
        {
            if(string.IsNullOrEmpty(code) || language == null)
                return Result.Fail("Code and language object are required for update");

            var toUpdate = _languages.FirstOrDefault(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            if(toUpdate == null)
                return Result.Fail("Language object not found in the list");

            if(!string.IsNullOrEmpty(language.Name))
                toUpdate.Name = language.Name;

            if(!string.IsNullOrEmpty(language.Native))
                toUpdate.Native = language.Native;

            toUpdate.Rtl = language.Rtl;

            StoreLanguagesToJson();
            return Result.Ok("Language updated successfully");
        }

        /// <summary>
        /// Updates a language found by its name and persists the change.
        /// </summary>
        /// <param name="name">The current name of the language to update.</param>
        /// <param name="language">The new values to apply.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        public static Result UpdateLanguageByName(string name, Language language)
        {
            if(string.IsNullOrEmpty(name) || language == null)
                return Result.Fail("Name and language object are required for update");

            var toUpdate = _languages.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if(toUpdate == null)
                return Result.Fail("Language object not found in the list");

            if(!string.IsNullOrEmpty(language.Code))
                toUpdate.Code = language.Code;

            if(!string.IsNullOrEmpty(language.Native))
                toUpdate.Native = language.Native;

            toUpdate.Rtl = language.Rtl;

            StoreLanguagesToJson();
            return Result.Ok("Language updated successfully");
        }

        // ---------------------------
        // 💾 FILE HANDLING
        // ---------------------------

        private static void UpdateLanguagesFromJson()
        {
            try
            {
                var json = File.ReadAllText(FilePath);
                _languages = JsonSerializer.Deserialize<List<Language>>(json, _options) ?? new List<Language>();
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Failed to load languages from the JSON file.", ex);
            }
        }

        private static Result StoreLanguagesToJson()
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(_languages, _options);
                File.WriteAllText(FilePath, jsonString);
                return Result.Ok("Language file stored successfully");
            }
            catch(Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }
}
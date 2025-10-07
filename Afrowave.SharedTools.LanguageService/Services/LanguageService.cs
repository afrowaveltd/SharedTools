using Afrowave.SharedTools.Models.Localization;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.I18N.Services
{
	/// <summary>
	/// Provides language metadata management backed by a JSON file. Allows reading, querying, adding,
	/// removing, and updating <see cref="Language"/> entries and persists changes to disk.
	/// </summary>
	/// <remarks>
	/// The service loads languages from a JSON file located at <c>Jsons/languages.json</c> under the application's base directory.
	/// All modifying operations update the in-memory list and persist the changes back to the JSON file.
	/// </remarks>
	public class LanguageService : ILanguageService
	{
		private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

		private readonly string filePath = Path.Combine(AppContext.BaseDirectory, "Jsons", "languages.json");
		private List<Language> _languages = new List<Language>();

		/// <summary>
		/// Initializes a new instance of the <see cref="LanguageService"/> class and loads languages from the JSON file.
		/// </summary>
		/// <exception cref="FileNotFoundException">Thrown when the languages JSON file does not exist.</exception>
		public LanguageService()
		{
			if(!File.Exists(filePath))
			{
				throw new FileNotFoundException($"The file at path {filePath} was not found.");
			}
			UpdateLanguagesFromJson();
		}

		/// <summary>
		/// Gets a shallow copy of the in-memory languages list.
		/// </summary>
		/// <returns>A new list containing the current languages.</returns>
		public List<Language> GetRawLanguages()
		{
			var response = new List<Language>(_languages);
			return response;
		}

		/// <summary>
		/// Gets the languages wrapped in a standardized response object.
		/// </summary>
		/// <returns>A response containing the languages list and status information.</returns>
		public Response<List<Language>> GetLanguages()
		{
			var response = new Response<List<Language>>()
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
		/// Finds a language by its code and returns the raw object.
		/// </summary>
		/// <param name="code">The language ISO code to search for.</param>
		/// <returns>The matching language or a new empty instance if not found or input is invalid.</returns>
		public Language GetRawLanguageByCode(string code)
		{
			if(string.IsNullOrWhiteSpace(code))
			{
				return new Language();
			}
			var language = _languages.Find(lang => string.Equals(lang.Code, code, StringComparison.OrdinalIgnoreCase));
			return language;
		}

		/// <summary>
		/// Finds a language by its code and returns a standardized response.
		/// </summary>
		/// <param name="code">The language ISO code to search for.</param>
		/// <returns>A response with the found language or an error message if not found.</returns>
		public Response<Language> GetLanguageByCode(string code)
		{
			var response = new Response<Language>()
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
		/// Finds languages matching any of the provided codes and returns the raw list.
		/// </summary>
		/// <param name="codes">The list of language ISO codes to search for.</param>
		/// <returns>A list of matching languages. Returns an empty list when no codes are provided or none match.</returns>
		public List<Language> GetRawLanguagesByCodes(List<string> codes)
		{
			if(codes == null || codes.Count == 0)
			{
				return new List<Language>();
			}
			var languages = _languages.FindAll(lang => codes.Exists(code => string.Equals(lang.Code, code, StringComparison.OrdinalIgnoreCase)));
			return languages;
		}

		/// <summary>
		/// Finds languages matching the provided codes and returns a standardized response.
		/// </summary>
		/// <param name="codes">The list of language ISO codes to search for.</param>
		/// <returns>A response with matching languages. Includes a warning message if some codes were not found.</returns>
		public Response<List<Language>> GetLanguagesByCodes(List<string> codes)
		{
			var response = new Response<List<Language>>()
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
				var foundCodes = new HashSet<string>(response.Data.ConvertAll(lang => lang.Code), StringComparer.OrdinalIgnoreCase);
				var missingCodes = codes.FindAll(code => !foundCodes.Contains(code));
				response.Message += $" Missing codes: {string.Join(", ", missingCodes)}";
				response.Warning = true;
			}
			return response;
		}

		/// <summary>
		/// Adds a new language to the list and persists the change.
		/// </summary>
		/// <param name="language">The language to add.</param>
		/// <returns>A task that resolves to a result indicating success or failure.</returns>
		public Result AddLanguageToList(Language language)
		{
			if(language == null)
			{
				return Result.Fail("Language object can't be empty");
			}

			if(language.Code == null)
			{
				return Result.Fail("Language code can't be empty");
			}

			if(language.Name == null || language.Name == string.Empty)
			{
				language.Name = language.Code;
			}

			if(_languages.Where(s => s.Code == language.Code).Any())
			{
				return Result.Fail("Language code already exists in the database");
			}

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
		/// Removes a language identified by its code and persists the change.
		/// </summary>
		/// <param name="languageCode">The ISO code of the language to remove.</param>
		/// <returns>A task that resolves to a result indicating success or failure.</returns>
		public Result RemoveLanguageFromList(string languageCode)
		{
			if(string.IsNullOrEmpty(languageCode))
			{
				return Result.Fail("Can't delete the language as the object is null");
			}
			var toDelete = _languages.FirstOrDefault(s => s.Code.Equals(languageCode));
			if(toDelete == null)
			{
				return Result.Fail("Language code not found");
			}
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
		/// Updates a language found by its code and persists the change.
		/// </summary>
		/// <param name="code">The current ISO code of the language to update.</param>
		/// <param name="language">The new language values to apply.</param>
		/// <returns>A task that resolves to a result indicating success or failure.</returns>
		public Result UpdateLanguageByCode(string code, Language language)
		{
			if(string.IsNullOrEmpty(code) || language == null)
			{
				return Result.Fail("Code and language object is required for the update");
			}

			var toUpdate = _languages.FirstOrDefault(s => s.Code.Equals(code));

			if(toUpdate == null)
			{
				return Result.Fail("Language object was not found in the list");
			}

			if(!string.IsNullOrEmpty(language.Name))
			{
				toUpdate.Name = language.Name;
			}
			if(!string.IsNullOrEmpty(language.Native))
			{
				toUpdate.Native = language.Native;
			}

			toUpdate.Rtl = language.Rtl;

			return Result.Ok("Language updated successfully");
		}

		/// <summary>
		/// Updates a language found by its name and persists the change.
		/// </summary>
		/// <param name="name">The current name of the language to update.</param>
		/// <param name="language">The new language values to apply.</param>
		/// <returns>A task that resolves to a result indicating success or failure.</returns>
		public Result UpdateLanguageByName(string name, Language language)
		{
			if(string.IsNullOrEmpty(name) || language == null)
			{
				return Result.Fail("Name and language object is required for the update");
			}

			var toUpdate = _languages.FirstOrDefault(s => s.Name.Equals(name));

			if(toUpdate == null)
			{
				return Result.Fail("Language object was not found in the list");
			}

			if(!string.IsNullOrEmpty(language.Code))
			{
				toUpdate.Code = language.Code;
			}
			if(!string.IsNullOrEmpty(language.Native))
			{
				toUpdate.Native = language.Native;
			}

			toUpdate.Rtl = language.Rtl;

			return Result.Ok("Language updated successfully");
		}

		/// <summary>
		/// Loads languages from the JSON file into the in-memory list.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when the JSON content cannot be deserialized.</exception>
		private void UpdateLanguagesFromJson()
		{
			if(File.Exists(filePath))
			{
				try
				{
					var json = File.ReadAllText(filePath);
					_languages = JsonSerializer.Deserialize<List<Language>>(json, _options) ?? new List<Language>();
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException("Failed to load languages from the JSON file.", ex);
				}
			}
		}

		/// <summary>
		/// Persists the current in-memory languages list to the JSON file.
		/// </summary>
		/// <returns>A result indicating success or failure of the persistence operation.</returns>
		private Result StoreLanguagesToJson()
		{
			try
			{
				var jsonString = JsonSerializer.Serialize(_languages, _options);
				File.WriteAllText(filePath, jsonString);
				return Result.Ok("Language file stored successfully");
			}
			catch(Exception ex)
			{
				return Result.Fail(ex.Message);
			}
		}
	}
}
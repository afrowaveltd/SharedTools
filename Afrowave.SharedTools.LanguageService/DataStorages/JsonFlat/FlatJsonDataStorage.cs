using Afrowave.SharedTools.I18N.DataStorages.JsonFlat.Models;
using Afrowave.SharedTools.I18N.Interfaces;
using Afrowave.SharedTools.I18N.Models;
using Afrowave.SharedTools.I18N.EventHandler;
using Afrowave.SharedTools.Models.Results;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Afrowave.SharedTools.I18N.DataStorages.Services;

namespace Afrowave.SharedTools.I18N.DataStorages.JsonFlat
{
	/// <summary>
	/// Provides a flat-file JSON-based implementation of the <see cref="IDataStorage"/> interface for managing language
	/// dictionaries and translations.
	/// </summary>
	/// <remarks>This class enables storage and retrieval of translation data using JSON files, supporting
	/// operations such as loading, saving, and deleting language dictionaries. It is suitable for scenarios where a
	/// simple, file-based persistence mechanism is preferred over database-backed solutions. Thread safety and performance
	/// characteristics depend on the underlying file system and usage patterns.</remarks>
	public class FlatJsonDataStorage : IDataStorage
	{
		private readonly string _localesPath = string.Empty;
		private bool _createIfNotExists = false;
		private string _defaultLanguage = "en";
		private readonly ICapabilities _capabilities;

		/// <summary>
		/// Occurs when the contents of the dictionary are changed.
		/// </summary>
		/// <remarks>Subscribe to this event to be notified when items are added, removed, or updated in the
		/// dictionary. Event handlers receive information about the specific change that occurred.</remarks>
		public event IDataStorage.DictionaryChangedEventHandler DictionaryChanged;

		/// <summary>
		/// Initializes a new instance of the FlatJsonDataStorage class using the specified configuration options and
		/// capabilities.
		/// </summary>
		/// <param name="options">The configuration options for the JSON flat data storage. If null, default options are used.</param>
		/// <param name="capabilities">The capabilities to be associated with this storage instance. If null, default capabilities are assigned.</param>
#pragma warning disable CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null. Zvažte přidání modifikátoru required nebo deklaraci s možnou hodnotou null.

		public FlatJsonDataStorage(IOptions<JsonFlatDataStorageOptions> options, ICapabilities capabilities)
#pragma warning restore CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null. Zvažte přidání modifikátoru required nebo deklaraci s možnou hodnotou null.
		{
			JsonFlatDataStorageOptions _options = options.Value ?? new JsonFlatDataStorageOptions();
			_localesPath = _options.LocalesPath;
			_createIfNotExists = _options.CreateIfNotExists;
			_defaultLanguage = _options.DefaultLanguage;
			_capabilities = capabilities ?? new Capabilities();
		}

		/// <summary>
		/// Deletes the dictionary file for the specified language code asynchronously.
		/// </summary>
		/// <remarks>If the dictionary file for the specified language code does not exist, the operation fails and
		/// returns an appropriate error message. The method performs input validation and returns failure results for invalid
		/// language codes or if an error occurs during deletion.</remarks>
		/// <param name="languageCode">The language code identifying the dictionary to delete. Must be between 2 and 5 characters long and cannot contain
		/// invalid file name characters. Cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation. The result indicates whether the deletion was successful or
		/// contains an error message if the operation failed.</returns>
		public Task<Result> DeleteDictionaryAsync(string languageCode)
		{
			if(string.IsNullOrWhiteSpace(languageCode))
				return Task.FromResult(Result.Fail("Language code cannot be null or empty."));

			if(languageCode.Length < 2 || languageCode.Length > 10)
				return Task.FromResult(Result.Fail("Language code must be between 2 and 10 characters long."));

			if(languageCode.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
				return Task.FromResult(Result.Fail("Language code contains invalid characters."));

			if(!System.IO.File.Exists(System.IO.Path.Combine(_localesPath, $"{languageCode}.json")))
				return Task.FromResult(Result.Fail("Dictionary for the specified language does not exist."));
			try
			{
				File.Delete(System.IO.Path.Combine(_localesPath, $"{languageCode}.json"));
				return Task.FromResult(Result.Ok());
			}
			catch(Exception ex)
			{
				return Task.FromResult(Result.Fail($"An error occurred while deleting the dictionary: {ex.Message}"));
			}
		}

		/// <summary>
		/// Asynchronously determines whether a dictionary file exists for the specified language code.
		/// </summary>
		/// <param name="languageCode">The language code to check for an existing dictionary file. Must be a non-empty string between 2 and 5 characters
		/// in length.</param>
		/// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if a dictionary file
		/// exists for the specified language code; otherwise, <see langword="false"/>.</returns>
		public Task<bool> DictionaryExistsAsync(string languageCode)
		{
			if(string.IsNullOrWhiteSpace(languageCode))
				return Task.FromResult(false);
			if(languageCode.Length < 2 || languageCode.Length > 10)
				return Task.FromResult(false);
			return Task.FromResult(System.IO.File.Exists(System.IO.Path.Combine(_localesPath, $"{languageCode}.json")));
		}

		/// <summary>
		/// Retrieves the capabilities supported by the current data storage provider.
		/// </summary>
		/// <returns>A <see cref="DataStorageCapabilities"/> object describing the features and limitations of the data storage
		/// provider.</returns>
		public DataStorageCapabilities GetCapabilities()
		{
			return _capabilities.GetCapabilities();
		}

		/// <summary>
		/// Retrieves the localized translation for the specified key in the given language asynchronously.
		/// </summary>
		/// <remarks>If the specified language dictionary does not exist or the key is not found, the response will
		/// indicate failure with an appropriate error message. This method does not throw exceptions for missing files or
		/// keys; errors are reported in the response object.</remarks>
		/// <param name="languageCode">The language code representing the target locale. Must be between 2 and 5 characters long and correspond to an
		/// existing dictionary file. Cannot be null or empty.</param>
		/// <param name="key">The key identifying the text to translate. Cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation. The result contains a <see cref="Response{string}"/> with the
		/// translation value if found; otherwise, a failure response with an error message.</returns>
		public Task<Response<string>> GetTranslation(string languageCode, string key)
		{
			if(string.IsNullOrWhiteSpace(languageCode))
				return Task.FromResult(Response<string>.Fail("Language code cannot be null or empty."));
			if(languageCode.Length < 2 || languageCode.Length > 10)
				return Task.FromResult(Response<string>.Fail("Language code must be between 2 and 10 characters long."));
			if(string.IsNullOrWhiteSpace(key))
				return Task.FromResult(Response<string>.Fail("Key cannot be null or empty."));
			if(!File.Exists(Path.Combine(_localesPath, $"{languageCode}.json")))
				return Task.FromResult(Response<string>.Fail("Dictionary for the specified language does not exist."));
			string filePath = Path.Combine(_localesPath, $"{languageCode}.json");
			try
			{
				string jsonContent = File.ReadAllText(filePath);
				var translations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
				if(translations != null && translations.TryGetValue(key, out string value))
				{
					return Task.FromResult(Response<string>.SuccessResponse(value, "Ok"));
				}
				else
				{
					return Task.FromResult(Response<string>.Fail("Translation key not found."));
				}
			}
			catch(Exception ex)
			{
				return Task.FromResult(Response<string>.Fail($"An error occurred while retrieving the translation: {ex.Message}"));
			}
		}

		/// <summary>
		/// Asynchronously determines whether the current instance is in read-only mode.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the instance is
		/// read-only; otherwise, <see langword="false"/>.</returns>
		public Task<bool> IsReadOnlyAsync()
		{
			return Task.FromResult(_capabilities.GetCapabilities().IsReadOnly);
		}

		/// <summary>
		/// Asynchronously retrieves a list of available language codes based on JSON locale files present in the configured
		/// locales directory.
		/// </summary>
		/// <remarks>Language codes are determined by the names of JSON files in the locales directory, excluding
		/// files with invalid or empty names. If the locales directory does not exist and automatic creation is enabled, the
		/// directory will be created and an empty list will be returned. The operation is safe to call multiple times and
		/// does not modify existing locale files.</remarks>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{List{string}}"/>
		/// object with a list of language codes. If no languages are found, the list will be empty. If the locales directory
		/// is missing or not configured, the response will indicate failure.</returns>
		public Task<Response<List<string>>> ListAvailableLanguagesAsync()
		{
			try
			{
				if(string.IsNullOrWhiteSpace(_localesPath))
					return Task.FromResult(Response<List<string>>.Fail("Locales path is not configured."));

				if(!Directory.Exists(_localesPath))
				{
					if(_createIfNotExists)
					{
						Directory.CreateDirectory(_localesPath);
						return Task.FromResult(Response<List<string>>.SuccessResponse(new List<string>(), "Locales directory created; no languages found."));
					}
					return Task.FromResult(Response<List<string>>.Fail("Locales directory not found."));
				}

				var files = Directory.GetFiles(_localesPath, "*.json");
				var codes = files
					.Select(Path.GetFileNameWithoutExtension)
					.Where(code => !string.IsNullOrWhiteSpace(code))
					// Keep typical language codes (2-10 chars, letters and optional '-')
					.Where(code => code!.Length >= 2 && code.Length <= 10 && code.All(ch => char.IsLetter(ch) || ch == '-'))
					.Select(code => code!.ToLowerInvariant())
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.OrderBy(s => s, StringComparer.Ordinal)
					.ToList();

				return Task.FromResult(Response<List<string>>.SuccessResponse(codes, "OK"));
			}
			catch(Exception ex)
			{
				return Task.FromResult(Response<List<string>>.Fail(ex));
			}
		}

		/// <summary>
		/// Asynchronously loads a dictionary of translations for the specified language code.
		/// </summary>
		/// <remarks>If the translation dictionary file for the specified language code does not exist, the method
		/// returns an empty dictionary with a warning. If an error occurs during loading or deserialization, the response
		/// will contain an error message.</remarks>
		/// <param name="languageCode">The language code identifying which translation dictionary to load. Must be between 2 and 10 characters in length
		/// and cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation. The result contains a response with a dictionary of translation
		/// key-value pairs for the specified language. If the dictionary does not exist or an error occurs, the response will
		/// indicate failure or provide a warning.</returns>
		public Task<Response<Dictionary<string, string>>> LoadDictionaryAsync(string languageCode)
		{
			if(string.IsNullOrWhiteSpace(languageCode))
				return Task.FromResult(Response<Dictionary<string, string>>.Fail("Language code cannot be null or empty."));
			if(languageCode.Length < 2 || languageCode.Length > 10)
				return Task.FromResult(Response<Dictionary<string, string>>.Fail("Language code must be between 2 and 10 characters long."));
			if(!File.Exists(Path.Combine(_localesPath, $"{languageCode}.json")))
				return Task.FromResult(Response<Dictionary<string, string>>.SuccessWithWarning(new Dictionary<string, string>(), "Dictionary doesn't exist"));
			string filePath = Path.Combine(_localesPath, $"{languageCode}.json");
			try
			{
				var dataString = File.ReadAllText(filePath);
				var translations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(dataString);
				if(translations == null)
					return Task.FromResult(Response<Dictionary<string, string>>.SuccessResponse(new Dictionary<string, string>(), "No entries found"));
				return Task.FromResult(Response<Dictionary<string, string>>.SuccessResponse(translations, "OK"));
			}
			catch(Exception ex)
			{
				return Task.FromResult(Response<Dictionary<string, string>>.Fail($"An error occurred while loading the dictionary: {ex.Message}"));
			}
		}

		/// <summary>
		/// Asynchronously saves the specified translations dictionary for the given language code to a JSON file in the
		/// locales directory.
		/// </summary>
		/// <remarks>If the locales directory does not exist and automatic creation is enabled, the directory will be
		/// created. Otherwise, the operation will fail. The translations are serialized as indented JSON for
		/// readability.</remarks>
		/// <param name="languageCode">The language code identifying the locale for which the translations are being saved. Must be between 2 and 10
		/// characters long and cannot be null or empty.</param>
		/// <param name="translations">A dictionary containing key-value pairs representing translation entries for the specified language. If null, an
		/// empty dictionary is saved.</param>
		/// <returns>A task that represents the asynchronous save operation. The result indicates success or failure, including an
		/// error message if the operation fails.</returns>
		public Task<Result> SaveDictionaryAsync(string languageCode, Dictionary<string, string> translations)
		{
			if(string.IsNullOrWhiteSpace(languageCode))
				return Task.FromResult(Result.Fail("Language code cannot be null or empty."));
			if(languageCode.Length < 2 || languageCode.Length > 10)
				return Task.FromResult(Result.Fail("Language code must be between 2 and 10 characters long."));
			if(translations == null)
				translations = new Dictionary<string, string>();
			try
			{
				if(!Directory.Exists(_localesPath))
				{
					if(_createIfNotExists)
						Directory.CreateDirectory(_localesPath);
					else
						return Task.FromResult(Result.Fail("Locales directory does not exist."));
				}
				string filePath = Path.Combine(_localesPath, $"{languageCode}.json");
				string jsonContent = System.Text.Json.JsonSerializer.Serialize(translations, new System.Text.Json.JsonSerializerOptions
				{
					WriteIndented = true
				});
				File.WriteAllText(filePath, jsonContent);
				return Task.FromResult(Result.Ok());
			}
			catch(Exception ex)
			{
				return Task.FromResult(Result.Fail($"An error occurred while saving the dictionary: {ex.Message}"));
			}
		}

		/// <summary>
		/// Adds a new translation entry to the dictionary for the specified language code.
		/// </summary>
		/// <remarks>If the dictionary for the specified language does not exist and automatic creation is enabled, a
		/// new dictionary will be created. Otherwise, the operation will fail. If the key already exists, the method returns
		/// a failure result. The method triggers the DictionaryChanged event upon successful addition.</remarks>
		/// <param name="languageCode">The language code identifying the target dictionary. Must be between 2 and 10 characters long and cannot be null
		/// or empty.</param>
		/// <param name="key">The key for the translation entry to add. Cannot be null or empty. Must not already exist in the dictionary.</param>
		/// <param name="value">The translation value to associate with the specified key. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous operation. The result indicates success or failure, including an error
		/// message if the operation fails.</returns>
		public Task<Result> AddTranslation(string languageCode, string key, string value)
		{
			if(string.IsNullOrWhiteSpace(languageCode))
				return Task.FromResult(Result.Fail("Language code cannot be null or empty."));
			if(languageCode.Length < 2 || languageCode.Length > 10)
				return Task.FromResult(Result.Fail("Language code must be between 2 and 10 characters long."));
			if(string.IsNullOrWhiteSpace(key))
				return Task.FromResult(Result.Fail("Key cannot be null or empty."));
			if(value == null)
				return Task.FromResult(Result.Fail("Value cannot be null."));
			if(!File.Exists(Path.Combine(_localesPath, $"{languageCode}.json")))
			{
				if(_createIfNotExists)
				{
					var createResult = SaveDictionaryAsync(languageCode, new Dictionary<string, string>()).Result;
					if(!createResult.Success)
						return Task.FromResult(Result.Fail($"Failed to create dictionary for language '{languageCode}': {createResult.Message}"));
				}
				else
				{
					return Task.FromResult(Result.Fail("Dictionary for the specified language does not exist."));
				}
			}
			string filePath = Path.Combine(_localesPath, $"{languageCode}.json");
			try
			{
				string jsonContent = File.ReadAllText(filePath);
				var translations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent)
					?? new Dictionary<string, string>();
				if(translations.ContainsKey(key))
					return Task.FromResult(Result.Fail("The specified key already exists in the dictionary."));
				translations[key] = value;
				string updatedJson = System.Text.Json.JsonSerializer.Serialize(translations, new System.Text.Json.JsonSerializerOptions
				{
					WriteIndented = true
				});
				File.WriteAllText(filePath, updatedJson);
				DictionaryChanged?.Invoke(this, new DictionaryChangedEventArgs(languageCode));
				return Task.FromResult(Result.Ok());
			}
			catch(Exception ex)
			{
				return Task.FromResult(Result.Fail($"An error occurred while adding the translation: {ex.Message}"));
			}
		}

		/// <summary>
		/// Updates or adds a translation entry for the specified language and key.
		/// </summary>
		/// <remarks>If the translation dictionary for the specified language does not exist and automatic creation is
		/// enabled, a new dictionary will be created. Otherwise, the operation will fail. The method triggers a dictionary
		/// change event upon successful update.</remarks>
		/// <param name="languageCode">The language code identifying the target translation dictionary. Must be between 2 and 10 characters and cannot be
		/// null or empty.</param>
		/// <param name="key">The key for the translation entry to update or add. Cannot be null or empty.</param>
		/// <param name="value">The translation value to associate with the specified key. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous operation. The result indicates success or failure, including an error
		/// message if the update could not be completed.</returns>
		public Task<Result> UpdateTranslation(string languageCode, string key, string value)
		{
			if(string.IsNullOrWhiteSpace(languageCode))
				return Task.FromResult(Result.Fail("Language code cannot be null or empty."));
			if(languageCode.Length < 2 || languageCode.Length > 10)
				return Task.FromResult(Result.Fail("Language code must be between 2 and 10 characters long."));
			if(string.IsNullOrWhiteSpace(key))
				return Task.FromResult(Result.Fail("Key cannot be null or empty."));
			if(value == null)
				return Task.FromResult(Result.Fail("Value cannot be null."));
			if(!File.Exists(Path.Combine(_localesPath, $"{languageCode}.json")))
			{
				if(_createIfNotExists)
				{
					var createResult = SaveDictionaryAsync(languageCode, new Dictionary<string, string>()).Result;
					if(!createResult.Success)
						return Task.FromResult(Result.Fail($"Failed to create dictionary for language '{languageCode}': {createResult.Message}"));
				}
				else
				{
					return Task.FromResult(Result.Fail("Dictionary for the specified language does not exist."));
				}
			}
			string filePath = Path.Combine(_localesPath, $"{languageCode}.json");
			try
			{
				string jsonContent = File.ReadAllText(filePath);
				var translations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent)
					?? new Dictionary<string, string>();
				translations[key] = value;
				string updatedJson = System.Text.Json.JsonSerializer.Serialize(translations, new System.Text.Json.JsonSerializerOptions
				{
					WriteIndented = true
				});
				File.WriteAllText(filePath, updatedJson);
				DictionaryChanged?.Invoke(this, new DictionaryChangedEventArgs(languageCode));
				return Task.FromResult(Result.Ok());
			}
			catch(Exception ex)
			{
				return Task.FromResult(Result.Fail($"An error occurred while adding the translation: {ex.Message}"));
			}
		}

		/// <summary>
		/// Removes a translation entry for the specified language and key from the local dictionary file.
		/// </summary>
		/// <remarks>If the specified key does not exist in the dictionary, the method completes successfully but no
		/// changes are made to the file. The <c>DictionaryChanged</c> event is raised after a successful removal.</remarks>
		/// <param name="lanugageCode">The language code identifying the dictionary to modify. Must be between 2 and 10 characters in length and cannot
		/// be null or empty.</param>
		/// <param name="key">The key of the translation entry to remove. Cannot be null or empty.</param>
		/// <returns>A <see cref="Result"/> indicating whether the removal was successful. Returns a failure result if the language
		/// code or key is invalid, the dictionary file does not exist, or an error occurs during removal.</returns>
		public async Task<Result> RemoveTranslation(string lanugageCode, string key)
		{
			if(string.IsNullOrWhiteSpace(lanugageCode))
				return Result.Fail("Language code cannot be null or empty.");
			if(lanugageCode.Length < 2 || lanugageCode.Length > 10)
				return Result.Fail("Language code must be between 2 and 10 characters long.");
			if(string.IsNullOrWhiteSpace(key))
				return Result.Fail("Key cannot be null or empty.");
			if(!File.Exists(Path.Combine(_localesPath, $"{lanugageCode}.json")))
				return Result.Fail("Dictionary for the specified language does not exist.");
			string filePath = Path.Combine(_localesPath, $"{lanugageCode}.json");
			if(!File.Exists(filePath))
				return Result.Fail("Dictionary file does not exist.");
			try
			{
				string jsonString = File.ReadAllText(filePath);
				Dictionary<string, string> dictionary
					= JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString)
					?? new Dictionary<string, string>();
				dictionary.Remove(key);

				jsonString = JsonSerializer.Serialize(dictionary);
				File.WriteAllText(filePath, jsonString);
				DictionaryChanged?.Invoke(this, new DictionaryChangedEventArgs(lanugageCode));
				return Result.Ok();
			}
			catch(Exception ex)
			{
				return Result.Fail($"An error occurred while removing the translation: {ex.Message}");
			}
		}
	}
}
using Afrowave.SharedTools.I18N.DataStorages.JsonFlat.Models;
using Afrowave.SharedTools.I18N.DataStorages.JsonFlat.Services;
using Afrowave.SharedTools.I18N.Interfaces;
using Afrowave.SharedTools.I18N.Models;
using Afrowave.SharedTools.Models.Results;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.I18N.DataStorages
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
		/// Initializes a new instance of the FlatJsonDataStorage class using the specified configuration options.
		/// </summary>
		/// <remarks>If the provided options are null, default values are applied for all configuration settings. This
		/// constructor is typically used for dependency injection scenarios where options are supplied via
		/// IOptions.</remarks>
		/// <param name="options">The configuration options for the JSON flat data storage. If null, default options are used.</param>
		public FlatJsonDataStorage(IOptions<JsonFlatDataStorageOptions> options, ICapabilities capabilities)
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

		public Task<Response<Dictionary<string, string>>> LoadDictionaryAsync(string languageCode)
		{
			throw new NotImplementedException();
		}

		public Task<Result> SaveDictionaryAsync(string languageCode, Dictionary<string, string> translations)
		{
			throw new NotImplementedException();
		}
	}
}
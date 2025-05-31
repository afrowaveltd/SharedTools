using Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Models;
using Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Options;
using Afrowave.SharedTools.Models.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Services
{
	/// <summary>
	/// Main service that implements localization backend logic using JSON files.
	/// </summary>
	public sealed class JsonLocalizationBackend
	{
		private readonly JsonFileProvider _fileProvider;
		private readonly JsonReaderService _reader;
		private readonly JsonBackendOptions _options;
		private readonly ILogger<JsonLocalizationBackend> _logger;

		/// <summary>
		/// Constructor for the JsonLocalizationBackend service.
		/// </summary>
		/// <param name="fileProvider">The JSON file provider.</param>
		/// <param name="reader">The service to read JSON content.</param>
		/// <param name="options">Options for the JSON backend service.</param>
		/// <param name="logger">Logger for logging information and warnings.</param>
		public JsonLocalizationBackend(
			 JsonFileProvider fileProvider,
			 JsonReaderService reader,
			 JsonBackendOptions options,
			 ILogger<JsonLocalizationBackend> logger)
		{
			_fileProvider = fileProvider;
			_reader = reader;
			_options = options;
			_logger = logger;
		}

		/// <summary>
		/// Reads flat translation dictionary for the specified language.
		/// </summary>
		public async Task<Dictionary<string, string>?> ReadFlatAsync(string langCode)
		{
			var filePath = GetPathForLanguage(langCode);

			_logger.LogInformation("Attempting to read flat translation for language: {LangCode}", langCode);

			var content = await _fileProvider.ReadFileAsync(filePath);

			if(string.IsNullOrWhiteSpace(content))
			{
				_logger.LogWarning("No content found for language '{LangCode}' at path '{Path}'", langCode, filePath);
				return null;
			}

			var result = await _reader.ParseAsync(content);

			if(!result.IsValid)
			{
				_logger.LogWarning("Invalid translation content for '{LangCode}': {Error}", langCode, result.ErrorMessage);
				return null;
			}

			if(result.Flat != null)
			{
				_logger.LogInformation("Successfully read flat translation for '{LangCode}' with {Count} keys.", langCode, result.Flat.Count);
				return result.Flat;
			}

			_logger.LogWarning("Expected flat structure for '{LangCode}' but got structured JSON.", langCode);
			return null;
		}

		/// <summary>
		/// Gets a list of available languages from the translation files.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation, containing a list of available languages.</returns>
		public async Task<List<string>> GetAvailableLanguagesAsync()
		{
			_logger.LogInformation("Scanning for available translation files in {Folder}", _options.TranslationFolder);

			var files = await _fileProvider.GetFilesAsync(_options.TranslationFolder, _options.FilePattern.Replace("{lang}", "*"));

			if(files.Length == 0)
			{
				_logger.LogWarning("No translation files found.");
				return new List<string>();
			}

			var list = new List<string>();

			foreach(var file in files)
			{
				var fileName = Path.GetFileNameWithoutExtension(file);
				var langCode = ExtractLanguageCode(fileName);

				if(!string.IsNullOrWhiteSpace(langCode) && !list.Contains(langCode))
				{
					list.Add(langCode);
				}
			}

			_logger.LogInformation("Found {Count} available languages: {Langs}", list.Count, string.Join(", ", list));
			return list;
		}

		/// <summary>
		/// Reads and parses a structured translation file for the specified language code.
		/// </summary>
		/// <remarks>This method attempts to locate and read a translation file based on the provided language code. If
		/// the file is found and contains valid structured content, it is parsed into a <see cref="TranslationTree"/>.
		/// Otherwise, <see langword="null"/> is returned, and appropriate warnings are logged.</remarks>
		/// <param name="langCode">The language code identifying the translation file to read. Must be a valid language code.</param>
		/// <returns>A <see cref="TranslationTree"/> representing the structured translation data, or <see langword="null"/> if the file
		/// is empty, invalid, or contains a flat structure instead of the expected hierarchical format.</returns>
		public async Task<TranslationTree?> ReadStructuredAsync(string langCode)
		{
			var filePath = GetPathForLanguage(langCode);

			_logger.LogInformation("Attempting to read structured translation for language: {LangCode}", langCode);

			var content = await _fileProvider.ReadFileAsync(filePath);

			if(string.IsNullOrWhiteSpace(content))
			{
				_logger.LogWarning("No content found for language '{LangCode}' at path '{Path}'", langCode, filePath);
				return null;
			}

			var result = await _reader.ParseAsync(content);

			if(!result.IsValid)
			{
				_logger.LogWarning("Invalid translation content for '{LangCode}': {Error}", langCode, result.ErrorMessage);
				return null;
			}

			if(result.Tree != null)
			{
				_logger.LogInformation("Successfully read structured translation for '{LangCode}'", langCode);
				return result.Tree;
			}

			_logger.LogWarning("Expected structured JSON for '{LangCode}' but got flat structure.");
			return null;
		}

		/// <summary>
		/// Writes flat translation data to a file for the specified language code.
		/// </summary>
		/// <param name="langCode">The language code for which the translation data is being written.</param>
		/// <param name="data">A dictionary containing the translation key-value pairs.</param>
		/// <returns>Returns true if the writing operation succeeded; otherwise, false.</returns>
		public async Task<bool> WriteFlatAsync(string langCode, Dictionary<string, string> data)
		{
			var filePath = GetPathForLanguage(langCode);

			try
			{
				_logger.LogInformation("Writing flat translation for language '{LangCode}' to path: {Path}", langCode, filePath);

				var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
				{
					WriteIndented = true
				});

				await _fileProvider.WriteFileAsync(filePath, json);

				_logger.LogInformation("Successfully wrote translation for '{LangCode}' with {Count} keys.", langCode, data.Count);
				return true;
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Failed to write flat translation for '{LangCode}'");
				return false;
			}
		}

		/// <summary>
		/// Writes structured translation data to a file for the specified language code.
		/// </summary>
		/// <param name="langCode">The language code for which the translation data is being written.</param>
		/// <param name="tree">A structured translation tree containing the translation data.</param>
		/// <returns>Returns true if the writing operation succeeded; otherwise, false.</returns>
		public async Task<bool> WriteStructuredAsync(string langCode, TranslationTree tree)
		{
			var filePath = GetPathForLanguage(langCode);

			try
			{
				_logger.LogInformation("Writing structured translation for language '{LangCode}' to path: {Path}", langCode, filePath);

				var serializableTree = ConvertTreeToDictionary(tree);

				var json = JsonSerializer.Serialize(serializableTree, new JsonSerializerOptions
				{
					WriteIndented = true
				});

				await _fileProvider.WriteFileAsync(filePath, json);

				_logger.LogInformation("Successfully wrote structured translation for '{LangCode}'.", langCode);
				return true;
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Failed to write structured translation for '{LangCode}'");
				return false;
			}
		}

		/// <summary>
		/// Updates a translation key-value pair for the specified language.
		/// </summary>
		/// <remarks>If <paramref name="createIfMissing"/> is <see langword="false"/> and the specified key does not
		/// exist, the operation will fail and return a response with an error message.</remarks>
		/// <param name="langCode">The language code identifying the translation file to update. Cannot be null or empty.</param>
		/// <param name="key">The key to update or add in the translation file. Cannot be null or empty.</param>
		/// <param name="value">The value to associate with the specified key. Cannot be null.</param>
		/// <param name="createIfMissing">A boolean value indicating whether the key should be created if it does not exist. <see langword="true"/> to
		/// create the key if it is missing; otherwise, <see langword="false"/>.</param>
		/// <returns>A <see cref="Response{T}"/> object containing a boolean value indicating whether the operation succeeded. Returns
		/// <see langword="true"/> if the key was successfully updated or added; otherwise, <see langword="false"/>.</returns>
		public async Task<Response<bool>> UpdateFlatKeyAsync(string langCode, string key, string value, bool createIfMissing = true)
		{
			try
			{
				var dict = await ReadFlatAsync(langCode) ?? new Dictionary<string, string>();

				if(!dict.ContainsKey(key) && !createIfMissing)
				{
					var msg = $"Key '{key}' not found in '{langCode}' and creation is disabled.";
					_logger.LogWarning(msg);
					return Response<bool>.Fail(msg);
				}

				dict[key] = value;

				var success = await WriteFlatAsync(langCode, dict);

				if(!success)
					return Response<bool>.Fail($"Failed to write translation file for '{langCode}'.");

				_logger.LogInformation("Key '{Key}' updated in language '{LangCode}'.", key, langCode);
				return Response<bool>.SuccessResponse(true, $"Key '{key}' updated in '{langCode}'.");
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Exception occurred while updating key '{Key}' in '{LangCode}'", key, langCode);
				return Response<bool>.Fail(ex);
			}
		}

		/// <summary>
		/// Removes the specified translation key from the flat localization data for the given language code.
		/// </summary>
		/// <param name="langCode">The language code from which to remove the key.</param>
		/// <param name="key">The key to remove.</param>
		/// <returns>A <see cref="Response{T}"/> object indicating the success or failure of the operation.</returns>
		public async Task<Response<bool>> RemoveFlatKeyAsync(string langCode, string key)
		{
			try
			{
				var dict = await ReadFlatAsync(langCode);

				if(dict == null || !dict.ContainsKey(key))
				{
					var msg = $"Key '{key}' not found in language '{langCode}'.";
					_logger.LogWarning(msg);
					return Response<bool>.Fail(msg);
				}

				dict.Remove(key);

				var success = await WriteFlatAsync(langCode, dict);
				if(!success)
					return Response<bool>.Fail($"Failed to save translation after removing '{key}' from '{langCode}'.");

				_logger.LogInformation("Key '{Key}' removed from language '{LangCode}'.", key, langCode);
				return Response<bool>.SuccessResponse(true, $"Key '{key}' removed from '{langCode}'.");
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Exception while removing key '{Key}' in '{LangCode}'", key, langCode);
				return Response<bool>.Fail(ex);
			}
		}

		/// <summary>
		/// Removes the structured key from the localization data.
		/// </summary>
		/// <param name="langCode">The language code to identify the structure.</param>
		/// <param name="path">The path of the structured key to be removed.</param>
		/// <returns>A response indicating success or failure.</returns>
		public async Task<Response<bool>> RemoveStructuredKeyAsync(string langCode, string[] path)
		{
			try
			{
				var tree = await ReadStructuredAsync(langCode);

				if(tree == null)
					return Response<bool>.Fail($"No structured translation found for language '{langCode}'.");

				var node = tree;
				Stack<TranslationTree> stack = new Stack<TranslationTree>();

				foreach(var segment in path)
				{
					if(!node.Children.ContainsKey(segment))
						return Response<bool>.Fail($"Path '{string.Join(".", path)}' not found in '{langCode}'.");

					stack.Push(node);
					node = node.Children[segment];
				}

				// Now remove the last node
				var lastKey = path.Last();
				var parent = stack.Pop();
				parent.Children.Remove(lastKey);

				var success = await WriteStructuredAsync(langCode, tree);
				if(!success)
					return Response<bool>.Fail($"Failed to write structured file after removing path '{string.Join(".", path)}'");

				_logger.LogInformation("Structured key '{Path}' removed from language '{LangCode}'.", string.Join(".", path), langCode);
				return Response<bool>.SuccessResponse(true, $"Structured key removed from '{langCode}'.");
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Exception while removing structured key '{Path}' in '{LangCode}'", string.Join(".", path), langCode);
				return Response<bool>.Fail(ex);
			}
		}

		/// <summary>
		/// Removes the translation file for the specified language code.
		/// </summary>
		/// <remarks>This method attempts to delete the translation file associated with the given language code.  If
		/// the file does not exist, the operation will fail and return a response indicating the error.  Any exceptions
		/// encountered during the deletion process are logged and included in the failure  response.</remarks>
		/// <param name="langCode">The language code identifying the translation file to remove.  This parameter cannot be null or empty.</param>
		/// <returns>A <see cref="Response{T}"/> object containing a boolean value indicating whether the operation  was successful.
		/// Returns <see langword="true"/> if the file was successfully deleted; otherwise,  <see langword="false"/>. If the
		/// file does not exist, the response will indicate failure with an  appropriate error message.</returns>
		public async Task<Response<bool>> RemoveLanguageAsync(string langCode)
		{
			try
			{
				var path = GetPathForLanguage(langCode);
				if(!File.Exists(path))
					return Response<bool>.Fail($"File for language '{langCode}' does not exist.");

				File.Delete(path);

				_logger.LogInformation("Translation file for language '{LangCode}' deleted.", langCode);
				return Response<bool>.SuccessResponse(true, $"Language '{langCode}' removed.");
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Error deleting translation file for '{LangCode}'", langCode);
				return Response<bool>.Fail(ex);
			}
		}

		/// <summary>
		/// Creates an empty language translation file for the specified language code.
		/// </summary>
		/// <remarks>If a translation file for the specified language code already exists, the operation will fail
		/// and return a response indicating the error. This method logs the creation of the file or any  errors encountered
		/// during the process.</remarks>
		/// <param name="langCode">The language code for which the empty translation file should be created. Must be a valid language code and cannot
		/// be null or empty.</param>
		/// <returns>A <see cref="Response{T}"/> object containing a boolean value indicating success or failure. Returns <see
		/// langword="true"/> if the language file was successfully created; otherwise,  <see langword="false"/>. The response
		/// also includes a message describing the result.</returns>
		public async Task<Response<bool>> AddEmptyLanguageAsync(string langCode)
		{
			try
			{
				var path = GetPathForLanguage(langCode);

				if(File.Exists(path))
					return Response<bool>.Fail($"Language '{langCode}' already exists.");

				var json = JsonSerializer.Serialize(new Dictionary<string, string>(), new JsonSerializerOptions
				{
					WriteIndented = true
				});

				await _fileProvider.WriteFileAsync(path, json);

				_logger.LogInformation("Created empty translation for language '{LangCode}'.", langCode);
				return Response<bool>.SuccessResponse(true, $"Language '{langCode}' created.");
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Error creating new language '{LangCode}'", langCode);
				return Response<bool>.Fail(ex);
			}
		}

		private Dictionary<string, object> ConvertTreeToDictionary(TranslationTree tree)
		{
			var result = new Dictionary<string, object>();

			foreach(var kvp in tree.Children)
			{
				if(kvp.Value.Children.Count > 0)
				{
					result[kvp.Key] = ConvertTreeToDictionary(kvp.Value);
				}
				else
				{
					result[kvp.Key] = kvp.Value.Value ?? string.Empty;
				}
			}

			return result;
		}

		private string ExtractLanguageCode(string fileName)
		{
			// Example: if FilePattern is "{lang}.json" → we assume the lang is whole fileName
			// In future we could support Regex or custom extractor
			if(_options.FilePattern == "{lang}.json")
				return fileName;

			// fallback
			return fileName;
		}

		private string GetPathForLanguage(string langCode)
		{
			var fileName = _options.FilePattern.Replace("{lang}", langCode);
			return Path.Combine(_options.TranslationFolder, fileName);
		}
	}
}
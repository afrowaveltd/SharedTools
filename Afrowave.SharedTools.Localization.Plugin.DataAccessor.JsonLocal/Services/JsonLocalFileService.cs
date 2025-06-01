using Afrowave.SharedTools.Localization.Plugin.DataAccessor.JsonLocal.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Plugin.DataAccessor.JsonLocal.Services
{
	/// <summary>
	/// Service for reading and writing JSON translation files, including autodetection of the folder.
	/// </summary>
	public sealed class JsonLocalFileService
	{
		private readonly JsonLocalOptions _options;
		private readonly ILogger<JsonLocalFileService> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonLocalFileService"/> class.
		/// </summary>
		/// <remarks>This constructor sets up the service with the specified options and logger. Ensure that the
		/// <paramref name="options"/> parameter is properly configured before using the service.</remarks>
		/// <param name="options">The configuration options for the service, including file paths and other settings.</param>
		/// <param name="logger">The logger instance used to log diagnostic and operational information.</param>
		public JsonLocalFileService(JsonLocalOptions options, ILogger<JsonLocalFileService> logger)
		{
			_options = options;
			_logger = logger;
		}

		/// <summary>
		/// Finds the effective folder for translations, using autodetection if enabled.
		/// </summary>
		public string GetTranslationFolder()
		{
			if(!_options.EnableAutoDetectFolder)
				return _options.TranslationFolder;

			var folder = AutoDetectTranslationFolder();
			_logger.LogInformation("Using translation folder: {Folder}", folder);
			return folder;
		}

		/// <summary>
		/// Tries to find 'Locales' folder from current directory upwards (max search depth).
		/// </summary>
		private string AutoDetectTranslationFolder()
		{
			var current = Directory.GetCurrentDirectory();
			for(int depth = 0; depth < _options.AutoDetectSearchDepth; depth++)
			{
				var locales = Path.Combine(current, "Locales");
				if(Directory.Exists(locales))
					return locales;

				var parent = Directory.GetParent(current);
				if(parent == null)
					break;
				current = parent.FullName;
			}
			// fallback
			return _options.TranslationFolder;
		}

		/// <summary>
		/// Asynchronously reads JSON file content for a language code.
		/// </summary>
		public async Task<string?> ReadJsonFileAsync(string langCode)
		{
			var folder = GetTranslationFolder();
			var fileName = _options.FilePattern.Replace("{lang}", langCode);
			var path = Path.Combine(folder, fileName);

			if(!File.Exists(path))
			{
				_logger.LogWarning("Translation file not found: {Path}", path);
				return null;
			}

			using var reader = new StreamReader(path, Encoding.UTF8);
			return await reader.ReadToEndAsync();
		}

		/// <summary>
		/// Asynchronously writes JSON content to file for a language code.
		/// </summary>
		public async Task WriteJsonFileAsync(string langCode, string json)
		{
			var folder = GetTranslationFolder();
			Directory.CreateDirectory(folder);
			var fileName = _options.FilePattern.Replace("{lang}", langCode);
			var path = Path.Combine(folder, fileName);

			using var writer = new StreamWriter(path, false, Encoding.UTF8);
			await writer.WriteAsync(json);

			_logger.LogInformation("Written translation file: {Path}", path);
		}

		/// <summary>
		/// Asynchronously reads all available language files as a dictionary [langCode, rawJson].
		/// </summary>
		public async Task<Dictionary<string, string>> ReadAllLanguagesAsync()
		{
			var folder = GetTranslationFolder();
			var result = new Dictionary<string, string>();
			if(!Directory.Exists(folder))
				return result;

			var pattern = _options.FilePattern.Replace("{lang}", "*");
			var files = Directory.GetFiles(folder, pattern);

			foreach(var file in files)
			{
				var lang = Path.GetFileNameWithoutExtension(file);
				try
				{
					using var reader = new StreamReader(file, Encoding.UTF8);
					var json = await reader.ReadToEndAsync();
					result[lang] = json;
				}
				catch(Exception ex)
				{
					_logger.LogError(ex, "Failed to read language file {File}", file);
				}
			}

			return result;
		}

		/// <summary>
		/// Asynchronously writes multiple languages to files (batch operation).
		/// </summary>
		public async Task WriteLanguagesAsync(Dictionary<string, string> languages)
		{
			var folder = GetTranslationFolder();
			Directory.CreateDirectory(folder);

			foreach(var kv in languages)
			{
				var langCode = kv.Key;
				var json = kv.Value;
				var fileName = _options.FilePattern.Replace("{lang}", langCode);
				var path = Path.Combine(folder, fileName);

				try
				{
					using var writer = new StreamWriter(path, false, Encoding.UTF8);
					await writer.WriteAsync(json);
					_logger.LogInformation("Written translation file: {Path}", path);
				}
				catch(Exception ex)
				{
					_logger.LogError(ex, "Failed to write language file {Path}", path);
				}
			}
		}

		/// <summary>
		/// Gets a list of all available languages (based on file names).
		/// </summary>
		public List<string> GetAvailableLanguages()
		{
			var folder = GetTranslationFolder();
			if(!Directory.Exists(folder))
				return new List<string>();

			var pattern = _options.FilePattern.Replace("{lang}", "*");
			var files = Directory.GetFiles(folder, pattern);

			var langs = files
				 .Select(f => Path.GetFileNameWithoutExtension(f))
				 .ToList();

			return langs;
		}
	}
}
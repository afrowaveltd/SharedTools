using Afrowave.SharedTools.Localization.Helpers;
using Afrowave.SharedTools.Localization.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Services.Di
{
	/// <summary>
	/// Provides functionality for managing and persisting localization settings.
	/// </summary>
	/// <remarks>This service allows asynchronous retrieval, modification, and resetting of localization settings.
	/// The settings are stored in a JSON file, and changes are automatically persisted to the file.</remarks>
	public class LocalizationSettingsService : ILocalizationSettingsService
	{
		private static string _settingsPath = string.Empty;

		private LocalizationSettings? _settings;
		private readonly object _lock = new object();

		/// <summary>
		/// The constructor initializes the service with a specified path for the localization settings JSON file.
		/// </summary>
		/// <param name="settingsPath"></param>
		public LocalizationSettingsService(string settingsPath = "")
		{
			_settingsPath = string.IsNullOrEmpty(settingsPath)
				? PathHelper.ResolveJsonsFile("localization_settings.json") : settingsPath;
			// Load is always async, user must call one of the async methods
		}

		public async Task<LocalizationSettings> GetSettingsAsync()
		{
			await EnsureLoadedAsync();
			lock(_lock) { return Clone(_settings!); }
		}

		public async Task UpdateAsync(Func<LocalizationSettings, Task> update)
		{
			await EnsureLoadedAsync();
			await update(_settings!);
			await SaveAsync();
		}

		public async Task ResetToDefaultAsync()
		{
			lock(_lock) { _settings = new LocalizationSettings(); }
			await SaveAsync();
		}

		private async Task EnsureLoadedAsync()
		{
			if(_settings == null)
			{
				if(File.Exists(_settingsPath))
				{
					var json = await File.ReadAllTextAsync(_settingsPath);
					_settings = JsonSerializer.Deserialize<LocalizationSettings>(json) ?? new LocalizationSettings();
				}
				else
				{
					_settings = new LocalizationSettings();
				}
			}
		}

		private async Task SaveAsync()
		{
			var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
			await File.WriteAllTextAsync(_settingsPath, json);
		}

		private static LocalizationSettings Clone(LocalizationSettings src)
		{
			var json = JsonSerializer.Serialize(src);
			return JsonSerializer.Deserialize<LocalizationSettings>(json)!;
		}
	}
}
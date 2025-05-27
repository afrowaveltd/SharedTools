using Afrowave.SharedTools.Localization.Helpers;
using Afrowave.SharedTools.Localization.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Services.Static
{
	/// <summary>
	/// Static service for managing localization settings (localization_settings.json) in a thread-safe and async manner.
	/// Uses /Jsons/localization_settings.json by default (next to the library), but allows path override.
	/// If the settings file is missing, it is auto-created with defaults on first access.
	/// </summary>
	public static class LocalizationSettingsStatic
	{
		private static string _settingsPath = PathHelper.ResolveJsonsFile("localization_settings.json");
		private static LocalizationSettings? _settings;
		private static readonly object _lock = new object();

		/// <summary>
		/// Changes the location of the settings file. Next request will reload from the new path.
		/// </summary>
		/// <param name="path">Absolute or relative path, or empty for default (/Jsons/localization_settings.json).</param>
		public static void SetSettingsPath(string path)
		{
			lock(_lock)
			{
				_settingsPath = string.IsNullOrEmpty(path)
					 ? PathHelper.ResolveJsonsFile("localization_settings.json")
					 : PathHelper.ResolveJsonsFile(path);
				_settings = null;
			}
		}

		/// <summary>
		/// Resets all static state – for test use (cache, path).
		/// </summary>
		public static void ResetStaticState()
		{
			lock(_lock)
			{
				_settings = null;
				_settingsPath = PathHelper.ResolveJsonsFile("localization_settings.json");
			}
		}

		/// <summary>
		/// Gets a deep clone of the current settings, always up to date.
		/// If the settings file is missing, creates it with defaults.
		/// </summary>
		public static async Task<LocalizationSettings> GetSettingsAsync()
		{
			await EnsureLoadedAsync();
			lock(_lock) { return Clone(_settings!); }
		}

		/// <summary>
		/// Updates settings using the provided async action, then saves changes.
		/// </summary>
		public static async Task UpdateAsync(Func<LocalizationSettings, Task> update)
		{
			await EnsureLoadedAsync();
			lock(_lock)
			{
				var task = update(_settings!);
				task.GetAwaiter().GetResult();
			}
			await SaveAsync();
		}

		/// <summary>
		/// Resets settings to defaults and saves to file.
		/// </summary>
		public static async Task ResetToDefaultAsync()
		{
			lock(_lock)
			{
				_settings = new LocalizationSettings();
			}
			await SaveAsync();
		}

		/// <summary>
		/// Returns the actual full path to the settings file for diagnostics/UI.
		/// </summary>
		public static string SettingsPath
		{
			get
			{
				lock(_lock) { return _settingsPath; }
			}
		}

		private static async Task EnsureLoadedAsync()
		{
			bool needToSave = false;
			lock(_lock)
			{
				if(_settings == null)
				{
					var dir = Path.GetDirectoryName(_settingsPath) ?? "";
					if(!Directory.Exists(dir))
						Directory.CreateDirectory(dir);

					if(File.Exists(_settingsPath))
					{
						var json = File.ReadAllText(_settingsPath);
						_settings = JsonSerializer.Deserialize<LocalizationSettings>(json)
										?? new LocalizationSettings();
					}
					else
					{
						_settings = new LocalizationSettings();
						needToSave = true;
					}
				}
			}
			if(needToSave)
				await SaveAsync();
		}

		private static async Task SaveAsync()
		{
			string json;
			string path;
			lock(_lock)
			{
				json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
				path = _settingsPath;
			}
			var dir = Path.GetDirectoryName(path) ?? "";
			if(!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			await File.WriteAllTextAsync(path, json);
		}

		private static LocalizationSettings Clone(LocalizationSettings src)
		{
			var json = JsonSerializer.Serialize(src);
			return JsonSerializer.Deserialize<LocalizationSettings>(json)!;
		}
	}
}
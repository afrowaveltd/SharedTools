using System;

namespace Afrowave.SharedTools.Localization.Models
{
	/// <summary>
	/// Defines global cache-related settings for the localization system.
	/// Used as a nested object in LocalizationSettings.
	/// </summary>
	public class LocalizationCacheSettings
	{
		/// <summary>
		/// Enables global in-memory cache for resolved translations.
		/// </summary>
		public bool EnableGlobal { get; set; } = true;

		/// <summary>
		/// Enables backend-level dictionary caching (e.g., loaded files or datasets).
		/// </summary>
		public bool EnableBackend { get; set; } = true;

		/// <summary>
		/// Enables memory caching of missing keys to avoid repeated backend calls.
		/// </summary>
		public bool EnableMisses { get; set; } = true;

		/// <summary>
		/// Default expiration for global cache entries.
		/// </summary>
		public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(30);
	}
}
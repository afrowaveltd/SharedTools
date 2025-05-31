using Afrowave.SharedTools.Localization.Common.Models.Enums;
using System;

namespace Afrowave.SharedTools.Localization.Models
{
	/// <summary>
	/// Represents global/localization configuration for Afrowave localization engine.
	/// All properties have safe defaults to avoid nulls.
	/// </summary>
	public class LocalizationSettings
	{
		/// <summary>
		/// Default language code (e.g. "en").
		/// </summary>
		public string DefaultLanguage { get; set; } = "en";

		/// <summary>
		/// Fallback language code (if translation is missing).
		/// </summary>
		public string FallbackLanguage { get; set; } = "en";

		/// <summary>
		/// List of all available languages from registered backends.
		/// Used for UI selectors, language detection, or display.
		/// May be left empty to resolve dynamically from backends (if supported).
		/// </summary>
		public string[] SupportedLanguages { get; set; } = Array.Empty<string>();

		/// <summary>
		/// Order of locale detection: "query", "cookie", "header". Used by middleware, if any.
		/// </summary>
		public LocaleDetectionMethod[] LocaleDetectionOrder { get; set; } = Array.Empty<LocaleDetectionMethod>();

		/// <summary>
		/// If true, resolved fallback translations (e.g., from AI translator) will be saved to the first writable backend.
		/// </summary>
		public bool AutoTranslateAndSaveMissing { get; set; } = false;

		/// <summary>
		/// Enable debug mode for verbose logging (optional).
		/// </summary>
		public bool DebugMode { get; set; } = false;

		// ✅ NEW: Cache Settings
		/// <summary>
		/// Gets or sets the settings for localization caching.
		/// This includes options for enabling/disabling global and backend-level caching,
		/// This property contains configuration settings for caching strategies used in localization.
		/// </summary>

		public LocalizationCacheSettings Cache { get; set; } = new LocalizationCacheSettings();
	}
}
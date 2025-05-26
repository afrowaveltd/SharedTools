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
		/// All allowed languages for translation. If empty, all present in /Locales are allowed.
		/// </summary>
		public string[] SupportedLanguages { get; set; } = Array.Empty<string>();

		/// <summary>
		/// List of language codes to ignore (not offered for UI or autoload).
		/// </summary>
		public string[] IgnoreLanguages { get; set; } = Array.Empty<string>();

		/// <summary>
		/// Order of locale detection: "query", "cookie", "header". Used by middleware, if any.
		/// </summary>
		public string[] LocaleDetectionOrder { get; set; } = Array.Empty<string>();

		/// <summary>
		/// If true, missing translations will be requested automatically from translation backends.
		/// </summary>
		public bool AutoTranslateMissing { get; set; } = false;

		/// <summary>
		/// Path to the Locales folder (relative to app root). Default is "Locales".
		/// </summary>
		public string LocalesFolderPath { get; set; } = "Locales";

		/// <summary>
		/// Enable debug mode for verbose logging (optional).
		/// </summary>
		public bool DebugMode { get; set; } = false;
	}
}
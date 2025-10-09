using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Afrowave.SharedTools.I18N.Models
{
	/// <summary>
	/// Provides configuration options for JSON-based string localization, including language settings, translation
	/// behavior, and storage paths.
	/// </summary>
	/// <remarks>Use this class to customize how localized strings are loaded, translated, and managed when working
	/// with JSON resources. These options control aspects such as the default language, the location of locale files,
	/// integration with external translation services, and handling of missing localization keys.</remarks>
	public sealed class JsonStringLocalizerOptions
	{
		/// <summary>
		/// Gets or sets the file system path to the directory containing locale resources.
		/// </summary>
		public string LocalesPath { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets a value indicating whether LibreTranslate should be used as the translation provider.
		/// </summary>
		/// <remarks>When set to <see langword="true"/>, translation requests will be routed through LibreTranslate
		/// instead of the default provider. This option may affect available languages, translation quality, and service
		/// reliability depending on the selected provider.</remarks>
		public bool UseLibreTranslate { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether missing keys should be stored during processing.
		/// </summary>
		/// <remarks>When set to <see langword="true"/>, any keys that are not found will be added to the store. This
		/// can be useful for tracking or auditing purposes. When set to <see langword="false"/>, missing keys are ignored and
		/// not stored.</remarks>
		public bool StoreMissingKeys { get; set; } = false;

		/// <summary>
		/// Gets or sets the default language code used for content localization.
		/// </summary>
		public string DefaultLanguage { get; set; } = "en";
	}
}
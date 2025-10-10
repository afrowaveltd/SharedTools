using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Afrowave.SharedTools.I18N.Models
{
	/// <summary>
	/// Provides configuration options for localization behavior, including default language, translation providers,
	/// supported dictionary formats, and handling of missing translation keys.
	/// </summary>
	/// <remarks>Use this class to customize how localization is performed within an application. Options include
	/// specifying the default language, enabling machine translation, selecting translation providers, defining supported
	/// dictionary formats, and determining whether missing keys should be stored for later review or
	/// translation.</remarks>
	public sealed class LocalizerOptions
	{
		/// <summary>
		/// Gets or sets the default language code used for localization.
		/// </summary>
		/// <remarks>The default value is "en" (English). This property should be set to a valid ISO language code
		/// representing the primary language for the application.</remarks
		/// </summary>
		public string DefaultLanguage { get; set; } = "en";

		/// <summary>
		/// Gets or sets a value indicating whether machine translation should be used when translating content.
		/// </summary>
		public bool UseMachineTranslation { get; set; } = false;

		/// <summary>
		/// Gets or sets the collection of translation providers to be used for language translation operations.
		/// </summary>
		/// <remarks>The list must contain at least one provider. By default, the collection includes <see
		/// cref="TranslationProvider.None"/>, indicating that no translation provider is selected. To enable translation,
		/// replace or add appropriate providers to the collection.</remarks>
		public List<TranslationProvider> TranslationProviders { get; set; } = new List<TranslationProvider> { TranslationProvider.None };

		/// <summary>
		/// Gets or sets the collection of supported dictionary formats for localization.
		/// </summary>
		/// <remarks>The default format is <see cref="DictionaryFormat.JSON_FLAT"/>. Additional formats can be added to
		/// the collection as needed to support different serialization styles for language resources.</remarks>
		///
		public List<DictionaryFormat> SupportedFormats { get; set; } = new List<DictionaryFormat> { DictionaryFormat.JSON_FLAT };

		/// <summary>
		/// Gets or sets a value indicating whether missing keys should be stored when encountered.
		/// </summary>
		/// <remarks>When set to <see langword="true"/>, any keys that are not found during lookup operations will be
		/// added to the store. This can be useful for tracking or caching unknown keys, but may increase storage usage if
		/// many missing keys are encountered.
		/// It is worth to mention, that Storing missing keys might not be available for all DataStorages</remarks>
		public bool StoreMissingKeys { get; set; } = false;
	}
}
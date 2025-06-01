using Afrowave.SharedTools.Localization.Common.Options;

namespace Afrowave.SharedTools.Localization.Plugin.DataAccessor.JsonLocal.Options
{
	/// <summary>
	/// Configuration options for the JsonLocal DataAccessor plugin.
	/// </summary>
	public sealed class JsonLocalOptions : PluginOptionSet
	{
		/// <summary>
		/// Gets the unique key identifying the JSON-local configuration source.
		/// </summary>
		public override string Key => "json-local";

		/// <summary>
		/// Path to the folder where translation files are stored. Defaults to 'Locales' in project root.
		/// </summary>
		public string TranslationFolder { get; set; } = "Locales";

		/// <summary>
		/// Pattern used to locate translation files. Example: '{lang}.json'
		/// </summary>
		public string FilePattern { get; set; } = "{lang}.json";

		/// <summary>
		/// Enables auto-detection of the translation folder if not set explicitly.
		/// </summary>
		public bool EnableAutoDetectFolder { get; set; } = true;

		/// <summary>
		/// Maximum search depth for auto-detection (directories above current path).
		/// </summary>
		public int AutoDetectSearchDepth { get; set; } = 3;
	}
}
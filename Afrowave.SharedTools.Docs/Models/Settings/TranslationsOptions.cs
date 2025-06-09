namespace Afrowave.SharedTools.Docs.Models.Settings
{
	/// <summary>
	/// Represents configuration options for managing translations in various formats.
	/// </summary>
	/// <remarks>This class provides settings to control default language behavior, ignored languages for specific
	/// formats, and the interval between translation processing cycles.</remarks>
	public class TranslationsOptions
	{
		/// <summary>
		/// Gets or sets the default language code used by the application.
		/// </summary>
		public string DefaultLanguage { get; set; } = "en";

		/// <summary>
		/// Gets or sets the list of language codes that should be ignored during JSON processing.
		/// </summary>
		public string[] IgnoredForJson { get; set; } = ["en", "cs"];

		/// <summary>
		/// Gets or sets the list of language codes that should be ignored for markdown processing.
		/// </summary>
		public string[] IgnoredForMd { get; set; } = ["en"];

		/// <summary>
		/// Gets or sets the number of minutes between each cycle.
		/// </summary>
		public int MinutesBetweenCycles { get; set; } = 20;
	}
}
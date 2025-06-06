namespace Afrowave.SharedTools.Docs.Models
{
	/// <summary>
	/// Represents the result of a translation operation performed by the LibreTranslation service.
	/// </summary>
	/// <remarks>This class encapsulates the translated text returned by the service.  It is typically used to store
	/// and access the output of a translation request.</remarks>
	public class LibreTranslationResult
	{
		/// <summary>
		/// Gets or sets the translated text resulting from a translation operation.
		/// </summary>
		public string TranslatedText { get; set; } = string.Empty;
	}
}
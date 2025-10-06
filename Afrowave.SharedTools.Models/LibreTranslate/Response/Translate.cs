using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.Models.LibreTranslate.Response
{
	/// <summary>
	/// Represents the result of a translation operation performed by the LibreTranslation service.
	/// </summary>
	/// <remarks>
	/// This class encapsulates the translated text returned by the service. It is typically used to
	/// store and access the output of a translation request.
	/// </remarks>
	public class Translate
	{
		/// <summary>
		/// Gets or sets the list of alternative translation.
		/// </summary>
		[JsonPropertyName("alternatives")]
		public List<string> Alternatives { get; set; } = new List<string>();

		/// <summary>
		/// Gets or sets the detected language of the analyzed text.
		/// </summary>
		[JsonPropertyName("detectedLanguage")]
		public Detections DetectedLanguage { get; set; } = new Detections();

		/// <summary>
		/// Gets or sets the translated text resulting from a translation operation.
		/// </summary>
		[JsonPropertyName("translatedText")]
		public string TranslatedText { get; set; } = string.Empty;
	}
}
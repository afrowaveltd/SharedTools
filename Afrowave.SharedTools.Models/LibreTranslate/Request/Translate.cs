using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.Models.LibreTranslate.Request
{
	/// <summary>
	/// Represents a request to the LibreTranslate API for translating text between languages.
	/// </summary>
	/// <remarks>
	/// This class encapsulates the parameters required to perform a translation using the
	/// LibreTranslate API. It includes the text to be translated, source and target languages,
	/// formatting options, and additional settings.
	/// </remarks>
	public class Translate
	{
		/// <summary>
		/// Gets or sets the number of alternative options available.
		/// </summary>
		[JsonPropertyName("alternatives")]
		public int Alternatives { get; set; } = 0;

		/// <summary>
		/// Gets or sets the API key used for authenticating requests to the service.
		/// </summary>

		[JsonPropertyName("api_key")]
		public string? Api_key { get; set; } = null;

		/// <summary>
		/// Gets or sets the format of the output.
		/// </summary>
		[JsonPropertyName("format")]
		public string? Format { get; set; } = "text";

		/// <summary>
		/// Gets or sets the value of Q.
		/// </summary>
		[JsonPropertyName("q")]
		public string Q { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the source language code for the translation.
		/// </summary>
		[JsonPropertyName("source")]
		public string Source { get; set; } = "auto";

		/// <summary>
		/// Gets or sets the target language code for translation.
		/// </summary>
		[JsonPropertyName("target")]
		public string Target { get; set; } = "en";
	}
}
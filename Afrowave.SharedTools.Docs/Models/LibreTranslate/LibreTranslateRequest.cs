﻿namespace Afrowave.SharedTools.Docs.Models.LibreTranslate
{
	/// <summary>
	/// Represents a request to the LibreTranslate API for translating text between languages.
	/// </summary>
	/// <remarks>This class encapsulates the parameters required to perform a translation using the LibreTranslate
	/// API. It includes the text to be translated, source and target languages, formatting options, and additional
	/// settings.</remarks>
	public class LibreTranslateRequest
	{
		/// <summary>
		/// Gets or sets the value of Q.
		/// </summary>
		public string Q { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the source language code for the translation.
		/// </summary>
		public string Source { get; set; } = "auto";

		/// <summary>
		/// Gets or sets the target language code for translation.
		/// </summary>
		public string Target { get; set; } = "en";

		/// <summary>
		/// Gets or sets the format of the output.
		/// </summary>
		public string? Format { get; set; } = "text";

		/// <summary>
		/// Gets or sets the number of alternative options available.
		/// </summary>
		public int Alternatives { get; set; } = 0;

		/// <summary>
		/// Gets or sets the API key used for authenticating requests to the service.
		/// </summary>
		public string? Api_key { get; set; } = null;
	}
}
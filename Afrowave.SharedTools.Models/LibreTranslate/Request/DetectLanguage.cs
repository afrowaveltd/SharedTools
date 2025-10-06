using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.Models.LibreTranslate.Request
{
	public class DetectLanguage
	{
		/// <summary>
		/// Gets or sets the API key for authentication purposes.
		/// </summary>
		[JsonPropertyName("api_key")]
		public string Api_key { get; set; }

		/// <summary>
		/// Gets or sets the value of the query string.
		/// </summary>
		[JsonPropertyName("q")]
		public string Q { get; set; } = string.Empty;
	}
}
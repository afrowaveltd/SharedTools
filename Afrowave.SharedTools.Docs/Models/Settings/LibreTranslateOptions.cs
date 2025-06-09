namespace Afrowave.SharedTools.Docs.Models.Settings
{
	/// <summary>
	/// Represents configuration options for connecting to a LibreTranslate service.
	/// </summary>
	/// <remarks>This class provides settings for specifying the host, API key, and whether an API key is required
	/// when interacting with a LibreTranslate instance. These options are typically used to configure a client for making
	/// translation requests.</remarks>
	public class LibreTranslateOptions
	{
		/// <summary>
		/// Gets or sets the host name or IP address of the server.
		/// </summary>
		public string Host { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the API key used for authenticating requests to the service.
		/// </summary>
		public string ApiKey { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets a value indicating whether a key is required for the operation.
		/// </summary>
		public bool NeedsKey { get; set; } = false;

		/// <summary>
		/// Gets or sets the endpoint URL used to retrieve supported languages.
		/// </summary>
		public string LanguagesEndpoint { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the endpoint URL for the translation service.
		/// </summary>
		public string TranslateEndpoint { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the endpoint URL used for file translation requests.
		/// </summary>
		public string TranslateFileEndpoint { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the two characters ISO code of the language detected from providen text.
		/// </summary>
		public string DetectLanguageEndpoint { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the number of retry attempts to perform when an operation fails.
		/// </summary>
		/// <remarks>This property determines how many times an operation will be retried in the event of a failure.
		/// Set this value to 0 to disable retries.</remarks>
		public int RetriesOnFailure { get; set; } = 10;

		/// <summary>
		/// Gets or sets the number of seconds to wait before retrying a failed operation.
		/// </summary>
		public int WaitSecondBeforeRetry { get; set; } = 2;
	}
}
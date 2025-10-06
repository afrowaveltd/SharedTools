﻿namespace Afrowave.SharedTools.Models.LibreTranslate.Settings
{
	public class LibreServer
	{
		/// <summary>
		/// Gets or sets the API key used for authenticating requests to the service.
		/// </summary>
		public string ApiKey { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the endpoint URL used to detect the language from provided text.
		/// </summary>
		public string DetectLanguageEndpoint { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the host name or IP address of the server.
		/// </summary>
		public string Host { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the endpoint URL used to retrieve supported languages.
		/// </summary>
		public string LanguagesEndpoint { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets a value indicating whether a key is required for the operation.
		/// </summary>
		public bool NeedsKey { get; set; } = false;

		/// <summary>
		/// Gets or sets the endpoint URL for the translation service.
		/// </summary>
		public string TranslateEndpoint { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the endpoint URL used for file translation requests.
		/// </summary>
		public string TranslateFileEndpoint { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the number of threads to use for parallel processing.
		/// </summary>
		/// <remarks>Increasing the number of threads may improve performance for workloads that can be parallelized,
		/// but may also increase resource usage. The optimal value depends on the nature of the task and the available system
		/// resources.</remarks>
		public int Threads { get; set; } = 1;
	}
}
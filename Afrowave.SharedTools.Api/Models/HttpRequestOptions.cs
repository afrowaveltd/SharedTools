using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

namespace Afrowave.SharedTools.Api.Models
{
	/// <summary>
	/// Represents options for configuring HTTP requests, including base address, timeout, headers, proxy, SSL, decompression, and retry policy.
	/// </summary>
	public sealed class HttpRequestOptions
	{
		/// <summary>
		/// Gets or sets the base address for HTTP requests.
		/// </summary>
		public Uri BaseAddress { get; set; } = new Uri("http://localhost");

		/// <summary>
		/// Gets or sets the timeout duration for HTTP requests.
		/// </summary>
		public TimeSpan? Timeout { get; set; }

		/// <summary>
		/// Gets or sets the default headers to include in every HTTP request.
		/// </summary>
		public IDictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();

		/// <summary>
		/// Gets or sets a value indicating whether to use a proxy for HTTP requests.
		/// </summary>
		public bool UseProxy { get; set; } = false;

		/// <summary>
		/// Gets or sets the proxy address to use if <see cref="UseProxy"/> is true.
		/// </summary>
		public string ProxyAddress { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the credentials to use for the proxy.
		/// </summary>
		public ICredentials ProxyCredentials { get; set; } = CredentialCache.DefaultCredentials;

		/// <summary>
		/// Gets or sets a value indicating whether to allow invalid SSL certificates (for development only).
		/// </summary>
		public bool AllowInvalidCertificates { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether to enable Gzip and Deflate decompression for HTTP responses.
		/// </summary>
		public bool DecompressionGzipDeflate { get; set; } = true;

		/// <summary>
		/// Gets or sets the retry policy options for HTTP requests.
		/// </summary>
		public RetryPolicyOptions Retry { get; set; } = new RetryPolicyOptions();

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpRequestOptions"/> class with default values.
		/// </summary>
		public HttpRequestOptions()
		{
			DefaultHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				{ "User-Agent", "Afrowave.SharedTools.ApiClient/1.0" }
			};
			Retry = new RetryPolicyOptions();
			DecompressionGzipDeflate = true;
		}
	}
}
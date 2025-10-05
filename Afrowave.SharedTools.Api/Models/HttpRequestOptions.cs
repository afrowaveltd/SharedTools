using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

namespace Afrowave.SharedTools.Api.Models
{
	public sealed class HttpRequestOptions
	{
		public Uri BaseAddress { get; set; } = new Uri("http://localhost");
		public TimeSpan? Timeout { get; set; }
		public IDictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();

		// Proxy settings
		public bool UseProxy { get; set; } = false;

		public string ProxyAddress { get; set; } = string.Empty;
		public ICredentials ProxyCredentials { get; set; } = CredentialCache.DefaultCredentials;

		// SSL (DEV Only)
		public bool AllowInvalidCertificates { get; set; } = false;

		// Decompression
		public bool DecompressionGzipDeflate { get; set; } = true;

		// Retry
		public RetryPolicyOptions Retry { get; set; } = new RetryPolicyOptions();

		public HttpRequestOptions()
		{
			DefaultHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			DefaultHeaders.Add("User-Agent", "Afrowave.SharedTools.ApiClient/1.0");
			Retry = new RetryPolicyOptions();
			DecompressionGzipDeflate = true;
		}
	}
}
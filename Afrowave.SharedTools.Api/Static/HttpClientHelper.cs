using Afrowave.SharedTools.Api.Models;
using Afrowave.SharedTools.Api.Serialization;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Api.Static
{
	/// <summary>
	/// Provides static helper methods for making HTTP requests with configurable options, retry logic, and response handling.
	/// </summary>
	public static class HttpClientHelper
	{
		private static readonly Random _rnd = new Random();

		// ----------- VEŘEJNÉ METODY -----------

		/// <summary>
		/// Sends a GET request and deserializes the JSON response to the specified type.
		/// </summary>
		public static Task<Response<T>> GetJsonAsync<T>(string url, HttpRequestOptions? options = null, IDictionary<string, string>? headers = null, CancellationToken ct = default)
		{
			if(string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
			options ??= new HttpRequestOptions();
			return SendWithRetry<T>(options, async client =>
			{
				using var req = new HttpRequestMessage(HttpMethod.Get, url);
				ApplyHeaders(req, headers);
				using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
				if(!res.IsSuccessStatusCode)
					return Response<T>.Fail("HTTP " + (int)res.StatusCode + " " + res.ReasonPhrase);

				var stream = await res.Content.ReadAsStreamAsync().ConfigureAwait(false);
				var data = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions.Default, ct).ConfigureAwait(false);
				return Response<T>.SuccessResponse(data, "");
			}, ct);
		}

		/// <summary>
		/// Sends a POST request with a JSON body and deserializes the JSON response to the specified type.
		/// </summary>
		public static Task<Response<TRes>> PostJsonAsync<TReq, TRes>(string url, TReq body, HttpRequestOptions? options = null, IDictionary<string, string>? headers = null, CancellationToken ct = default)
		{
			if(string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
			options ??= new HttpRequestOptions();

			return SendWithRetry<TRes>(options, async client =>
			{
				var json = JsonSerializer.Serialize(body, JsonOptions.Default);
				using var req = new HttpRequestMessage(HttpMethod.Post, url);
				req.Content = new StringContent(json, Encoding.UTF8, "application/json");
				ApplyHeaders(req, headers);
				using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
				if(!res.IsSuccessStatusCode)
					return Response<TRes>.Fail("HTTP " + (int)res.StatusCode + " " + res.ReasonPhrase);

				var stream = await res.Content.ReadAsStreamAsync().ConfigureAwait(false);
				var data = await JsonSerializer.DeserializeAsync<TRes>(stream, JsonOptions.Default, ct).ConfigureAwait(false);
				return Response<TRes>.SuccessResponse(data, string.Empty);
			}, ct);
		}

		/// <summary>
		/// Sends a GET request and returns the response as a string.
		/// </summary>
		public static Task<Response<string>> GetStringAsync(string url, HttpRequestOptions? options = null, IDictionary<string, string>? headers = null, CancellationToken ct = default)
		{
			if(string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
			options ??= new HttpRequestOptions();

			return SendWithRetry<string>(options, async client =>
				{
					using var req = new HttpRequestMessage(HttpMethod.Get, url);
					ApplyHeaders(req, headers);
					using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
					if(!res.IsSuccessStatusCode)
						return Response<string>.Fail("HTTP " + (int)res.StatusCode + " " + res.ReasonPhrase);
					var text = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
					return Response<string>.SuccessResponse(text, string.Empty);
				}, ct);
		}

		/// <summary>
		/// Sends a POST request with form URL-encoded data and returns the response as a string.
		/// </summary>
		public static Task<Response<string>> PostFormUrlEncodedAsync(
			  string url,
			  HttpRequestOptions? options = null,
			  IEnumerable<KeyValuePair<string, string>>? form = null,
			  IDictionary<string, string>? headers = null,
			  CancellationToken ct = default)
		{
			options ??= new HttpRequestOptions();

			return SendWithRetry<string>(options, async client =>
			{
				using var req = new HttpRequestMessage(HttpMethod.Post, url);
				req.Content = new FormUrlEncodedContent(form ?? new List<KeyValuePair<string, string>>());
				ApplyHeaders(req, headers);

				using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
				if(!res.IsSuccessStatusCode)
					return Response<string>.Fail("HTTP " + (int)res.StatusCode + " " + res.ReasonPhrase);

				var htmlOrText = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
				return Response<string>.SuccessResponse(htmlOrText, string.Empty);
			}, ct);
		}

		/// <summary>
		/// Sends a POST request with multipart/form-data and deserializes the JSON response to the specified type.
		/// </summary>
		public static Task<Response<TRes>> PostMultipartAsync<TRes>(
			 string url,
			 HttpRequestOptions? options = null,
			 IEnumerable<KeyValuePair<string, string>>? fields = null,
			 IEnumerable<FilePart>? files = null,
			 IDictionary<string, string>? headers = null,
			 CancellationToken ct = default)
		{
			options ??= new HttpRequestOptions();

			return SendWithRetry<TRes>(options, async client =>
			{
				using var content = new MultipartFormDataContent();
				if(fields != null)
				{
					foreach(var kv in fields)
						content.Add(new StringContent(kv.Value ?? string.Empty, Encoding.UTF8), kv.Key);
				}

				if(files != null)
				{
					foreach(var f in files)
					{
						var bytes = f.Bytes ?? new byte[0];
						var fileContent = new ByteArrayContent(bytes);
						if(!string.IsNullOrEmpty(f.ContentType))
							fileContent.Headers.ContentType = new MediaTypeHeaderValue(f.ContentType);

						content.Add(fileContent, f.Name ?? "file", f.FileName ?? "upload.bin");
					}
				}

				using var req = new HttpRequestMessage(HttpMethod.Post, url);
				req.Content = content;
				ApplyHeaders(req, headers);

				using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
				if(!res.IsSuccessStatusCode)
					return Response<TRes>.Fail("HTTP " + (int)res.StatusCode + " " + res.ReasonPhrase);

				var stream = await res.Content.ReadAsStreamAsync().ConfigureAwait(false);
				var data = await JsonSerializer.DeserializeAsync<TRes>(stream, JsonOptions.Default, ct).ConfigureAwait(false);
				return Response<TRes>.SuccessResponse(data, string.Empty);
			}, ct);
		}

		// ----------- SOUKROMÉ METODY -----------

		/// <summary>
		/// Creates a configured <see cref="HttpClient"/> instance based on the specified <see cref="HttpRequestOptions"/>.
		/// </summary>
		private static HttpClient CreateClient(HttpRequestOptions options)
		{
			var handler = new HttpClientHandler();
			if(options.DecompressionGzipDeflate)
				handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			if(options.AllowInvalidCertificates)
				handler.ServerCertificateCustomValidationCallback = (m, c, ch, e) => true;
			if(options.UseProxy && !string.IsNullOrEmpty(options.ProxyAddress))
			{
				handler.Proxy = new WebProxy(options.ProxyAddress, false)
				{
					Credentials = options.ProxyCredentials
				};
				handler.UseProxy = true;
			}

			var client = new HttpClient(handler);
			if(options.BaseAddress != null) client.BaseAddress = options.BaseAddress;
			if(options.Timeout.HasValue) client.Timeout = options.Timeout.Value;
			foreach(var kv in options.DefaultHeaders)
				client.DefaultRequestHeaders.TryAddWithoutValidation(kv.Key, kv.Value);
			return client;
		}

		/// <summary>
		/// Applies the specified headers to the <see cref="HttpRequestMessage"/>.
		/// </summary>
		private static void ApplyHeaders(HttpRequestMessage req, IDictionary<string, string> headers)
		{
			if(headers == null) return;
			foreach(var kv in headers)
				req.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
		}

		/// <summary>
		/// Executes the specified HTTP action with retry logic based on the provided options.
		/// </summary>
		private static async Task<Response<T>> SendWithRetry<T>(HttpRequestOptions options, Func<HttpClient, Task<Response<T>>> action, CancellationToken ct)
		{
			int retries = options.Retry.MaxRetries;
			TimeSpan delay = options.Retry.BaseDelay;

			for(int attempt = 0; attempt <= retries; attempt++)
			{
				var client = CreateClient(options);
				try
				{
					var result = await action(client).ConfigureAwait(false);
					if(result != null && result.Success) return result;
					if(attempt == retries) return result ?? Response<T>.Fail("Unknown error");
				}
				catch(Exception ex)
				{
					if(attempt == retries) return Response<T>.Fail(ex);
				}

				await Task.Delay(WithJitter(delay, options.Retry.Jitter), ct).ConfigureAwait(false);
				delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * options.Retry.BackoffFactor);
			}

			return Response<T>.Fail("Unknown error");
		}

		/// <summary>
		/// Applies random jitter to the base delay if enabled.
		/// </summary>
		private static TimeSpan WithJitter(TimeSpan baseDelay, bool jitter)
		{
			if(!jitter) return baseDelay;
			double factor = _rnd.NextDouble() * 0.4 + 0.8;
			return TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * factor);
		}
	}
}
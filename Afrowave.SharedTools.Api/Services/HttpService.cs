using Afrowave.SharedTools.Api.Models;
using Afrowave.SharedTools.Api.Serialization;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Afrowave.SharedTools.Api.Services
{
	/// <summary>
	/// Provides HTTP client functionality with support for JSON, XML, string, and byte responses, including retry and custom options.
	/// </summary>
	public sealed class HttpService : IHttpService
	{
		private readonly IHttpClientFactory _factory;
		private readonly HttpRequestOptions _options;
		private readonly Random _rnd;

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpService"/> class.
		/// </summary>
		/// <param name="factory">The HTTP client factory to use for creating clients.</param>
		/// <param name="options">Optional HTTP request options. If null, defaults are used.</param>
		public HttpService(IHttpClientFactory factory, HttpRequestOptions? options = null)
		{
			_factory = factory ?? throw new ArgumentNullException(nameof(factory));
			_options = options ?? new HttpRequestOptions();
			_rnd = new Random();
		}

		/// <summary>
		/// Creates a raw <see cref="HttpClient"/> instance with applied options.
		/// </summary>
		/// <returns>A configured <see cref="HttpClient"/>.</returns>
		public HttpClient CreateRawClient()
		{
			var client = _factory.CreateClient("AfrowaveHttpService");
			ApplyOptions(client, _options);
			return client;
		}

		/// <summary>
		/// Sends a GET request and deserializes the JSON response to the specified type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the response to.</typeparam>
		/// <param name="url">The request URL.</param>
		/// <param name="headers">Optional headers to include in the request.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A <see cref="Response{T}"/> containing the deserialized data or error information.</returns>
		public Task<Response<T>> GetJsonAsync<T>(string url, IDictionary<string, string>? headers, CancellationToken ct = default)
		{
			return SendWithRetry<T>(async client =>
			{
				using var req = new HttpRequestMessage(HttpMethod.Get, url);
				ApplyHeaders(req, headers);
				using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
				if(!res.IsSuccessStatusCode)
				{
					return Response<T>.Fail("HTTP " + (int)res.StatusCode + " " + res.ReasonPhrase);
				}

				var stream = await res.Content.ReadAsStreamAsync().ConfigureAwait(false);
				var data = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions.Default, ct).ConfigureAwait(false);
#pragma warning disable CS8604 // Může jít o argument s odkazem null.
				return Response<T>.SuccessResponse(data, "OK");
#pragma warning restore CS8604 // Může jít o argument s odkazem null.
			}, ct);
		}

		/// <summary>
		/// Sends a POST request with a JSON body and deserializes the JSON response to the specified type.
		/// </summary>
		/// <typeparam name="TReq">The type of the request body.</typeparam>
		/// <typeparam name="TRes">The type to deserialize the response to.</typeparam>
		/// <param name="url">The request URL.</param>
		/// <param name="body">The request body.</param>
		/// <param name="headers">Optional headers to include in the request.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A <see cref="Response{TRes}"/> containing the deserialized data or error information.</returns>
		public Task<Response<TRes>> PostJsonAsync<TReq, TRes>(string url, TReq body, IDictionary<string, string>? headers = null, CancellationToken ct = default)
		{
			return SendWithRetry<TRes>(async client =>
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
#pragma warning disable CS8604 // Může jít o argument s odkazem null.
				return Response<TRes>.SuccessResponse(data, "");
#pragma warning restore CS8604 // Může jít o argument s odkazem null.
			}, ct);
		}

		/// <summary>
		/// Sends a GET request and returns the response as a string.
		/// </summary>
		/// <param name="url">The request URL.</param>
		/// <param name="headers">Optional headers to include in the request.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A <see cref="Response{string}"/> containing the response text or error information.</returns>
		public Task<Response<string>> GetStringAsync(string url, IDictionary<string, string>? headers = null, CancellationToken ct = default)
		{
			return SendWithRetry<string>(async client =>
			{
				using var req = new HttpRequestMessage(HttpMethod.Get, url);
				ApplyHeaders(req, headers);
				using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
				if(!res.IsSuccessStatusCode)
					return Response<string>.Fail("HTTP " + (int)res.StatusCode + " " + res.ReasonPhrase);

				var text = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
				return Response<string>.SuccessResponse(text, "Ok");
			}, ct);
		}

		/// <summary>
		/// Sends a GET request and returns the response as a byte array.
		/// </summary>
		/// <param name="url">The request URL.</param>
		/// <param name="headers">Optional headers to include in the request.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A <see cref="Response{List}"/> containing the response bytes or error information.</returns>
		public Task<Response<byte[]>> GetBytesAsync(string url, IDictionary<string, string>? headers = null, CancellationToken ct = default)
		{
			return SendWithRetry<byte[]>(async client =>
			{
				using var req = new HttpRequestMessage(HttpMethod.Get, url);
				ApplyHeaders(req, headers);
				using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
				if(!res.IsSuccessStatusCode)
					return Response<byte[]>.Fail("HTTP " + (int)res.StatusCode + " " + res.ReasonPhrase);

				var data = await res.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
				return Response<byte[]>.SuccessResponse(data, "");
			}, ct);
		}

		/// <summary>
		/// Sends a GET request and deserializes the XML response to the specified type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the response to.</typeparam>
		/// <param name="url">The request URL.</param>
		/// <param name="headers">Optional headers to include in the request.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A <see cref="Response{T}"/> containing the deserialized data or error information.</returns>
		public Task<Response<T>> GetXmlAsync<T>(string url, IDictionary<string, string>? headers = null, CancellationToken ct = default)
		{
			return SendWithRetry<T>(async client =>
			{
				using var req = new HttpRequestMessage(HttpMethod.Get, url);
				ApplyHeaders(req, headers);
				using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
				if(!res.IsSuccessStatusCode)
					return Response<T>.Fail("HTTP " + (int)res.StatusCode + " " + res.ReasonPhrase);

				var stream = await res.Content.ReadAsStreamAsync().ConfigureAwait(false);
				var serializer = new XmlSerializer(typeof(T));
				var obj = (T)serializer.Deserialize(stream);
				return Response<T>.SuccessResponse(obj, "OK");
			}, ct);
		}

		/// <summary>
		/// Sends a POST request with form URL-encoded data and returns the response as a string.
		/// </summary>
		/// <param name="url">The request URL.</param>
		/// <param name="form">The form data to send.</param>
		/// <param name="headers">Optional headers to include in the request.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A <see cref="Response{string}"/> containing the response text or error information.</returns>
		public Task<Response<string>> PostFormUrlEncodedAsync(string url, IEnumerable<KeyValuePair<string, string>> form, IDictionary<string, string>? headers = null, CancellationToken ct = default)
		{
			return SendWithRetry<string>(async client =>
			{
				using var req = new HttpRequestMessage(HttpMethod.Post, url);
				req.Content = new FormUrlEncodedContent(form);
				ApplyHeaders(req, headers);
				using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
				if(!res.IsSuccessStatusCode)
					return Response<string>.Fail("HTTP " + (int)res.StatusCode + " " + res.ReasonPhrase);

				var html = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
				return Response<string>.SuccessResponse(html, "");
			}, ct);
		}

		/// <summary>
		/// Sends multiple GET requests in parallel and deserializes the JSON responses to the specified type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the responses to.</typeparam>
		/// <param name="urls">The collection of request URLs.</param>
		/// <param name="maxParallel">The maximum number of parallel requests.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A read-only list of <see cref="Response{T}"/> objects containing the results.</returns>
		public async Task<IReadOnlyList<Response<T>>> GetJsonManyAsync<T>(IEnumerable<string> urls, int maxParallel = 2, CancellationToken ct = default)
		{
			if(maxParallel < 1) maxParallel = 1;
			var sem = new System.Threading.SemaphoreSlim(maxParallel);
			var tasks = new List<Task<Response<T>>>();

			foreach(var url in urls)
			{
				await sem.WaitAsync(ct).ConfigureAwait(false);
				tasks.Add(Task.Run(async () =>
				{
					try { return await GetJsonAsync<T>(url, null, ct).ConfigureAwait(false); }
					finally { sem.Release(); }
				}, ct));
			}

			return await Task.WhenAll(tasks).ConfigureAwait(false);
		}

		/// <summary>
		/// Sends an HTTP POST request with multipart/form-data content to the specified URL, including form fields and file
		/// attachments, and asynchronously deserializes the response to the specified type.
		/// </summary>
		/// <remarks>If the API response is not in JSON format, use TRes as string to receive the raw response text.
		/// The method automatically retries the request on transient failures according to the configured retry policy. File
		/// parts must provide non-null byte arrays; empty arrays are sent if file data is missing.</remarks>
		/// <typeparam name="TRes">The type to which the response content will be deserialized. Typically a model representing the expected API
		/// response.</typeparam>
		/// <param name="url">The endpoint URL to which the multipart POST request is sent. Must be a valid absolute or relative URI.</param>
		/// <param name="fields">A collection of key-value pairs representing form fields to include in the multipart request. Keys are used as
		/// field names; values are sent as UTF-8 encoded strings.</param>
		/// <param name="files">A collection of file parts to attach to the request. Each file part specifies the field name, file name, content
		/// type, and file data.</param>
		/// <param name="headers">An optional dictionary of HTTP headers to include in the request. If null, no additional headers are added.</param>
		/// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a Response TRes object with the
		/// deserialized response data if the request succeeds, or error information if the request fails.</returns>
		public Task<Response<TRes>> PostMultipartAsync<TRes>(
			 string url,
			 IEnumerable<KeyValuePair<string, string>> fields,
			 IEnumerable<FilePart> files,
			 IDictionary<string, string>? headers = null,
			 CancellationToken ct = default)
		{
			return SendWithRetry<TRes>(async client =>
			{
				using var content = new MultipartFormDataContent();
				// fields
				if(fields != null)
				{
					foreach(var kv in fields)
					{
						content.Add(new StringContent(kv.Value ?? string.Empty, Encoding.UTF8), kv.Key);
					}
				}

				// files (byte[] varianta)
				if(files != null)
				{
					foreach(var f in files)
					{
						var bytes = f.Bytes ?? new byte[0];
						var fileContent = new ByteArrayContent(bytes);
						if(!string.IsNullOrEmpty(f.ContentType))
							fileContent.Headers.ContentType = new MediaTypeHeaderValue(f.ContentType);

						// name: field name; fileName: přiložený název souboru
						content.Add(fileContent, f.Name ?? "file", f.FileName ?? "upload.bin");
					}
				}

				using var req = new HttpRequestMessage(HttpMethod.Post, url);
				req.Content = content;
				ApplyHeaders(req, headers);

				using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
				if(!res.IsSuccessStatusCode)
					return Response<TRes>.Fail("HTTP " + (int)res.StatusCode + " " + res.ReasonPhrase);

				// Pokusit se o JSON deserializaci (typická odpověď API)
				// Pokud API vrací čistý text, použij metodu s TRes = string.
				var stream = await res.Content.ReadAsStreamAsync().ConfigureAwait(false);
				var data = await JsonSerializer.DeserializeAsync<TRes>(stream, JsonOptions.Default, ct).ConfigureAwait(false);
				return Response<TRes>.SuccessResponse(data, string.Empty);
			}, ct);
		}

		// ---------- interní pomocné ----------

		private Task<Response<T>> SendWithRetry<T>(Func<HttpClient, Task<Response<T>>> action, CancellationToken ct)
		{
			return ExecuteWithRetry(action, ct);
		}

		private async Task<Response<T>> ExecuteWithRetry<T>(Func<HttpClient, Task<Response<T>>> action, CancellationToken ct)
		{
			int retries = _options.Retry.MaxRetries;
			TimeSpan delay = _options.Retry.BaseDelay;

			for(int attempt = 0; attempt <= retries; attempt++)
			{
				var client = CreateRawClient();
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

				await Task.Delay(WithJitter(delay, _options.Retry.Jitter), ct).ConfigureAwait(false);
				delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * _options.Retry.BackoffFactor);
			}

			return Response<T>.Fail("Unknown error");
		}

		private TimeSpan WithJitter(TimeSpan baseDelay, bool jitter)
		{
			if(!jitter) return baseDelay;
			double factor = _rnd.NextDouble() * 0.4 + 0.8; // 0.8–1.2×
			return TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * factor);
		}

		private static void ApplyHeaders(HttpRequestMessage req, IDictionary<string, string>? headers)
		{
			if(headers == null) return;
			foreach(var kv in headers)
				req.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
		}

		private static void ApplyOptions(HttpClient client, HttpRequestOptions opt)
		{
			if(opt.BaseAddress != null) client.BaseAddress = opt.BaseAddress;
			if(opt.Timeout.HasValue) client.Timeout = opt.Timeout.Value;
			foreach(var h in opt.DefaultHeaders)
				client.DefaultRequestHeaders.TryAddWithoutValidation(h.Key, h.Value);
		}
	}
}
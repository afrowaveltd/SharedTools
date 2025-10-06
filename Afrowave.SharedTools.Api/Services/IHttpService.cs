using Afrowave.SharedTools.Api.Models;
using Afrowave.SharedTools.Models.Results;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Api.Services
{
	/// <summary>
	/// Defines a contract for HTTP service operations, including GET, POST, and response handling for various formats.
	/// </summary>
	public interface IHttpService
	{
		/// <summary>
		/// Creates a raw <see cref="HttpClient"/> instance with applied options.
		/// </summary>
		/// <returns>A configured <see cref="HttpClient"/>.</returns>
		HttpClient CreateRawClient();

		/// <summary>
		/// Sends a GET request and returns the response as a byte array.
		/// </summary>
		/// <param name="url">The request URL.</param>
		/// <param name="headers">Optional headers to include in the request.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A <see cref="Response{byte[]}"/> containing the response bytes or error information.</returns>
		Task<Response<byte[]>> GetBytesAsync(string url, IDictionary<string, string> headers = null, CancellationToken ct = default);

		/// <summary>
		/// Sends a GET request and deserializes the JSON response to the specified type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the response to.</typeparam>
		/// <param name="url">The request URL.</param>
		/// <param name="headers">Optional headers to include in the request.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A <see cref="Response{T}"/> containing the deserialized data or error information.</returns>
		Task<Response<T>> GetJsonAsync<T>(string url, IDictionary<string, string> headers, CancellationToken ct = default);

		/// <summary>
		/// Sends multiple GET requests in parallel and deserializes the JSON responses to the specified type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the responses to.</typeparam>
		/// <param name="urls">The collection of request URLs.</param>
		/// <param name="maxParallel">The maximum number of parallel requests.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A read-only list of <see cref="Response{T}"/> objects containing the results.</returns>
		Task<IReadOnlyList<Response<T>>> GetJsonManyAsync<T>(IEnumerable<string> urls, int maxParallel = 2, CancellationToken ct = default);

		/// <summary>
		/// Sends a GET request and returns the response as a string.
		/// </summary>
		/// <param name="url">The request URL.</param>
		/// <param name="headers">Optional headers to include in the request.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A <see cref="Response{string}"/> containing the response text or error information.</returns>
		Task<Response<string>> GetStringAsync(string url, IDictionary<string, string> headers = null, CancellationToken ct = default);

		/// <summary>
		/// Sends a GET request and deserializes the XML response to the specified type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the response to.</typeparam>
		/// <param name="url">The request URL.</param>
		/// <param name="headers">Optional headers to include in the request.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A <see cref="Response{T}"/> containing the deserialized data or error information.</returns>
		Task<Response<T>> GetXmlAsync<T>(string url, IDictionary<string, string> headers = null, CancellationToken ct = default);

		/// <summary>
		/// Sends a POST request with form URL-encoded data and returns the response as a string.
		/// </summary>
		/// <param name="url">The request URL.</param>
		/// <param name="form">The form data to send.</param>
		/// <param name="headers">Optional headers to include in the request.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A <see cref="Response{string}"/> containing the response text or error information.</returns>
		Task<Response<string>> PostFormUrlEncodedAsync(string url, IEnumerable<KeyValuePair<string, string>> form, IDictionary<string, string> headers = null, CancellationToken ct = default);

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
		Task<Response<TRes>> PostJsonAsync<TReq, TRes>(string url, TReq body, IDictionary<string, string> headers = null, CancellationToken ct = default);

		/// <summary>POST multipart/form-data (form fields + soubory).</summary>
		Task<Response<TRes>> PostMultipartAsync<TRes>(
			 string url,
			 IEnumerable<KeyValuePair<string, string>> fields,
			 IEnumerable<FilePart> files,
			 IDictionary<string, string> headers = null,
			 CancellationToken ct = default(CancellationToken));
	}
}
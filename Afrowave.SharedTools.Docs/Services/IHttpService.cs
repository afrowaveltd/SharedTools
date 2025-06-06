using System.Text.Json;

namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Defines a contract for making HTTP requests and handling HTTP responses.
	/// </summary>
	/// <remarks>This interface provides methods for performing common HTTP operations, such as sending GET, POST
	/// (form, JSON, and multipart),  and custom HTTP requests. It also includes functionality for deserializing JSON
	/// content from HTTP responses.</remarks>
	public interface IHttpService
	{
		/// <summary>
		/// Sends an asynchronous GET request to the specified URL with optional headers.
		/// </summary>
		/// <remarks>The caller is responsible for disposing of the returned <see cref="HttpResponseMessage"/> to free
		/// resources.</remarks>
		/// <param name="url">The URL to which the GET request is sent. This cannot be null or empty.</param>
		/// <param name="headers">An optional dictionary of headers to include in the request. Each key-value pair represents a header name and its
		/// value. If null, no additional headers are included.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message received
		/// from the server.</returns>
		Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string>? headers = null);

		/// <summary>
		/// Sends an HTTP POST request with form-encoded data to the specified URL.
		/// </summary>
		/// <remarks>The form fields are sent as application/x-www-form-urlencoded content. If additional headers are
		/// provided, they will be added to the request. The caller is responsible for disposing of the returned <see
		/// cref="HttpResponseMessage"/> to release resources.</remarks>
		/// <param name="url">The URL to which the POST request will be sent. This cannot be null or empty.</param>
		/// <param name="formFields">A dictionary containing the form fields and their corresponding values to be included in the request body. Each
		/// key-value pair represents a form field name and its value. This cannot be null or empty.</param>
		/// <param name="headers">An optional dictionary containing additional HTTP headers to include in the request. Each key-value pair
		/// represents a header name and its value. If null, no additional headers will be included.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="HttpResponseMessage"/>
		/// returned by the server in response to the POST request.</returns>
		Task<HttpResponseMessage> PostFormAsync(string url, Dictionary<string, string> formFields, Dictionary<string, string>? headers = null);

		/// <summary>
		/// Sends a POST request with a JSON payload to the specified URL.
		/// </summary>
		/// <remarks>The payload is serialized into JSON format using the default JSON serializer.  Ensure that the
		/// type <typeparamref name="T"/> is serializable to JSON.</remarks>
		/// <typeparam name="T">The type of the payload to be serialized into JSON.</typeparam>
		/// <param name="url">The URL to which the POST request is sent. This cannot be null or empty.</param>
		/// <param name="payload">The object to be serialized into JSON and included in the request body. This cannot be null.</param>
		/// <param name="headers">An optional dictionary of headers to include in the request. If null, no additional headers are added.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="HttpResponseMessage"/>
		/// returned by the server.</returns>
		Task<HttpResponseMessage> PostJsonAsync<T>(string url, T payload, Dictionary<string, string>? headers = null);

		/// <summary>
		/// Sends an HTTP POST request with multipart/form-data content to the specified URL.
		/// </summary>
		/// <remarks>This method constructs a multipart/form-data request body, including both form fields and file
		/// content, and sends it to the specified URL. Ensure that the provided <see cref="Stream"/> objects for the files
		/// remain open and readable until the request is completed.</remarks>
		/// <param name="url">The URL to which the request is sent. This cannot be null or empty.</param>
		/// <param name="formFields">A dictionary of form field names and their corresponding values to include in the request body. This can be null
		/// if no form fields are required.</param>
		/// <param name="files">A collection of files to include in the request body. Each file is represented as a tuple containing the field
		/// name, file name, content stream, and content type. The <see cref="Stream"/> must be readable and positioned at the
		/// beginning of the content.</param>
		/// <param name="headers">An optional dictionary of additional HTTP headers to include in the request. Keys represent header names, and
		/// values represent header values. This can be null if no additional headers are required.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="HttpResponseMessage"/>
		/// returned by the server.</returns>
		Task<HttpResponseMessage> PostMultipartAsync(string url, Dictionary<string, string> formFields, IEnumerable<(string FieldName, string FileName, Stream Content, string ContentType)> files, Dictionary<string, string>? headers = null);

		/// <summary>
		/// Asynchronously reads and deserializes JSON content from an <see cref="HttpContent"/> instance into an object of
		/// the specified type.
		/// </summary>
		/// <typeparam name="T">The type of the object to deserialize the JSON content into.</typeparam>
		/// <param name="content">The <see cref="HttpContent"/> containing the JSON data to deserialize. Cannot be <see langword="null"/>.</param>
		/// <param name="options">Optional <see cref="JsonSerializerOptions"/> to customize the deserialization process. If <see langword="null"/>,
		/// the default options are used.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object of type
		/// <typeparamref name="T"/>, or <see langword="null"/> if the content is empty or cannot be deserialized.</returns>
		Task<T?> ReadJsonAsync<T>(HttpContent content, JsonSerializerOptions? options = null);

		/// <summary>
		/// Sends an HTTP request asynchronously and returns the response.
		/// </summary>
		/// <remarks>This method does not throw an exception for HTTP response status codes indicating an error (e.g.,
		/// 4xx or 5xx). Callers should inspect the <see cref="HttpResponseMessage.StatusCode"/> property of the returned
		/// response to determine success or failure.</remarks>
		/// <param name="request">The HTTP request message to send. This cannot be <see langword="null"/>.</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message.</returns>
		Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
	}
}
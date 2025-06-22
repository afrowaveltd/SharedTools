namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Provides a service for making HTTP requests, including support for JSON serialization,  form submissions, and
	/// multipart file uploads. This service is designed to interact with  APIs, including those requiring authentication
	/// or specific headers.
	/// </summary>
	/// <remarks>The <see cref="HttpService"/> class simplifies HTTP communication by providing methods  for common
	/// request types such as GET, POST with JSON payloads, form submissions, and  multipart file uploads. It also supports
	/// adding custom headers and deserializing JSON  responses. The service is configured using options provided via an
	/// <see cref="IConfiguration"/>  instance, including base URI, API key, and endpoint paths.  This class is thread-safe
	/// and can be used in concurrent scenarios. Ensure proper  disposal of the underlying <see cref="HttpClient"/> if the
	/// service is no longer needed.</remarks>
	public class HttpService : IHttpService
	{
		private readonly ILogger<HttpService> _logger;
		private readonly HttpClient _client;
		private readonly string _baseUri;
		private readonly string _apiKey;
		private readonly bool _needsKey;

		private readonly JsonSerializerOptions _options = new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpService"/> class, configuring it with the specified settings and
		/// HTTP client.
		/// </summary>
		/// <remarks>This constructor initializes the service with settings for interacting with the LibreTranslate
		/// API. It reads the "LibreTranslateOptions" section from the provided <paramref name="configuration"/> to configure
		/// the API host, endpoints, and optional API key. If the API key is required, an authorization header is added to the
		/// created _client/>.</remarks>

		public HttpService(IConfiguration configuration, ILogger<HttpService> logger)
		{
			LibreTranslateOptions o = configuration.GetSection("LibreTranslateOptions").Get<LibreTranslateOptions>() ?? new();
			_baseUri = o.Host;
			_apiKey = o.ApiKey;
			_needsKey = o.NeedsKey;
			_client = new();
			if(o.NeedsKey)
			{
				_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + o.ApiKey);
			}
			_client.BaseAddress = new Uri(o.Host);
			_logger = logger;
		}

		/// <summary>
		/// Sends an HTTP request asynchronously and returns the response.
		/// </summary>
		/// <remarks>This method delegates the request to an underlying <see cref="HttpClient"/> instance. Ensure
		/// proper disposal of the response to free resources.</remarks>
		/// <param name="request">The HTTP request message to send. This cannot be <see langword="null"/>.</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message received
		/// from the server.</returns>
		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
			=> _client.SendAsync(request, cancellationToken);

		/// <summary>
		/// Reads and deserializes the JSON content of an HTTP response into an object of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of the object to deserialize the JSON content into.</typeparam>
		/// <param name="content">The HTTP content containing the JSON data to deserialize. Cannot be <see langword="null"/>.</param>
		/// <param name="options">Optional. The <see cref="JsonSerializerOptions"/> to use for deserialization. If <see langword="null"/>, default
		/// options for web-based JSON serialization are used.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object of type
		/// <typeparamref name="T"/>, or <see langword="null"/> if the content is empty or cannot be deserialized.</returns>
		public Task<T?> ReadJsonAsync<T>(HttpContent content, JsonSerializerOptions? options = null)
			=> content.ReadFromJsonAsync<T>(options ?? new JsonSerializerOptions(JsonSerializerDefaults.Web));

		/// <summary>
		/// Sends an asynchronous HTTP GET request to the specified URL.
		/// </summary>
		/// <remarks>The method uses an instance of <see cref="HttpClient"/> to send the request.  If headers are
		/// provided, they will be added to the request before it is sent.</remarks>
		/// <param name="url">The URL to which the GET request will be sent. This cannot be null or empty.</param>
		/// <param name="headers">An optional dictionary of headers to include in the request.  Each key-value pair represents a header name and its
		/// corresponding value.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the  <see cref="HttpResponseMessage"/>
		/// returned by the server.</returns>
		public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string>? headers = null)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
			AddHeaders(request, headers);
			return await _client.SendAsync(request);
		}

		/// <summary>
		/// Sends an HTTP POST request with a JSON payload to the specified URL.
		/// </summary>
		/// <remarks>The <paramref name="payload"/> is serialized to JSON using the default JSON serialization
		/// settings. If additional headers are provided via <paramref name="headers"/>, they are added to the
		/// request.</remarks>
		/// <typeparam name="T">The type of the payload to be serialized as JSON.</typeparam>
		/// <param name="url">The URL to which the POST request is sent. Cannot be null or empty.</param>
		/// <param name="payload">The object to be serialized as JSON and sent in the request body. Cannot be null.</param>
		/// <param name="headers">An optional dictionary of headers to include in the request. Keys represent header names, and values represent
		/// header values. If null, no additional headers are added.</param>
		/// <returns>A <see cref="HttpResponseMessage"/> representing the response to the HTTP request. The caller is responsible for
		/// disposing of the response when it is no longer needed.</returns>
		public async Task<HttpResponseMessage> PostJsonAsync<T>(string url, T payload, Dictionary<string, string>? headers = null)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
			{
				Content = JsonContent.Create(payload)
			};
			AddHeaders(request, headers);
			return await _client.SendAsync(request);
		}

		/// <summary>
		/// Sends an HTTP POST request with form-encoded content to the specified URL.
		/// </summary>
		/// <remarks>This method uses the <see cref="HttpClient"/> instance to send the request. The form fields are
		/// encoded as application/x-www-form-urlencoded content. If headers are provided, they are added to the request
		/// before it is sent.</remarks>
		/// <param name="url">The URL to which the POST request will be sent. This cannot be null or empty.</param>
		/// <param name="formFields">A dictionary containing the form fields and their corresponding values to include in the request body. Each
		/// key-value pair represents a form field name and its value.</param>
		/// <param name="headers">An optional dictionary of headers to include in the request. Each key-value pair represents a header name and its
		/// value. If null, no additional headers are added.</param>
		/// <returns>A <see cref="HttpResponseMessage"/> representing the response received from the server. The caller is responsible
		/// for disposing of the response object when it is no longer needed.</returns>
		public async Task<HttpResponseMessage> PostFormAsync(string url, Dictionary<string, string> formFields, Dictionary<string, string>? headers = null)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
			{
				Content = new FormUrlEncodedContent(formFields)
			};
			AddHeaders(request, headers);
			try
			{
				return await _client.SendAsync(request);
			}
			catch
			{
				_logger.LogError("Error sending POST request to {Url}", url);
				return new();
			}
		}

		/// <summary>
		/// Sends an HTTP POST request with multipart form data content to the specified URL.
		/// </summary>
		/// <remarks>This method constructs a multipart form data request, including both form fields and file
		/// uploads, and sends it asynchronously to the specified URL. The caller is responsible for disposing of the returned
		/// <see cref="HttpResponseMessage"/>.</remarks>
		/// <param name="url">The URL to which the request will be sent. This cannot be null or empty.</param>
		/// <param name="formFields">A dictionary containing form field names and their corresponding values to include in the multipart request.</param>
		/// <param name="files">A collection of files to include in the multipart request. Each file is represented as a tuple containing: <list
		/// type="bullet"> <item><description><c>FieldName</c>: The name of the form field for the file.</description></item>
		/// <item><description><c>FileName</c>: The name of the file to be sent.</description></item>
		/// <item><description><c>Content</c>: A <see cref="Stream"/> containing the file's content.</description></item>
		/// <item><description><c>ContentType</c>: The MIME type of the file (e.g., "application/pdf" or
		/// "image/png").</description></item> </list></param>
		/// <param name="headers">An optional dictionary of additional HTTP headers to include in the request. If null, no additional headers are
		/// added.</param>
		/// <returns>A <see cref="HttpResponseMessage"/> representing the response from the server.</returns>
		public async Task<HttpResponseMessage> PostMultipartAsync(string url,
			Dictionary<string, string> formFields,
			IEnumerable<(string FieldName, string FileName, Stream Content, string ContentType)> files,
			Dictionary<string, string>? headers = null)
		{
			using MultipartFormDataContent content = new MultipartFormDataContent();

			// Add form fields
			foreach(KeyValuePair<string, string> field in formFields)
			{
				content.Add(new StringContent(field.Value), field.Key);
			}

			// Add files
			foreach((string FieldName, string FileName, Stream Content, string ContentType) file in files)
			{
				StreamContent streamContent = new StreamContent(file.Content);
				streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
				content.Add(streamContent, file.FieldName, file.FileName);
			}

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
			{
				Content = content
			};

			AddHeaders(request, headers);
			return await _client.SendAsync(request);
		}

		private static void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
		{
			if(headers == null)
			{
				return;
			}

			foreach((string key, string value) in headers)
			{
				_ = request.Headers.TryAddWithoutValidation(key, value);
			}
		}
	}
}
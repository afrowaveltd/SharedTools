namespace Afrowave.SharedTools.Docs.Services.LibreTranslate
{
	/// <summary>
	/// Service for interacting with the LibreTranslate API to perform translations and retrieve language information.
	/// </summary>
	public class LibreTranslateService : ILibreTranslateService
	{
		private readonly LibreTranslateOptions options = new();
		private readonly IHttpService _httpService;
		private readonly ILogger<LibreTranslateService> _logger;
		private readonly string api = string.Empty;

		private readonly JsonSerializerOptions _options = new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="LibreTranslateService"/> class,  configuring it to
		/// communicate with the LibreTranslate API.
		/// </summary>
		/// <remarks>If the "LibreTranslateOptions" section is not found in the configuration, default options will be
		/// used.</remarks>
		/// <param name="httpService">The HTTP service used to send requests to the LibreTranslate API.</param>
		/// <param name="configuration">The application configuration from which the LibreTranslate options are retrieved.  The "LibreTranslateOptions"
		/// section is expected to contain the necessary configuration.</param>
		/// <param name="logger">The logger used to log diagnostic and error information.</param>
		public LibreTranslateService(IHttpService httpService, IConfiguration configuration, ILogger<LibreTranslateService> logger)
		{
			options = configuration.GetSection("LibreTranslateOptions").Get<LibreTranslateOptions>() ?? new();

			_logger = logger;
			_httpService = httpService;
			api = options.ApiKey;
		}

		/// <summary>
		/// Gets the list of supported languages from the LibreTranslate service.
		/// </summary>
		/// <returns>array of two digits language codes supported by the LibreTranslate</returns>
		public async Task<Response<string[]>> GetAvailableLanguagesAsync()
		{
			bool repeat = true;
			int retries = 0;
			Response<string[]> result = new();

			while(repeat || retries < options.RetriesOnFailure)
			{
				try
				{
					var response = await _httpService.GetAsync(options.LanguagesEndpoint);
					response.EnsureSuccessStatusCode();
					var languages = await _httpService.ReadJsonAsync<List<LibreTranslateLanguage>>(response.Content, _options) ?? new();
					if(languages.Count == 0)
					{
						_logger.LogWarning("No supported languages found");
						repeat = false;
						result.Success = true;
						result.Warning = true;
						result.Message += "Warning: No supported languages found";
						retries = 0;
					}

					result.Data = [.. languages.Select(c => c.Code)];
					_logger.LogInformation("{languages.Count} languages found", languages.Count);
					retries = 0;
					repeat = false;
				}
				catch(HttpRequestException e)
				{
					retries++;
					_logger.LogError(e, "Error getting supported languages");
					result.Success = false;
					result.Warning = false;
					result.Message += $"Error: Try {retries}: {e.Message}\n";
				}
			}
			return result;
		}

		// Private methods for comunication with LibreTranslate API proxy methods to give available all methods from the interface

		// get available languages
		private async Task<Response<List<LibreTranslateLanguage>>> GetLanguagesAsync()
		{
			var response = await _httpService.GetAsync(options.LanguagesEndpoint);
			if(!response.IsSuccessStatusCode)
			{
				_logger.LogError("Failed to get languages: {StatusCode}", response.StatusCode);
				Response<List<LibreTranslateLanguage>> res = new()
				{
					Success = false,
					Message = $"Failed to get languages: {response.ReasonPhrase}"
				};
				return res;
			}

			var result = await _httpService.ReadJsonAsync<List<LibreTranslateLanguage>>(response.Content, _options) ?? [];
			Response<List<LibreTranslateLanguage>> responseResult = new()
			{
				Success = true,
				Data = result
			};
			return responseResult;
		}

		/// <summary>
		/// Translates the specified text from the source language to the target language asynchronously.
		/// </summary>
		/// <remarks>This method uses an external translation service to perform the translation. Ensure that the
		/// source and target language codes are valid and supported by the service. If an API key is required by the service,
		/// it must be configured appropriately.</remarks>
		/// <param name="text">The text to be translated. Cannot be null or empty.</param>
		/// <param name="sourceLanguage">The language code of the source text (e.g., "en" for English). Cannot be null or empty.</param>
		/// <param name="targetLanguage">The language code of the target text (e.g., "es" for Spanish). Cannot be null or empty.</param>
		/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The result contains a <see
		/// cref="Response{T}"/> object with the translation result. If the operation fails, the response will indicate the
		/// failure reason.</returns>
		public async Task<Response<LibreTranslationResult>> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage)
		{
			var formFields = new Dictionary<string, string>
			{
				{ "q", text },
				{ "source", sourceLanguage },
				{ "target", targetLanguage },
				{ "format", "text" },
				{ "alternatives", "0" } // Default to no alternatives
			};
			if(!string.IsNullOrEmpty(api) && options.NeedsKey)
			{
				formFields.Add("api_key", api);
			}
			var response = await _httpService.PostFormAsync(options.TranslateEndpoint, formFields);
			if(!response.IsSuccessStatusCode)
			{
				_logger.LogError("Failed to translate text: {StatusCode}", response.StatusCode);
				return Response<LibreTranslationResult>.Fail($"Failed to translate text: {response.ReasonPhrase}");
			}
			var result = await _httpService.ReadJsonAsync<LibreTranslationResult>(response.Content, _options);
			if(result == null)
			{
				_logger.LogError("Failed to deserialize translation result.");
				return Response<LibreTranslationResult>.Fail("Failed to deserialize translation result.");
			}
			return Response<LibreTranslationResult>.Successful(result ?? new(), "Translation successful");
		}

		/// <summary>
		/// Translates the specified text from its detected source language to the target language.
		/// </summary>
		/// <remarks>The source language is automatically detected by the translation service.</remarks>
		/// <param name="text">The text to be translated. Cannot be null or empty.</param>
		/// <param name="targetLanguage">The language code of the target language (e.g., "en" for English, "es" for Spanish).  Must be a valid language
		/// code supported by the translation service.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a  <see cref="Response{T}"/> object
		/// wrapping a <see cref="LibreTranslationResult"/>  with the translation result.</returns>
		public async Task<Response<LibreTranslationResult>> TranslateTextFromAnyLanugageAsync(string text, string targetLanguage)
		{
			return await TranslateTextAsync(text, "auto", targetLanguage);
		}

		/// <summary>
		/// Translates the content of a file from the specified source language to the target language asynchronously.
		/// </summary>
		/// <remarks>The method sends the file and translation parameters to a remote translation service. Ensure that
		/// the provided <paramref name="fileStream"/> is valid and that the source and target language codes are supported by
		/// the service.</remarks>
		/// <param name="fileStream">The stream containing the file to be translated. The stream must be readable and positioned at the beginning.</param>
		/// <param name="sourceLanguage">The language code of the source language (e.g., "en" for English).</param>
		/// <param name="targetLanguage">The language code of the target language (e.g., "fr" for French).</param>
		/// <param name="fileName">The name of the file being translated. This is used for metadata purposes.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with a <see cref="LibreTranslateFileResult"/> indicating the translation result. If the operation fails, the
		/// response will contain an error message.</returns>
		public async Task<Response<LibreTranslateFileResult>> TranslateFileAsync(Stream fileStream, string sourceLanguage, string targetLanguage, string fileName)
		{
			var formFields = new Dictionary<string, string>
			{
				{ "source", sourceLanguage },
				{ "target", targetLanguage },
				{ "format", "text" }
			};
			if(!string.IsNullOrEmpty(api) && options.NeedsKey)
			{
				formFields.Add("api_key", api);
			}
			var files = new List<(string FieldName, string FileName, Stream Content, string ContentType)>
			{
				("file", fileName, fileStream, "application/octet-stream")
			};
			var response = await _httpService.PostMultipartAsync(options.TranslateFileEndpoint, formFields, files);
			if(!response.IsSuccessStatusCode)
			{
				_logger.LogError("Failed to translate file: {StatusCode}", response.StatusCode);
				return Response<LibreTranslateFileResult>.Fail($"Failed to translate file: {response.ReasonPhrase}");
			}
			var result = await _httpService.ReadJsonAsync<LibreTranslateFileResult>(response.Content, _options);
			if(result == null)
			{
				_logger.LogError("Failed to deserialize translation result.");
				return Response<LibreTranslateFileResult>.Fail("Failed to deserialize translation result.");
			}
			return Response<LibreTranslateFileResult>.Successful(result ?? new(), "Translation successful");
		}

		/// <summary>
		/// Translates the content of a file from its detected source language to the specified target language.
		/// </summary>
		/// <remarks>The source language of the file is automatically detected. Ensure that the provided file stream
		/// is valid and that the target language code is supported by the translation service.</remarks>
		/// <param name="fileStream">The stream containing the file to be translated. The stream must be readable and positioned at the beginning.</param>
		/// <param name="targetLanguage">The language code of the target language for the translation (e.g., "en" for English, "es" for Spanish).</param>
		/// <param name="fileName">The name of the file being translated. This is used for metadata purposes and may affect the translation process.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with a <see cref="LibreTranslateFileResult"/> representing the translation result.</returns>
		public async Task<Response<LibreTranslateFileResult>> TranslateFileFromAnyLanguageAsync(Stream fileStream, string targetLanguage, string fileName)
		{
			return await TranslateFileAsync(fileStream, "auto", targetLanguage, fileName);
		}

		/// <summary>
		/// Detects the language of the provided text asynchronously.
		/// </summary>
		/// <remarks>This method sends the provided text to a language detection service and returns the detected
		/// language(s) along with confidence scores. Ensure that the API key is configured if required by the
		/// service.</remarks>
		/// <param name="text">The text for which the language needs to be detected. Cannot be null or empty.</param>
		/// <returns>A <see cref="Response{T}"/> containing the detected language information as a <see cref="Detections"/> object if
		/// the operation is successful. If the operation fails, the response will indicate the failure reason.</returns>
		public async Task<Response<Detections>> DetectLanguageAsync(string text)
		{
			var formFields = new Dictionary<string, string>
			{
				{ "q", text }
			};
			if(!string.IsNullOrEmpty(api) && options.NeedsKey)
			{
				formFields.Add("api_key", api);
			}
			var response = await _httpService.PostFormAsync(options.DetectLanguageEndpoint, formFields);
			if(!response.IsSuccessStatusCode)
			{
				_logger.LogError("Failed to detect language: {StatusCode}", response.StatusCode);
				return Response<Detections>.Fail($"Failed to detect language: {response.ReasonPhrase}");
			}
			var result = await _httpService.ReadJsonAsync<Detections>(response.Content, _options);
			if(result == null)
			{
				_logger.LogError("Failed to deserialize detection result.");
				return Response<Detections>.Fail("Failed to deserialize detection result.");
			}
			return Response<Detections>.Successful(result ?? new(), "Language detection successful");
		}
	}
}
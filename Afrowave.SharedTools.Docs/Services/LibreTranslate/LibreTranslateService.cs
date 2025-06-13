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

		private async Task<Response<LibreTranslationResult>> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage)
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

		private async Task<Response<LibreTranslationResult>> TranslateTextFromAnyLanugageAsync(string text, string targetLanguage)
		{
			return await TranslateTextAsync(text, "auto", targetLanguage);
		}

		private async Task<Response<LibreTranslateFileResult>> TranslateFileAsync(Stream fileStream, string sourceLanguage, string targetLanguage, string fileName)
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

		private async Task<Response<LibreTranslateFileResult>> TranslateFileFromAnyLanguageAsync(Stream fileStream, string targetLanguage, string fileName)
		{
			return await TranslateFileAsync(fileStream, "auto", targetLanguage, fileName);
		}

		private async Task<Response<Detections>> DetectLanguageAsync(string text)
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
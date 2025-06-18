using Afrowave.SharedTools.Docs.Hubs;

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
		private readonly IHubContext<RealtimeHub> _hub;

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
		/// <param name="hub">The signal R hub</param>
		public LibreTranslateService(IHttpService httpService,
			IConfiguration configuration,
			ILogger<LibreTranslateService> logger,
			IHubContext<RealtimeHub> hub)
		{
			_hub = hub;
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
			DateTime start = DateTime.Now;

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
			result.ExecutionTime = DateTime.Now.Subtract(start).Milliseconds;
			return result;
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
			DateTime start = DateTime.Now;
			bool exitTheLoop = false;
			bool decapitalized = false; // Libre translate sometimes fail to translate words with first letter as Capital and just returns original
			int retries = 0;
			int maxRetries = options.RetriesOnFailure;
			if(maxRetries == 0)
				maxRetries = 1;
			int delay = options.WaitSecondBeforeRetry * 1000; // Convert seconds to milliseconds

			if(targetLanguage.Length != 2)
			{
				_logger.LogError("Invalid target language {target}", targetLanguage);
				return new Response<LibreTranslationResult>()
				{
					Success = false,
					Data = new LibreTranslationResult(),
					Message = "Invalid target language",
					ExecutionTime = DateTime.Now.Subtract(start).Milliseconds,
				};
			}

			if(string.IsNullOrEmpty(text))
			{
				_logger.LogWarning("Empty text is pointless to translate");
				return new Response<LibreTranslationResult>()
				{
					Success = false,
					Data = new LibreTranslationResult(),
					Message = "Text cannot be null or empty",
					ExecutionTime = DateTime.Now.Subtract(start).Milliseconds
				};
			}

			if(string.IsNullOrEmpty(sourceLanguage) || sourceLanguage.Length != 2 || sourceLanguage == "auto")
			{
				_logger.LogError("Error source language {lang}", sourceLanguage);
				return new Response<LibreTranslationResult>()
				{
					Success = false,
					Data = new LibreTranslationResult(),
					Message = "Invalid source language",
					ExecutionTime = DateTime.Now.Subtract(start).Milliseconds
				};
			}

			var formFields = new Dictionary<string, string>
			{
				{ "q", text },
				{ "source", sourceLanguage },
				{ "target", targetLanguage },
				{ "format", "text" },
				{ "alternatives", "2" } // Default to no alternatives
			};
			if(!string.IsNullOrEmpty(api) && options.NeedsKey)
			{
				formFields.Add("api_key", api);
			}

			while(!exitTheLoop)
			{
				var response = await _httpService.PostFormAsync(options.TranslateEndpoint, formFields);
				if(!response.IsSuccessStatusCode)
				{
					if(response.StatusCode == System.Net.HttpStatusCode.BadGateway
						|| response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
					{
						if(retries < maxRetries)
						{
							retries++;
							_logger.LogWarning("Error wrong {StatusCode} {retries} try to translate {text} to {language}", response.StatusCode, retries, text, targetLanguage);
							await Task.Delay(delay);
						}
						else
						{
							exitTheLoop = true;
							_logger.LogError("even after {maxRetries} retries, we didn't manage to translate {text}", maxRetries, text);
							return new Response<LibreTranslationResult>
							{
								Data = new LibreTranslationResult() { TranslatedText = text },
								Success = false,
								ExecutionTime = DateTime.Now.Subtract(start).Milliseconds
							};
						}
					}
					exitTheLoop = true;
					return new Response<LibreTranslationResult>
					{
						Data = new LibreTranslationResult() { TranslatedText = text },
						Success = false,
						Message = response.ReasonPhrase,
						ExecutionTime = DateTime.Now.Subtract(start).Milliseconds
					};
				}

				var result = await _httpService.ReadJsonAsync<LibreTranslationResult>(response.Content, _options);
				if(result == null)
				{
					_logger.LogError("Failed to deserialize translation result.");
					return Response<LibreTranslationResult>.Fail("Failed to deserialize translation result.", DateTime.Now.Subtract(start).Milliseconds);
				}
				if(result.TranslatedText == text)
				{
					if(text == text.ToLower() || decapitalized)
					{
						_logger.LogWarning("Translated phrase is the same as orinal");
						return Response<LibreTranslationResult>.Successful(result ?? new(), "Translation successful", DateTime.Now.Subtract(start).Milliseconds);
					}
					else
					{
						_logger.LogInformation("Translated phrase {text} is same as original. Trying with small letters", text);
						text = text.ToLower();
						decapitalized = true;
					}
				}
				else
				{
					_logger.LogInformation("{text} from {sourceLanguage} to {targetLanguage} translated as {result}", text, sourceLanguage, targetLanguage, result.TranslatedText);
					return Response<LibreTranslationResult>.Successful(result ?? new(), "Translation successful", DateTime.Now.Subtract(start).Milliseconds);
				}
			}
			return new();
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
			DateTime start = DateTime.Now;
			bool leaveLoop = false;
			int retries = 0;
			int maxRetries = options.RetriesOnFailure == 0 ? 1 : options.RetriesOnFailure;
			int delay = options.WaitSecondBeforeRetry == 0 ? 1 : options.WaitSecondBeforeRetry;

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

			while(!leaveLoop)
			{
				var response = await _httpService.PostMultipartAsync(options.TranslateFileEndpoint, formFields, files);
				if(response.StatusCode == System.Net.HttpStatusCode.BadGateway
						|| response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
				{
					if(retries < maxRetries)
					{
						retries++;
						_logger.LogWarning("Error wrong {StatusCode} {retries} try to translate {filename} to {language}", response.StatusCode, retries, fileName, targetLanguage);
						await Task.Delay(delay);
					}
					else
					{
						leaveLoop = true;
						_logger.LogError("even after {maxRetries} retries, we didn't manage to translate {filename}", maxRetries, fileName);
						return new Response<LibreTranslateFileResult>
						{
							Data = new LibreTranslateFileResult() { },
							Success = false,
							ExecutionTime = DateTime.Now.Subtract(start).Milliseconds
						};
					}
				}
				if(!response.IsSuccessStatusCode)
				{
					_logger.LogError("Failed to translate file: {StatusCode}", response.StatusCode);
					return Response<LibreTranslateFileResult>.Fail($"Failed to translate file: {response.ReasonPhrase}", DateTime.Now.Subtract(start).Milliseconds);
				}
				var result = await _httpService.ReadJsonAsync<LibreTranslateFileResult>(response.Content, _options);
				if(result == null)
				{
					_logger.LogError("Failed to deserialize translation result.");
					return Response<LibreTranslateFileResult>.Fail("Failed to deserialize translation result.", DateTime.Now.Subtract(start).Milliseconds);
				}
				return Response<LibreTranslateFileResult>.Successful(result ?? new(), "Translation successful", DateTime.Now.Subtract(start).Milliseconds);
			}
			return Response<LibreTranslateFileResult>.Fail("Translation unsuccessful");
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
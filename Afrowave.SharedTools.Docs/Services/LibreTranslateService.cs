using System.Text.Json;
using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.Docs.Services
{
	public class LibreTranslateCommunicationService
	{
		private readonly LibreTranslateOptions options = new();
		private readonly IHttpService _httpService;
		private readonly ILogger<LibreTranslateCommunicationService> _logger;

		private readonly JsonSerializerOptions _options = new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="LibreTranslateCommunicationService"/> class,  configuring it to
		/// communicate with the LibreTranslate API.
		/// </summary>
		/// <remarks>If the "LibreTranslateOptions" section is not found in the configuration, default options will be
		/// used.</remarks>
		/// <param name="httpService">The HTTP service used to send requests to the LibreTranslate API.</param>
		/// <param name="configuration">The application configuration from which the LibreTranslate options are retrieved.  The "LibreTranslateOptions"
		/// section is expected to contain the necessary configuration.</param>
		/// <param name="logger">The logger used to log diagnostic and error information.</param>
		public LibreTranslateCommunicationService(IHttpService httpService, IConfiguration configuration, ILogger<LibreTranslateCommunicationService> logger)
		{
			options = configuration.GetSection("LibreTranslateOptions").Get<LibreTranslateOptions>() ?? new();

			_logger = logger;
			_httpService = httpService;
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
	}
}
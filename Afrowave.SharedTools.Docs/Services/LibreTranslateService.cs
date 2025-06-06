using System.Text.Json;
using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.Docs.Services
{
	public class LibreTranslateService
	{
		private TranslationsOptions translationsOptions = new();
		private LibreTranslateOptions libreTranslateOptions = new();
		private readonly HttpClient _httpClient = new HttpClient();

		private readonly JsonSerializerOptions _options = new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

		public LibreTranslateService(IConfiguration configuration)
		{
			translationsOptions.DefaultLanguage = configuration["Translations:DefaultLanguage"] ?? "en";
			translationsOptions.IgnoredForJson = configuration.GetSection("Translations:IgnoredForJson").Get<string[]>() ?? new[] { "en", "cs" };
			translationsOptions.IgnoredForMd = configuration.GetSection("Translations:IgnoredForMd").Get<string[]>() ?? new[] { "en" };
			translationsOptions.MinutesBetweenCycles = int.TryParse(configuration["Translations:MinutesBetweenCycles"], out var minutes) ? minutes : 20;
		}

		/// <summary>
		/// Gets the list of supported languages from the LibreTranslate service.
		/// </summary>
		/// <returns>array of two digits language codes supported by the LibreTranslate</returns>
		public async Task<Response<string[]>> GetAvailableLanguagesAsync()
		{
			try
			{
				var response = await _httpClient.GetAsync("/languages");
				response.EnsureSuccessStatusCode();
				var languages = await response.Content.ReadFromJsonAsync<string[]>() ?? [];
				return Response<string[]>.Successful(languages, "Languages retrieved successfully.");
			}
			catch(Exception ex)
			{
				return Response<string[]>.Fail($"Error retrieving languages: {ex.Message}");
			}
		}

		/// <summary>
		/// Retrieves a list of supported languages from the translation API.
		/// </summary>
		/// <returns>array of strings or empty array</returns>
		/*
		public async Task<string[]> GetSupportedLanguagesAsync()
		{
			try
			{
			}
			catch(HttpRequestException e)
			{
			}
		}

		public async Task<Response<string>> TranslateAsync(string text, string targetLanguage, string sourceLanguage = "en")
		{
			if(string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(targetLanguage))
			{
				return Response<string>.Fail("Text and target language must be provided.");
			}
		}
		*/
	}
}
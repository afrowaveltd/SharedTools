namespace Afrowave.SharedTools.Docs.Services
{
	public class LibreTranslateService
	{
		private TranslationsOptions translationsOptions = new();
		private LibreTranslateOptions libreTranslateOptions = new();
		private readonly HttpClient _httpClient = new HttpClient();

		public LibreTranslateService(IConfiguration configuration)
		{
			translationsOptions.DefaultLanguage = configuration["Translations:DefaultLanguage"] ?? "en";
			translationsOptions.IgnoredForJson = configuration.GetSection("Translations:IgnoredForJson").Get<string[]>() ?? new[] { "en", "cs" };
			translationsOptions.IgnoredForMd = configuration.GetSection("Translations:IgnoredForMd").Get<string[]>() ?? new[] { "en" };
			translationsOptions.MinutesBetweenCycles = int.TryParse(configuration["Translations:MinutesBetweenCycles"], out var minutes) ? minutes : 20;

			libreTranslateOptions.Host = configuration["LibreTranslate:Host"] ?? "https://libretranslate.de";
			libreTranslateOptions.ApiKey = configuration["LibreTranslate:ApiKey"] ?? string.Empty;
			libreTranslateOptions.NeedsKey = bool.TryParse(configuration["LibreTranslate:NeedsKey"], out var needsKey) && needsKey;

			if(libreTranslateOptions.NeedsKey)
			{
				_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + libreTranslateOptions.ApiKey);
			}
			_httpClient.BaseAddress = new Uri(libreTranslateOptions.Host);
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
	}
}
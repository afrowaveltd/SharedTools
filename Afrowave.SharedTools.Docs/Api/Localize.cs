namespace Afrowave.SharedTools.Docs.Api;

/// <summary>
/// Provides localization and translation services for text, with support for fallback to external translation when
/// localized resources are unavailable.
/// </summary>
/// <remarks>This controller handles requests to translate text into a specified target language. If a localized
/// resource is not found, it attempts to use an external translation service. The target language can be specified in
/// the request or inferred from the `Accept-Language` header. If no target language is provided, English ("en") is used
/// as the default.</remarks>
/// <param name="localizer"></param>
/// <param name="translator"></param>
/// <param name="logger"></param>
/// <param name="httpContextAccessor"></param>
[Route("api/localize")]
[ApiController]
[Produces("application/json")]
public class Localize(IStringLocalizer<Localize> localizer,
	ILibreTranslateService translator,
	ILogger<Localize> logger,
	IHttpContextAccessor httpContextAccessor) : ControllerBase
{
	private readonly IStringLocalizer<Localize> _localizer = localizer;
	private readonly ILibreTranslateService _translator = translator;
	private readonly ILogger<Localize> _logger = logger;
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
	private List<string> supportedLanguages => _translator.GetAvailableLanguagesAsync().Result.Data?.ToList() ?? new();

	/// <summary>
	/// Translates the specified text into the target language or the default language if no target is specified.
	/// </summary>
	/// <remarks>If the target language is "en" (English), the method returns the original text without translation.
	/// If the text  cannot be found in the localization resources, the method attempts to translate it using an external
	/// translation  service. Logs are generated for successful translations, failures, and cases where no translation is
	/// performed.</remarks>
	/// <param name="toTranslate">The text to be translated. This parameter cannot be null or empty.</param>
	/// <param name="target">The target language code (e.g., "fr" for French, "es" for Spanish). If null, the method attempts to use the
	/// "Accept-Language" header from the HTTP request, defaulting to "en" (English) if no header is present.</param>
	/// <returns>An <see cref="IActionResult"/> containing the translated text if successful, or a <see cref="BadRequestResult"/>
	/// if the input text or target language is invalid.</returns>
	[Route("{toTranslate}/{target?}")]
	[HttpGet]
	public async Task<IActionResult> OnGetAsync(string toTranslate, string? target = null)
	{
		string translateTo = target ?? _httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString() ?? "en";
		string translated = toTranslate;
		if(string.IsNullOrEmpty(toTranslate) || string.IsNullOrEmpty(translateTo))
		{
			return BadRequest();
		}

		if(translateTo == "en")
		{
			_logger.LogDebug("No point to translate from English to English");
			return Content(toTranslate, "text/plain", Encoding.UTF8); ;
		}
		LocalizedString result = _localizer[toTranslate];
		if(result.ResourceNotFound)
		{
			var libreResult = await _translator.TranslateTextAsync(toTranslate, "en", translateTo);
			if(libreResult.Success)
			{
				_logger.LogInformation("Translated {toTranslate} to {translateTo} as {result}", toTranslate, translateTo, libreResult.Data);
				translated = libreResult.Data?.TranslatedText ?? toTranslate;
			}
			else
			{
				_logger.LogWarning("Failed to translate {toTranslate} to {translateTo} due to {error}", toTranslate, translateTo, libreResult.Message);
				translated = toTranslate;
				_logger.LogError("Failed to translate {toTranslate} to {translateTo} after 10 attempts", toTranslate, translateTo);
			}
		}
		else
		{
			translated = result;
		}
		return Content(translated, "text/plain", Encoding.UTF8);
	}
}
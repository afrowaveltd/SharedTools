namespace Afrowave.SharedTools.Docs.Api;

[Route("api/[controller]")]
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
			return Ok(toTranslate);
		}
		LocalizedString result = _localizer[toTranslate];
		if(result.ResourceNotFound)
		{
			bool failure = true;
			int failureCount = 0;

			var supportedLanguages = await _translator.GetAvailableLanguagesAsync();
			bool isSupported = supportedLanguages.Data.Contains(translateTo);
			if(!isSupported)
			{
				translated = toTranslate;
			}

			while(failure)
			{
				var libreResult = await _translator.TranslateTextAsync(toTranslate, "en", translateTo);
				if(libreResult.Success)
				{
					_logger.LogInformation("Translated {toTranslate} to {translateTo} as {result}", toTranslate, translateTo, libreResult.Data);
					translated = libreResult.Data.TranslatedText ?? toTranslate;
					failure = false;
				}
				else
				{
					failureCount++;
					_logger.LogWarning("Failed to translate {toTranslate} to {translateTo}", toTranslate, translateTo);
					if(failureCount > 10)
					{
						failure = false;
						translated = toTranslate;
						_logger.LogError("Failed to translate {toTranslate} to {translateTo} after 10 attempts", toTranslate, translateTo);
					}
					await Task.Delay(1000);
				}
			}
		}
		else
		{
			translated = result.Value;
		}
		return Ok(translated);
	}
}
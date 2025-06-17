namespace Afrowave.SharedTools.Docs.Middlewares
{
	/// <summary>
	/// Middleware for handling internationalization (i18n) by setting culture based on cookies or request headers.
	/// </summary>
	public class I18nMiddleware(ICookieService cookieService, ILogger<I18nMiddleware> logger) : IMiddleware
	{
		private readonly ICookieService _cookieService = cookieService;
		private readonly ILogger<I18nMiddleware> _logger = logger;

		/// <summary>
		/// Middleware execution logic to set the culture for the request.
		/// </summary>
		/// <param name="context">The HTTP context.</param>
		/// <param name="next">The next middleware delegate.</param>

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			string? cookieValue = _cookieService.GetCookie("language") == string.Empty ? null : _cookieService.GetCookie("language");
			cookieValue ??= _cookieService.GetCookie("Language") == string.Empty ? null : _cookieService.GetCookie("Language");
			// 1. Determine UI culture
			string uiCultureKey = cookieValue
								 ?? context.Request.Headers.AcceptLanguage.ToString()
								 ?? context.Request.Query["ui-lang"].ToString();

			// 2. Determine formatting culture
			string cultureKey = cookieValue
								?? context.Request.Headers.AcceptLanguage.ToString()
								?? uiCultureKey
								?? "en";

			// 3. Validate and apply cultures
			CultureInfo uiCulture = IsValidCulture(uiCultureKey) ? new CultureInfo(uiCultureKey) : new CultureInfo("en");
			CultureInfo culture = IsValidCulture(cultureKey) ? new CultureInfo(cultureKey) : new CultureInfo("en");

			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = uiCulture;
			context.Request.Headers.AcceptLanguage = uiCulture.TwoLetterISOLanguageName;
			_cookieService.SetCookie("language", uiCulture.TwoLetterISOLanguageName);

			await next(context);
		}

		/// <summary>
		/// Checks if the provided culture exists in the system.
		/// </summary>
		/// <param name="name">The culture name to check.</param>
		/// <returns>True if the culture exists, otherwise false.</returns>
		private static bool IsValidCulture(string? name)
		{
			return CultureInfo.GetCultures(CultureTypes.AllCultures).Any(culture => string.Equals(culture.Name, name, StringComparison.CurrentCultureIgnoreCase));
		}
	}
}
using Afrowave.SharedTools.Api.Services;
using Afrowave.SharedTools.I18N.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.I18N.Middlewares
{
	/// <summary>
	/// Middleware that resolves and applies the current UI culture for the request based on configured sources
	/// (cookie, query string, or Accept-Language header) and falls back to a default.
	/// </summary>
	public class LocalizationMiddleware : IMiddleware
	{
		private readonly ICookieService _cookieService;
		private readonly IOptionsMonitor<LocalizationMiddlewareOptions> _options;

		/// <summary>
		/// Initializes a new instance of <see cref="LocalizationMiddleware"/>.
		/// </summary>
		/// <param name="cookieService">Cookie service used to read the language cookie.</param>
		/// <param name="options">Options monitor providing <see cref="LocalizationMiddlewareOptions"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown when dependencies are null.</exception>
		public LocalizationMiddleware(ICookieService cookieService, IOptionsMonitor<LocalizationMiddlewareOptions> options)
		{
			_cookieService = cookieService ?? throw new ArgumentNullException(nameof(cookieService));
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		/// <inheritdoc />
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));
			if(next == null) throw new ArgumentNullException(nameof(next));

			var cfg = _options.CurrentValue ?? new LocalizationMiddlewareOptions();

			// Resolve language by configured order
			string? lang = null;
			foreach(var source in cfg.OrderedSources)
			{
				if(source == LocalizationSource.Cookie)
				{
					var cookieName = string.IsNullOrWhiteSpace(cfg.CookieName) ? "lang" : cfg.CookieName;
					lang = _cookieService.Read(cookieName);
				}
				else if(source == LocalizationSource.QueryString)
				{
					// Common query keys
					lang = context.Request.Query["lang"].ToString();
					if(string.IsNullOrWhiteSpace(lang)) lang = context.Request.Query["culture"].ToString();
				}
				else if(source == LocalizationSource.AcceptLanguageHeader)
				{
					lang = context.Request.Headers["Accept-Language"].ToString()?.Split(',')[0]?.Trim();
				}

				if(!string.IsNullOrWhiteSpace(lang)) break;
			}

			if(string.IsNullOrWhiteSpace(lang)) lang = cfg.DefaultCulture;

			// Apply culture; on invalid value, fall back to DefaultCulture
			try
			{
				var culture = new CultureInfo(lang);
				CultureInfo.CurrentCulture = culture;
				CultureInfo.CurrentUICulture = culture;
			}
			catch(CultureNotFoundException)
			{
				try
				{
					var fallback = new CultureInfo(string.IsNullOrWhiteSpace(cfg.DefaultCulture) ? "en" : cfg.DefaultCulture);
					CultureInfo.CurrentCulture = fallback;
					CultureInfo.CurrentUICulture = fallback;
				}
				catch(CultureNotFoundException)
				{
					// give up if even fallback is invalid
				}
			}

			await next(context);
		}
	}
}
using Afrowave.SharedTools.Docs.Middlewares;
using Afrowave.SharedTools.Docs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Globalization;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Docs.Middlewares
{
	public class I18nMiddlewareTests
	{
		private readonly ICookieService _cookieService;
		private readonly ILogger<I18nMiddleware> _logger;
		private readonly I18nMiddleware _middleware;

		public I18nMiddlewareTests()
		{
			_cookieService = Substitute.For<ICookieService>();
			_logger = Substitute.For<ILogger<I18nMiddleware>>();
			_middleware = new I18nMiddleware(_cookieService, _logger);
		}

		private async Task<HttpContext> CreateTestHttpContext(string acceptLanguage = "en")
		{
			DefaultHttpContext context = new DefaultHttpContext();
			context.Request.Headers.AcceptLanguage = acceptLanguage;
			return await Task.FromResult(context);
		}

		/// <summary>
		/// Tests if the middleware sets the culture based on the language cookie value.
		/// </summary>
		/// <returns>No return value; it asserts that the culture is set correctly.</returns>
		[Fact]
		public async Task InvokeAsync_ShouldSetCultureFromCookie()
		{
			// Arrange
			HttpContext context = await CreateTestHttpContext();
			_ = _cookieService.GetCookie("language").Returns("fr");

			string? actualCulture = null;
			string? actualUICulture = null;

			// Act
			await _middleware.InvokeAsync(context, (ctx) =>
			{
				actualCulture = CultureInfo.CurrentCulture.Name;
				actualUICulture = CultureInfo.CurrentUICulture.Name;
				return Task.CompletedTask;
			});

			// Assert
			Assert.Equal("fr", actualCulture);
			Assert.Equal("fr", actualUICulture);
			_cookieService.Received(1).SetCookie("language", "fr");
		}

		/// <summary>
		/// Tests the middleware's behavior when no language cookie or header is present, ensuring the default culture is set
		/// to English.
		/// </summary>
		/// <returns>No return value as this is a void method.</returns>
		[Fact]
		public async Task InvokeAsync_ShouldSetDefaultCulture_WhenNoCookieOrHeader()
		{
			// Arrange
			CultureInfo.CurrentCulture = new CultureInfo("en");
			CultureInfo.CurrentUICulture = new CultureInfo("en");

			HttpContext context = await CreateTestHttpContext("");
			_ = _cookieService.GetCookie("language").Returns((string?)null);

			// Act
			await _middleware.InvokeAsync(context, (ctx) => Task.CompletedTask);

			// Assert
			Assert.Equal("en", CultureInfo.CurrentCulture.Name);
			Assert.Equal("en", CultureInfo.CurrentUICulture.Name);
		}

		/// <summary>
		/// Tests the middleware's behavior when the Accept-Language header is used instead of a cookie for language settings.
		/// </summary>
		/// <returns>Verifies that the culture and UI culture are set to the language specified in the Accept-Language header.</returns>
		[Fact]
		public async Task InvokeAsync_ShouldUseAcceptLanguageHeader_WhenNoCookie()
		{
			// Arrange
			HttpContext context = await CreateTestHttpContext("es");
			_ = _cookieService.GetCookie("language").Returns("");

			string? actualCulture = null;
			string? actualUICulture = null;

			// Act
			await _middleware.InvokeAsync(context, (ctx) =>
			{
				actualCulture = CultureInfo.CurrentCulture.Name;
				actualUICulture = CultureInfo.CurrentUICulture.Name;
				return Task.CompletedTask;
			});

			// Assert
			Assert.Equal("es", actualCulture);
			Assert.Equal("es", actualUICulture);
			_cookieService.Received(1).SetCookie("language", "es");
		}

		/// <summary>
		/// Tests the middleware's behavior when an invalid culture is provided, ensuring it defaults to English.
		/// </summary>
		/// <returns>No return value; the method is asynchronous and performs assertions.</returns>
		[Fact]
		public async Task InvokeAsync_ShouldFallbackToEnglish_WhenCultureIsInvalid()
		{
			// Arrange
			CultureInfo.CurrentCulture = new CultureInfo("en");
			CultureInfo.CurrentUICulture = new CultureInfo("en");

			HttpContext context = await CreateTestHttpContext("invalid-culture");
			_ = _cookieService.GetCookie("language").Returns("invalid-culture");

			// Act
			await _middleware.InvokeAsync(context, (ctx) => Task.CompletedTask);

			// Assert
			Assert.Equal("en", CultureInfo.CurrentCulture.Name);
			Assert.Equal("en", CultureInfo.CurrentUICulture.Name);
		}
	}
}
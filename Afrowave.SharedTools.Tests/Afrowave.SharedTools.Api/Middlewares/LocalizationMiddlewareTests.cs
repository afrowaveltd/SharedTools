using Afrowave.SharedTools.Api.Services;
using Afrowave.SharedTools.I18N.Middlewares;
using Afrowave.SharedTools.I18N.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Globalization;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Api.Middlewares;

[Collection("Non-Parallel Collection")]
public class LocalizationMiddlewareTests
{
	private static LocalizationMiddleware Create(ICookieService cookieService, LocalizationMiddlewareOptions options)
	{
		var monitor = Substitute.For<IOptionsMonitor<LocalizationMiddlewareOptions>>();
		monitor.CurrentValue.Returns(options);
		return new LocalizationMiddleware(cookieService, monitor);
	}

	private static DefaultHttpContext Ctx() => new DefaultHttpContext();

	[Fact]
	public async Task Applies_Culture_From_Cookie_When_Configured()
	{
		var ctx = Ctx();
		var cookie = Substitute.For<ICookieService>();
		cookie.Read("lang").Returns("cs-CZ");

		var opts = new LocalizationMiddlewareOptions
		{
			LocalizeByCookie = true,
			LocalizeByQueryString = false,
			LocalizeByAcceptLanguageHeader = false,
			FirstSource = LocalizationSource.Cookie,
			SecondSource = LocalizationSource.None,
			ThirdSource = LocalizationSource.None,
			DefaultCulture = "en"
		};

		var mw = Create(cookie, opts);
		string? applied = null;
		await mw.InvokeAsync(ctx, _ => { applied = CultureInfo.CurrentUICulture.Name; return Task.CompletedTask; });

		Assert.Equal("cs-CZ", applied);
	}

	[Fact]
	public async Task Applies_Culture_From_QueryString_When_First()
	{
		var ctx = Ctx();
		ctx.Request.QueryString = new QueryString("?lang=fr-FR");
		var cookie = Substitute.For<ICookieService>();
		cookie.Read(Arg.Any<string>()).Returns((string?)null);

		var opts = new LocalizationMiddlewareOptions
		{
			LocalizeByQueryString = true,
			LocalizeByCookie = true,
			LocalizeByAcceptLanguageHeader = true,
			FirstSource = LocalizationSource.QueryString,
			SecondSource = LocalizationSource.Cookie,
			ThirdSource = LocalizationSource.AcceptLanguageHeader,
			DefaultCulture = "en"
		};

		var mw = Create(cookie, opts);
		string? applied = null;
		await mw.InvokeAsync(ctx, _ => { applied = CultureInfo.CurrentUICulture.Name; return Task.CompletedTask; });

		Assert.Equal("fr-FR", applied);
	}

	[Fact]
	public async Task Applies_Culture_From_AcceptLanguage_When_Others_Missing()
	{
		var ctx = Ctx();
		ctx.Request.Headers["Accept-Language"] = "de-DE,de;q=0.9";
		var cookie = Substitute.For<ICookieService>();
		cookie.Read(Arg.Any<string>()).Returns((string?)null);

		var opts = new LocalizationMiddlewareOptions
		{
			LocalizeByQueryString = false,
			LocalizeByCookie = false,
			LocalizeByAcceptLanguageHeader = true,
			FirstSource = LocalizationSource.AcceptLanguageHeader,
			SecondSource = LocalizationSource.None,
			ThirdSource = LocalizationSource.None,
			DefaultCulture = "en"
		};

		var mw = Create(cookie, opts);
		string? applied = null;
		await mw.InvokeAsync(ctx, _ => { applied = CultureInfo.CurrentUICulture.Name; return Task.CompletedTask; });

		Assert.Equal("de-DE", applied);
	}

	[Fact]
	public async Task Falls_Back_To_DefaultCulture_When_Invalid_Or_Missing()
	{
		var ctx = Ctx();
		var cookie = Substitute.For<ICookieService>();
		cookie.Read("lang").Returns("xx-invalid");

		var opts = new LocalizationMiddlewareOptions
		{
			LocalizeByCookie = true,
			LocalizeByQueryString = false,
			LocalizeByAcceptLanguageHeader = false,
			FirstSource = LocalizationSource.Cookie,
			SecondSource = LocalizationSource.None,
			ThirdSource = LocalizationSource.None,
			DefaultCulture = "en-GB"
		};

		var mw = Create(cookie, opts);
		string? applied = null;
		await mw.InvokeAsync(ctx, _ => { applied = CultureInfo.CurrentUICulture.Name; return Task.CompletedTask; });

		Assert.Equal("en-GB", applied);
	}
}

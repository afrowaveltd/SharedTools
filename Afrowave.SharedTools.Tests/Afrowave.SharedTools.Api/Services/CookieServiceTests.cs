using Afrowave.SharedTools.Api.Models;
using Afrowave.SharedTools.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Linq;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Api.Services;

public class CookieServiceTests
{
	private static CookieService Create(DefaultHttpContext ctx, out IOptionsMonitor<CookieSettings> monitor)
	{
		var accessor = Substitute.For<IHttpContextAccessor>();
		accessor.HttpContext.Returns(ctx);

		monitor = Substitute.For<IOptionsMonitor<CookieSettings>>();
		monitor.CurrentValue.Returns(new CookieSettings
		{
			Domain = string.Empty,
			Path = "/",
			HttpOnly = false,
			IsEssential = true,
			Secure = false,
			SameSite = CookieSettings.SameSitePolicy.Lax
		});

		return new CookieService(accessor, monitor);
	}

	private static string[] GetSetCookieHeaders(DefaultHttpContext ctx)
	{
		return ctx.Response.Headers.TryGetValue("Set-Cookie", out var values)
			? values.ToArray()
			: Array.Empty<string>();
	}

	[Fact]
	public void Exists_And_Read_Work_From_RequestCookies()
	{
		var ctx = new DefaultHttpContext();
		ctx.Request.Headers["Cookie"] = "my=val; other=x";
		var svc = Create(ctx, out _);

		Assert.True(svc.Exists("my"));
		Assert.Equal("val", svc.Read("my"));
		Assert.False(svc.Exists("missing"));
		Assert.Null(svc.Read("missing"));
	}

	[Fact]
	public void ReadResponse_Returns_Success_When_Found_Else_Fail()
	{
		var ctx = new DefaultHttpContext();
		ctx.Request.Headers["Cookie"] = "k=v";
		var svc = Create(ctx, out _);

		var ok = svc.ReadResponse("k");
		Assert.True(ok.Success);
		Assert.Equal("v", ok.Data);

		var fail = svc.ReadResponse("missing");
		Assert.False(fail.Success);
	}

	[Fact]
	public void Write_Writes_When_Not_Exists_And_Skips_When_Exists()
	{
		var ctx = new DefaultHttpContext();
		var svc = Create(ctx, out _);

		var res = svc.Write("w", "1");
		Assert.True(res.Success);
		var headers = GetSetCookieHeaders(ctx);
		Assert.Contains(headers, h => h.StartsWith("w=", StringComparison.OrdinalIgnoreCase) && h.Contains("w=1"));

		// Simulate existing cookie on request
		ctx = new DefaultHttpContext();
		ctx.Request.Headers["Cookie"] = "w=1";
		svc = Create(ctx, out _);
		headers = GetSetCookieHeaders(ctx);
		Assert.Empty(headers);
		var res2 = svc.Write("w", "2");
		headers = GetSetCookieHeaders(ctx);
		Assert.True(res2.Success);
		// Should not write anything because it already exists
		Assert.Empty(headers);
	}

	[Fact]
	public void Update_Overwrites_Always()
	{
		var ctx = new DefaultHttpContext();
		var svc = Create(ctx, out _);

		var res = svc.Update("u", "A");
		Assert.True(res.Success);
		var headers = GetSetCookieHeaders(ctx);
		Assert.Contains(headers, h => h.StartsWith("u=", StringComparison.OrdinalIgnoreCase) && h.Contains("u=A"));
	}

	[Fact]
	public void Delete_Writes_Deletion_Header()
	{
		var ctx = new DefaultHttpContext();
		ctx.Request.Headers["Cookie"] = "d=z";
		var svc = Create(ctx, out _);

		var res = svc.Delete("d");
		Assert.True(res.Success);
		var headers = GetSetCookieHeaders(ctx);
		Assert.Contains(headers, h => h.StartsWith("d=", StringComparison.OrdinalIgnoreCase) && h.Contains("d=") && h.Contains("expires", StringComparison.OrdinalIgnoreCase));
	}

	[Fact]
	public void ReadOrCreate_Returns_Default_And_Writes_When_Missing()
	{
		var ctx = new DefaultHttpContext();
		var svc = Create(ctx, out _);

		var result = svc.ReadOrCreate("rc", "def");
		Assert.Equal("def", result);
		var headers = GetSetCookieHeaders(ctx);
		Assert.Contains(headers, h => h.Contains("rc=def"));
	}

	[Fact]
	public void UpdateObject_Writes_Json()
	{
		var ctx = new DefaultHttpContext();
		var svc = Create(ctx, out _);

		var res = svc.UpdateObject("obj", new { A = 1 });
		Assert.True(res.Success);
		var headers = GetSetCookieHeaders(ctx);
		Assert.Contains(headers, h => h.StartsWith("obj=", StringComparison.OrdinalIgnoreCase));
	}

	[Fact]
	public void ReadObjectOrCreate_Creates_And_Returns_Default_When_Missing()
	{
		var ctx = new DefaultHttpContext();
		var svc = Create(ctx, out _);

		var def = new TestDto { A = 5 };
		var val = svc.ReadObjectOrCreate("obj2", def);
		Assert.Equal(5, val.A);
		var headers = GetSetCookieHeaders(ctx);
		Assert.Contains(headers, h => h.StartsWith("obj2=", StringComparison.OrdinalIgnoreCase));
	}

	[Fact]
	public void ProfileName_Is_Used_For_Options()
	{
		var ctx = new DefaultHttpContext();
		var accessor = Substitute.For<IHttpContextAccessor>();
		accessor.HttpContext.Returns(ctx);

		var monitor = Substitute.For<IOptionsMonitor<CookieSettings>>();
		monitor.CurrentValue.Returns(new CookieSettings());
		monitor.Get("p").Returns(new CookieSettings { Path = "/p", Domain = "example.com" });

		var svc = new CookieService(accessor, monitor);
		var res = svc.Update("prof", "v", "p");
		Assert.True(res.Success);
		var headers = GetSetCookieHeaders(ctx);
		Assert.Contains(headers, h => h.Contains("path=/p", StringComparison.OrdinalIgnoreCase));
		Assert.Contains(headers, h => h.Contains("domain=example.com", StringComparison.OrdinalIgnoreCase));
	}

	private sealed class TestDto { public int A { get; set; } }
}

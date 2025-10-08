using Afrowave.SharedTools.Api.Models;
using Afrowave.SharedTools.Api.Static;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Api.Static;

[Collection("Non-Parallel Collection")]
public class CookieHelperTests
{
	private static string[] GetSetCookieHeaders(DefaultHttpContext ctx)
		=> ctx.Response.Headers.TryGetValue("Set-Cookie", out var values) ? values.ToArray() : Array.Empty<string>();

	[Fact]
	public void Exists_And_Read_Work()
	{
		var ctx = new DefaultHttpContext();
		ctx.Request.Headers["Cookie"] = "a=1; b=2";
		Assert.True(CookieHelper.Exists(ctx, "a"));
		Assert.Equal("1", CookieHelper.Read(ctx, "a"));
		Assert.False(CookieHelper.Exists(ctx, "x"));
		Assert.Null(CookieHelper.Read(ctx, "x"));
	}

	[Fact]
	public void Write_Skips_When_Already_Exists()
	{
		var ctx = new DefaultHttpContext();
		ctx.Request.Headers["Cookie"] = "w=1";
		var res = CookieHelper.Write(ctx, "w", "new");
		Assert.True(res.Success);
		Assert.Empty(GetSetCookieHeaders(ctx));
	}

	[Fact]
	public void Write_And_Update_SetCookie_Header()
	{
		var ctx = new DefaultHttpContext();
		var res1 = CookieHelper.Write(ctx, "c", "v");
		Assert.True(res1.Success);
		Assert.Contains(GetSetCookieHeaders(ctx), h => h.StartsWith("c=", StringComparison.OrdinalIgnoreCase) && h.Contains("c=v"));

		var res2 = CookieHelper.Update(ctx, "c", "v2");
		Assert.True(res2.Success);
		Assert.Contains(GetSetCookieHeaders(ctx), h => h.Contains("c=v2"));
	}

	[Fact]
	public void Delete_Writes_Expired_SetCookie()
	{
		var ctx = new DefaultHttpContext();
		var res = CookieHelper.Delete(ctx, "d");
		Assert.True(res.Success);
		Assert.Contains(GetSetCookieHeaders(ctx), h => h.StartsWith("d=", StringComparison.OrdinalIgnoreCase) && h.Contains("expires", StringComparison.OrdinalIgnoreCase));
	}

	[Fact]
	public void ReadOrCreate_Writes_When_Missing()
	{
		var ctx = new DefaultHttpContext();
		var value = CookieHelper.ReadOrCreate(ctx, "rc", "def");
		Assert.Equal("def", value);
		Assert.Contains(GetSetCookieHeaders(ctx), h => h.Contains("rc=def"));
	}
}

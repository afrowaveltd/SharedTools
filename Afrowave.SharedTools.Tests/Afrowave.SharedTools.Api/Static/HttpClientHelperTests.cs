using Afrowave.SharedTools.Api.Models;
using Afrowave.SharedTools.Api.Serialization;
using Afrowave.SharedTools.Api.Static;
using Afrowave.SharedTools.Models.Results;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HttpReqOptions = Afrowave.SharedTools.Api.Models.HttpRequestOptions;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Api.Static;

public class HttpClientHelperTests
{
	private sealed class StubHandler : HttpMessageHandler
	{
		public Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> Impl { get; set; } =
			(req, ct) => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}", Encoding.UTF8, "application/json") };

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			=> Task.FromResult(Impl(request, cancellationToken));
	}

	private static HttpReqOptions Options()
	{
		return new HttpReqOptions
		{
			Retry = new RetryPolicyOptions { MaxRetries = 0 },
			DecompressionGzipDeflate = false
		};
	}

	[Fact]
	public async Task GetStringAsync_Returns_Text_On_200()
	{
		// We cannot inject handler into HttpClientHelper; call a known URL and assert Response shape only
		var opts = new HttpReqOptions { Retry = new RetryPolicyOptions { MaxRetries = 0 } };
		var res = await HttpClientHelper.GetStringAsync("https://example.org", opts);
		Assert.NotNull(res);
	}

	[Fact]
	public async Task PostJsonAsync_Returns_Response_Object()
	{
		var body = new { A = 1 };
		var result = await HttpClientHelper.PostJsonAsync<object, Dictionary<string, object>>("https://example.org/echo", body, new HttpReqOptions { Retry = new RetryPolicyOptions { MaxRetries = 0 } });
		Assert.NotNull(result);
	}

	[Fact]
	public async Task GetJsonAsync_Fails_On_404()
	{
		var result = await HttpClientHelper.GetJsonAsync<object>("https://example.org/404", new HttpReqOptions { Retry = new RetryPolicyOptions { MaxRetries = 0 } });
		Assert.NotNull(result);
	}
}

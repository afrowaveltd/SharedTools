using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Afrowave.SharedTools.Api.Models;
using Afrowave.SharedTools.Api.Services;
using Afrowave.SharedTools.Api.Serialization;
using Afrowave.SharedTools.Models.Results;
using NSubstitute;
using Xunit;
using RequestOptions = Afrowave.SharedTools.Api.Models.HttpRequestOptions;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Api.Services;

public sealed class HttpClientTests
{
    [Fact]
    public async Task GetStringAsync_ReturnsSuccess_AndBody()
    {
        // Arrange
        var handler = new StubHandler(async (req, ct) =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("hello world", Encoding.UTF8, "text/plain")
            };
        });
        var client = new HttpClient(handler);
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient("AfrowaveHttpService").Returns(client);
        var sut = new HttpService(factory, new RequestOptions());

        // Act
        var res = await sut.GetStringAsync("https://example.test/echo");

        // Assert
        Assert.True(res.Success);
        Assert.Equal("hello world", res.Data);
    }

    [Fact]
    public async Task GetJsonAsync_Deserializes_ReturnsData()
    {
        // Arrange
        var payload = new Demo { Id = 7, Name = "Alice" };
        var json = JsonSerializer.Serialize(payload, JsonOptions.Default);
        var handler = new StubHandler(async (req, ct) =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        });
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient("AfrowaveHttpService").Returns(new HttpClient(handler));
        var sut = new HttpService(factory, new RequestOptions());

        // Act
        var res = await sut.GetJsonAsync<Demo>("https://example.test/data", null);

        // Assert
        Assert.True(res.Success);
        Assert.NotNull(res.Data);
        Assert.Equal(7, res.Data.Id);
        Assert.Equal("Alice", res.Data.Name);
    }

    [Fact]
    public async Task GetStringAsync_WhenHttpError_ReturnsFail()
    {
        // Arrange
        var handler = new StubHandler(async (req, ct) => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            ReasonPhrase = "ServerError"
        });
        var factory = Substitute.For<IHttpClientFactory>();
        // Return a NEW HttpClient per call to mimic real IHttpClientFactory behavior
        factory.CreateClient("AfrowaveHttpService").Returns(_ => new HttpClient(handler));
        var sut = new HttpService(factory, new RequestOptions());

        // Act
        var res = await sut.GetStringAsync("https://example.test/fail");

        // Assert
        Assert.False(res.Success);
        Assert.Contains("HTTP 500", res.Message);
    }

    [Fact]
    public async Task GetStringAsync_AppliesHeaders_ToRequest()
    {
        // Arrange
        HttpRequestMessage? captured = null;
        var handler = new StubHandler(async (req, ct) =>
        {
            captured = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("ok", Encoding.UTF8, "text/plain")
            };
        });
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient("AfrowaveHttpService").Returns(new HttpClient(handler));
        var sut = new HttpService(factory, new RequestOptions());

        var headers = new Dictionary<string, string>
        {
            ["X-Test"] = "123",
            ["Authorization"] = "Bearer abc"
        };

        // Act
        var res = await sut.GetStringAsync("https://example.test/h", headers);

        // Assert
        Assert.True(res.Success);
        Assert.NotNull(captured);
        Assert.True(captured!.Headers.Contains("X-Test"));
        Assert.True(captured!.Headers.Contains("Authorization"));
    }

    [Fact]
    public async Task PostJsonAsync_SendsBody_AndDeserializesResponse()
    {
        // Arrange
        Demo? received = null;
        var response = new Demo { Id = 99, Name = "OK" };
        var handler = new StubHandler(async (req, ct) =>
        {
            if(req.Content is StringContent sc)
            {
                var sent = await sc.ReadAsStringAsync();
                received = JsonSerializer.Deserialize<Demo>(sent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            var respJson = JsonSerializer.Serialize(response, JsonOptions.Default);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(respJson, Encoding.UTF8, "application/json")
            };
        });
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient("AfrowaveHttpService").Returns(new HttpClient(handler));
        var sut = new HttpService(factory, new RequestOptions());

        // Act
        var res = await sut.PostJsonAsync<Demo, Demo>("https://example.test/post", new Demo { Id = 1, Name = "Bob" });

        // Assert
        Assert.True(res.Success);
        Assert.Equal(99, res.Data.Id);
        Assert.Equal("OK", res.Data.Name);
        Assert.NotNull(received);
        Assert.Equal(1, received!.Id);
        Assert.Equal("Bob", received!.Name);
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responder;
        public StubHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
            => _responder = responder ?? throw new ArgumentNullException(nameof(responder));

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => _responder(request, cancellationToken);
    }

    public sealed class Demo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}

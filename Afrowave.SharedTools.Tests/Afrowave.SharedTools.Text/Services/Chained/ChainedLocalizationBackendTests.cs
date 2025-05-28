using Afrowave.SharedTools.Localization.Common.Models.Backend;
using Afrowave.SharedTools.Localization.Interfaces;
using Afrowave.SharedTools.Localization.Services.Chained;
using Afrowave.SharedTools.Models.Results;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Text.Services.Chained
{
	public class ChainedLocalizationBackendTests
	{
		private class FakeBackend : ILocalizationBackend
		{
			private readonly Dictionary<string, string> _data;
			private readonly bool _shouldFail;
			public string Name => "Fake";

			public LocalizationBackendCapabilities Capabilities => new()
			{
				CanRead = true,
				CanWrite = false
			};

			public FakeBackend(Dictionary<string, string> data, bool shouldFail = false)
			{
				_data = data;
				_shouldFail = shouldFail;
			}

			public Task<Response<string>> GetValueAsync(string language, string key)
			{
				if(_shouldFail)
					throw new InvalidOperationException("Simulated failure");

				return Task.FromResult(
					 _data.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value)
					 ? Response<string>.SuccessResponse(value, "Found")
					 : Response<string>.Fail("Not found")
				);
			}

			public Task<bool> SetValueAsync(string language, string key, string value) => Task.FromResult(false);

			public Task<IDictionary<string, string>> GetAllValuesAsync(string language) => Task.FromResult<IDictionary<string, string>>(new Dictionary<string, string>());

			public Task<bool> SetAllValuesAsync(string language, IDictionary<string, string> values) => Task.FromResult(false);

			public Task<IList<string>> GetAvailableLanguagesAsync() => Task.FromResult<IList<string>>(new List<string>());
		}

		[Fact]
		public async Task Returns_Value_From_First_Backend()
		{
			var backend1 = new FakeBackend(new() { ["hello"] = "ahoj" });
			var backend2 = new FakeBackend(new());

			var chain = new ChainedLocalizationBackend(backend1, backend2);
			var result = await chain.GetValueAsync("cs", "hello");

			Assert.True(result.Success);
			Assert.Equal("ahoj", result.Data);
			Assert.False(result.Warning);
		}

		[Fact]
		public async Task Skips_Failed_Backend_And_Uses_Next()
		{
			var backend1 = new FakeBackend(new(), shouldFail: true);
			var backend2 = new FakeBackend(new() { ["hello"] = "ahoj" });

			var chain = new ChainedLocalizationBackend(backend1, backend2);
			var result = await chain.GetValueAsync("cs", "hello");

			Assert.True(result.Success);
			Assert.Equal("ahoj", result.Data);
		}

		[Fact]
		public async Task Returns_Key_When_All_Backends_Fail()
		{
			var backend1 = new FakeBackend(new());
			var backend2 = new FakeBackend(new());

			var chain = new ChainedLocalizationBackend(backend1, backend2);
			var result = await chain.GetValueAsync("cs", "hello");

			Assert.True(result.Success);
			Assert.True(result.Warning);
			Assert.Equal("hello", result.Data);
		}

		[Fact]
		public async Task Returns_Empty_When_Key_Is_Null_Or_Whitespace()
		{
			var backend1 = new FakeBackend(new());
			var chain = new ChainedLocalizationBackend(backend1);

			var result = await chain.GetValueAsync("cs", " ");

			Assert.True(result.Success);
			Assert.Equal(string.Empty, result.Data);
		}
	}
}
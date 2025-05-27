using Afrowave.SharedTools.Localization.Backend.Mock;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Localization.Backend
{
	public class MockLocalizationBackendTests
	{
		[Fact]
		public async Task Returns_Value_When_Key_Exists()
		{
			var backend = new MockLocalizationBackend();
			backend.SetValue("cs", "hello", "ahoj");

			var response = await backend.GetValueAsync("cs", "hello");

			Assert.True(response.Success);
			Assert.Equal("ahoj", response.Data);
			Assert.False(response.Warning);
		}

		[Fact]
		public async Task Simulates_Warning_Correctly()
		{
			var backend = new MockLocalizationBackend { SimulateWarning = true };
			backend.SetValue("cs", "hello", "nazdar");

			var response = await backend.GetValueAsync("cs", "hello");

			Assert.True(response.Success);
			Assert.True(response.Warning);
			Assert.Equal("nazdar", response.Data);
		}

		[Fact]
		public async Task Simulates_Empty_Success()
		{
			var backend = new MockLocalizationBackend { SimulateEmptySuccess = true };

			var response = await backend.GetValueAsync("cs", "unknown");

			Assert.True(response.Success);
			Assert.Equal(string.Empty, response.Data);
		}

		[Fact]
		public async Task Returns_Failure_When_Key_Missing()
		{
			var backend = new MockLocalizationBackend();

			var response = await backend.GetValueAsync("cs", "nonexistent");

			Assert.False(response.Success);
			Assert.Contains("not found", response.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task Throws_When_Configured()
		{
			var backend = new MockLocalizationBackend { ThrowOnGet = true };

			await Assert.ThrowsAsync<InvalidOperationException>(() =>
				 backend.GetValueAsync("cs", "key"));
		}

		[Fact]
		public async Task Tracks_Number_Of_Calls()
		{
			var backend = new MockLocalizationBackend();
			await backend.GetValueAsync("cs", "a");
			await backend.GetValueAsync("cs", "b");
			await backend.GetValueAsync("cs", "c");

			Assert.Equal(3, backend.CallCount);
		}

		[Fact]
		public async Task Allows_Overwriting_Values()
		{
			var backend = new MockLocalizationBackend();
			backend.SetValue("en", "save", "Save");
			backend.SetValue("en", "save", "Overwrite");

			var response = await backend.GetValueAsync("en", "save");

			Assert.True(response.Success);
			Assert.Equal("Overwrite", response.Data);
		}

		[Fact]
		public async Task Lists_Languages_Correctly()
		{
			var backend = new MockLocalizationBackend();
			backend.SetValue("cs", "k1", "v1");
			backend.SetValue("fr", "k2", "v2");

			var langs = await backend.GetAvailableLanguagesAsync();

			Assert.Contains("cs", langs);
			Assert.Contains("fr", langs);
			Assert.DoesNotContain("de", langs);
		}
	}
}
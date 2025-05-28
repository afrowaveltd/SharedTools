using Afrowave.SharedTools.Localization.Common.Models.Backend;
using Afrowave.SharedTools.Localization.Helpers;
using System.Text.Json;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Localization.Helpers
{
	public class LocalizationBackendHelperTests
	{
		[Fact]
		public async Task FixBackendCapabilitiesAsync_CreatesAndValidatesCorrectly()
		{
			// Arrange
			var tempDir = Path.Combine(Path.GetTempPath(), "AfrowaveTest", Guid.NewGuid().ToString());
			Directory.CreateDirectory(tempDir);

			var backendDir = Path.Combine(tempDir, "Afrowave.JsonLocalizationBackend");
			Directory.CreateDirectory(backendDir);

			var backendIdPath = Path.Combine(backendDir, ".afrowave-backend-id.txt");
			await File.WriteAllLinesAsync(backendIdPath, new[]
			{
				"JsonLocalizationBackend",
				"Afrowave.JsonLocalizationBackend"
		  });

			var expected = new LocalizationBackendCapabilities
			{
				BackendType = "File",
				Description = "Test backend",
				CanRead = true,
				CanWrite = true,
				CanBulkRead = true,
				CanBulkWrite = false,
				SupportsLanguagesListing = true,
				Version = "1.0.0",
				Author = "Test"
			};

			// Act
			await LocalizationBackendHelper.FixBackendCapabilitiesAsync(
				 backendName: "JsonLocalizationBackend",
				 packageName: "Afrowave.JsonLocalizationBackend",
				 capabilities: expected,
				 searchRoot: tempDir
			);

			// Assert
			var capabilitiesPath = Path.Combine(backendDir, "capabilities.json");
			Assert.True(File.Exists(capabilitiesPath));

			var json = await File.ReadAllTextAsync(capabilitiesPath);
			var actual = JsonSerializer.Deserialize<LocalizationBackendCapabilities>(json);

			Assert.NotNull(actual);
			Assert.Equal(expected.BackendType, actual.BackendType);
			Assert.Equal(expected.Description, actual.Description);
			Assert.Equal(expected.CanRead, actual.CanRead);
			Assert.Equal(expected.CanWrite, actual.CanWrite);
			Assert.Equal(expected.CanBulkRead, actual.CanBulkRead);
			Assert.Equal(expected.CanBulkWrite, actual.CanBulkWrite);
			Assert.Equal(expected.SupportsLanguagesListing, actual.SupportsLanguagesListing);
			Assert.Equal(expected.Version, actual.Version);
			Assert.Equal(expected.Author, actual.Author);

			// Cleanup
			Directory.Delete(tempDir, recursive: true);
		}
	}
}
using Afrowave.SharedTools.Docs.I18n;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using NSubstitute;
using System.Globalization;
using System.Reflection;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Docs.I18n
{
	/// <summary>
	/// Unit tests for the <see cref="JsonStringLocalizer"/> class.
	/// </summary>
	public class JsonStringLocalizerTests : IDisposable
	{
		private readonly string _tempLocalesPath;
		private readonly IDistributedCache _mockCache;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonStringLocalizerTests"/> class.
		/// </summary>
		public JsonStringLocalizerTests()
		{
			_tempLocalesPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			_ = Directory.CreateDirectory(_tempLocalesPath);
			_mockCache = Substitute.For<IDistributedCache>();
		}

		/// <summary>
		/// Disposes of the resources used by the test class.
		/// </summary>
		public void Dispose()
		{
			if(Directory.Exists(_tempLocalesPath))
			{
				Directory.Delete(_tempLocalesPath, recursive: true);
			}
		}

		private JsonStringLocalizer CreateLocalizerWithTempLocales()
		{
			JsonStringLocalizer localizer = new JsonStringLocalizer(_mockCache);
			SetPrivateField(localizer, "_localesPath", _tempLocalesPath);
			return localizer;
		}

		private static void SetPrivateField(JsonStringLocalizer instance, string fieldName, object value)
		{
			FieldInfo? field = typeof(JsonStringLocalizer).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
			field?.SetValue(instance, value);
		}

		/// <summary>
		/// Verifies that the <see cref="JsonStringLocalizer"/> constructor initializes the instance correctly.
		/// </summary>
		[Fact]
		public void GetExistingStringWithFormatting_ReturnsFormattedString()
		{
			// Arrange
			CultureInfo.CurrentUICulture = new CultureInfo("en");
			string key = "Welcome";
			string format = "Welcome, {0}!";
			File.WriteAllText(Path.Combine(_tempLocalesPath, "en.json"), $"{{\"{key}\":\"{format}\"}}");

			JsonStringLocalizer localizer = CreateLocalizerWithTempLocales();

			// Act
			LocalizedString result = localizer[key, "Alice"];

			// Assert
			Assert.Equal("Welcome, Alice!", result.Value);
			Assert.False(result.ResourceNotFound);
		}

		/// <summary>
		/// Verifies that the <see cref="JsonStringLocalizer"/> constructor initializes the instance correctly.
		/// </summary>
		[Fact]
		public void GetAllStrings_ReturnsAllKeysFromJsonFile()
		{
			// Arrange
			CultureInfo.CurrentUICulture = new CultureInfo("en");
			Dictionary<string, string> entries = new Dictionary<string, string>
				{
					 { "Key1", "Value1" },
					 { "Key2", "Value2" }
				};
			File.WriteAllText(Path.Combine(_tempLocalesPath, "en.json"), JsonConvert.SerializeObject(entries));
			JsonStringLocalizer localizer = CreateLocalizerWithTempLocales();

			// Act
			List<LocalizedString> results = localizer.GetAllStrings(false).ToList();

			// Assert
			Assert.Equal(2, results.Count);
			Assert.Contains(results, r => r.Name == "Key1" && r.Value == "Value1");
			Assert.Contains(results, r => r.Name == "Key2" && r.Value == "Value2");
		}

		/// <summary>
		/// Verifies that the <see cref="JsonStringLocalizer"/> constructor initializes the instance correctly.
		/// </summary>
		[Fact]
		public void FallbackToDefaultCulture_WhenCultureFileMissing()
		{
			// Arrange
			CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
			string key = "Greeting";
			string value = "Bonjour";
			File.WriteAllText(Path.Combine(_tempLocalesPath, "en.json"), $"{{\"{key}\":\"{value}\"}}");
			JsonStringLocalizer localizer = CreateLocalizerWithTempLocales();

			// Act
			LocalizedString result = localizer[key];

			// Assert
			Assert.Equal(value, result.Value);
			Assert.False(result.ResourceNotFound);
		}

		/// <summary>
		/// Verifies that the <see cref="JsonStringLocalizer"/> constructor initializes the instance correctly.
		/// </summary>
		[Fact]
		public void GetAllStrings_ThrowsFileNotFoundException_WhenDefaultFileMissing()
		{
			// Arrange
			CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
			JsonStringLocalizer localizer = CreateLocalizerWithTempLocales();

			// Act & Assert
			_ = Assert.Throws<FileNotFoundException>(() => localizer.GetAllStrings(false).ToList());
		}
	}
}
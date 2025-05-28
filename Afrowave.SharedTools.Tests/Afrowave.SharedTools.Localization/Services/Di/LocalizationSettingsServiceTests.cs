using Afrowave.SharedTools.Localization.Common.Models.Enums;
using Afrowave.SharedTools.Localization.Services.Di;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Localization.Services.Di
{
	public class LocalizationSettingsServiceTests
	{
		private static string GetUniqueTestPath() =>
			 Path.Combine("TestData", "Di", $"DiTestSettings_{System.Guid.NewGuid():N}.json");

		[Fact]
		public async Task Service_Creates_And_Resets_Settings()
		{
			var testPath = GetUniqueTestPath();
			var dir = Path.GetDirectoryName(testPath)!;
			if(!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			ILocalizationSettingsService service = new LocalizationSettingsService(testPath);

			// Po vytvoření musí být default nastavení
			var settings = await service.GetSettingsAsync();
			Assert.NotNull(settings);
			Assert.Equal("en", settings.DefaultLanguage);

			// Upravíme hodnotu, uložíme
			await service.UpdateAsync(async s =>
			{
				s.DefaultLanguage = "fr";
				await Task.CompletedTask;
			});

			var updated = await service.GetSettingsAsync();
			Assert.Equal("fr", updated.DefaultLanguage);

			// Reset na výchozí hodnoty
			await service.ResetToDefaultAsync();
			var reset = await service.GetSettingsAsync();
			Assert.Equal("en", reset.DefaultLanguage);

			// Úklid
			if(File.Exists(testPath))
				File.Delete(testPath);
		}

		[Fact]
		public async Task Service_Saves_And_Loads_Complex_Settings()
		{
			var testPath = GetUniqueTestPath();
			var dir = Path.GetDirectoryName(testPath)!;
			if(!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			LocalizationSettingsService localizationSettingsService = new(testPath);
			ILocalizationSettingsService service = localizationSettingsService;

			// Nastav složitější nastavení
			await service.UpdateAsync(async s =>
			{
				s.DefaultLanguage = "es";
				s.FallbackLanguage = "en";
				s.SupportedLanguages = ["en", "es", "cs"];
				s.LocaleDetectionOrder = [LocaleDetectionMethod.QueryParameter, LocaleDetectionMethod.Cookie];
				s.AutoTranslateMissing = true;
				s.DebugMode = true;
				await Task.CompletedTask;
			});

			// Nový servis musí umět načíst uložené hodnoty
			LocalizationSettingsService localizationSettingsService1 = new(testPath);
			ILocalizationSettingsService service2 = localizationSettingsService1;
			var settings = await service2.GetSettingsAsync();

			Assert.Equal("es", settings.DefaultLanguage);
			Assert.Equal("en", settings.FallbackLanguage);
			Assert.Contains("cs", settings.SupportedLanguages);
			Assert.Equal(new[] { LocaleDetectionMethod.QueryParameter, LocaleDetectionMethod.Cookie }, settings.LocaleDetectionOrder);
			Assert.True(settings.AutoTranslateMissing);
			Assert.True(settings.DebugMode);

			// Úklid
			if(File.Exists(testPath))
				File.Delete(testPath);
		}
	}
}
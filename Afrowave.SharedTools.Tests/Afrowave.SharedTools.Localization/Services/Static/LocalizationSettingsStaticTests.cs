using Afrowave.SharedTools.Localization.Models;
using Afrowave.SharedTools.Localization.Services.Static;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Localization.Services.Static;

[Collection("Non-Parallel Collection")]
public class LocalizationSettingsStaticTests : System.IDisposable
{
	private static readonly string TestPath =
	 Path.Combine(Path.GetTempPath(), $"LocalizationSettingsStaticTest_{System.Guid.NewGuid():N}.json");

	public LocalizationSettingsStaticTests()
	{
		// Před každým testem – opravdu čistý stav
		if(File.Exists(TestPath))
			File.Delete(TestPath);

		LocalizationSettingsStatic.ResetStaticState();
		LocalizationSettingsStatic.SetSettingsPath(TestPath);
	}

	[Fact]
	public async Task GetSettingsAsync_Creates_Default_If_Missing()
	{
		var settings = await LocalizationSettingsStatic.GetSettingsAsync();
		Assert.NotNull(settings);
		Assert.Equal("en", settings.DefaultLanguage);
		Assert.True(File.Exists(TestPath));
	}

	[Fact]
	public async Task UpdateAsync_Changes_And_Saves()
	{
		await LocalizationSettingsStatic.UpdateAsync(async s =>
		{
			s.FallbackLanguage = "cs";
			await Task.CompletedTask;
		});

		var settings = await LocalizationSettingsStatic.GetSettingsAsync();
		Assert.Equal("cs", settings.FallbackLanguage);

		var fromDisk = System.Text.Json.JsonSerializer.Deserialize<LocalizationSettings>(File.ReadAllText(TestPath));
		Assert.Equal("cs", fromDisk!.FallbackLanguage);
	}

	[Fact]
	public async Task ResetToDefaultAsync_Works()
	{
		await LocalizationSettingsStatic.UpdateAsync(async s =>
		{
			s.DefaultLanguage = "fr";
			await Task.CompletedTask;
		});

		await LocalizationSettingsStatic.ResetToDefaultAsync();
		var reset = await LocalizationSettingsStatic.GetSettingsAsync();
		Assert.Equal("en", reset.DefaultLanguage);
	}

	// Po každém testu – úklid a hard reset
	public void Dispose()
	{
		if(File.Exists(TestPath))
			File.Delete(TestPath);
		LocalizationSettingsStatic.ResetStaticState();
	}
}
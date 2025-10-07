using Afrowave.SharedTools.I18N.Services;
using Afrowave.SharedTools.Models.Localization;
using Afrowave.SharedTools.Models.Results;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.I18N.Services;

[Collection("Non-Parallel Collection")]
public class LanguageServiceTests
{
	private static string LanguagesFilePath => Path.Combine(AppContext.BaseDirectory, "Jsons", "languages.json");

	private static void PrepareLanguagesJson(params Language[] languages)
	{
		var dir = Path.GetDirectoryName(LanguagesFilePath)!;
		Directory.CreateDirectory(dir);
		var json = JsonSerializer.Serialize(languages.ToList());
		File.WriteAllText(LanguagesFilePath, json, Encoding.UTF8);
	}

	[Fact]
	public void Constructor_Throws_When_File_Missing()
	{
		// Arrange
		if (File.Exists(LanguagesFilePath))
		{
			File.Delete(LanguagesFilePath);
		}

		// Act + Assert
		Assert.Throws<FileNotFoundException>(() => new LanguageService());
	}

	[Fact]
	public void GetLanguages_Returns_All_With_Success()
	{
		// Arrange
		PrepareLanguagesJson(
			new Language { Code = "en", Name = "English", Native = "English", Rtl = false },
			new Language { Code = "ar", Name = "Arabic", Native = "العربية", Rtl = true }
		);
		var service = new LanguageService();

		// Act
		var response = service.GetLanguages();

		// Assert
		Assert.True(response.Success);
		Assert.NotNull(response.Data);
		Assert.Equal(2, response.Data!.Count);
	}

	[Fact]
	public void GetLanguageByCode_Finds_Ignoring_Case()
	{
		// Arrange
		PrepareLanguagesJson(new Language { Code = "en", Name = "English", Native = "English", Rtl = false });
		var service = new LanguageService();

		// Act
		var response = service.GetLanguageByCode("EN");

		// Assert
		Assert.True(response.Success);
		Assert.Equal("en", response.Data!.Code);
	}

	[Fact]
	public void GetLanguageByCode_Returns_NotFound_When_Missing()
	{
		// Arrange
		PrepareLanguagesJson(new Language { Code = "en", Name = "English", Native = "English" });
		var service = new LanguageService();

		// Act
		var response = service.GetLanguageByCode("xx");

		// Assert
		Assert.False(response.Success);
		Assert.Contains("Language with code 'xx' not found.", response.Message ?? string.Empty);
	}

	[Fact]
	public void GetLanguagesByCodes_Returns_Warning_For_Missing_Codes()
	{
		// Arrange
		PrepareLanguagesJson(new Language { Code = "en", Name = "English", Native = "English" });
		var service = new LanguageService();

		// Act
		var response = service.GetLanguagesByCodes(new List<string> { "en", "xx" });

		// Assert
		Assert.True(response.Success);
		Assert.True(response.Warning);
		Assert.Single(response.Data!);
		Assert.Contains("Missing codes: xx", response.Message ?? string.Empty);
	}

	[Fact]
	public void AddLanguageToList_Adds_And_Persists()
	{
		// Arrange
		PrepareLanguagesJson(new Language { Code = "en", Name = "English", Native = "English" });
		var service = new LanguageService();

		// Act
		var result = service.AddLanguageToList(new Language { Code = "cs", Name = string.Empty, Native = "Čeština" });

		// Assert
		Assert.True(result.Success);

		// Verify persisted by creating a new instance
		var reloaded = new LanguageService();
		var cs = reloaded.GetRawLanguageByCode("cs");
		Assert.Equal("cs", cs.Code);
		Assert.Equal("cs", cs.Name); // Name is auto-filled when empty
	}

	[Fact]
	public void UpdateLanguageByCode_Changes_Fields_And_Persists()
	{
		// Arrange
		PrepareLanguagesJson(new Language { Code = "ar", Name = "Arabic", Native = "العربية", Rtl = true });
		var service = new LanguageService();

		// Act
		var result = service.UpdateLanguageByCode("ar", new Language { Name = "Arabic (Updated)", Native = "العربية", Rtl = false });

		// Assert
		Assert.True(result.Success);
		var reloaded = new LanguageService();
		var ar = reloaded.GetRawLanguageByCode("ar");
		Assert.Equal("Arabic (Updated)", ar.Name);
		Assert.False(ar.Rtl);
	}

	[Fact]
	public void UpdateLanguageByName_Changes_Code_InMemory()
	{
		// Arrange
		PrepareLanguagesJson(new Language { Code = "cz", Name = "Czech", Native = "Čeština" });
		var service = new LanguageService();

		// Act
		var result = service.UpdateLanguageByName("Czech", new Language { Code = "cs", Native = "Čeština", Rtl = false });

		// Assert
		Assert.True(result.Success);
		var updated = service.GetRawLanguageByCode("cs");
		Assert.Equal("cs", updated.Code);
		Assert.Equal("Czech", updated.Name); // Name unchanged in UpdateByName
	}

	[Fact]
	public void RemoveLanguageFromList_Removes_And_Persists()
	{
		// Arrange
		PrepareLanguagesJson(new Language { Code = "en", Name = "English", Native = "English" });
		var service = new LanguageService();

		// Act
		var result = service.RemoveLanguageFromList("en");

		// Assert
		Assert.True(result.Success);
		var reloaded = new LanguageService();
		var en = reloaded.GetRawLanguageByCode("en");
		Assert.True(string.IsNullOrEmpty(en.Code));
	}

	[Fact]
	public void Substitute_For_ILanguageService_Demonstrates_NSubstitute()
	{
		// Arrange
		var substitute = Substitute.For<ILanguageService>();
		substitute.GetLanguageByCode("en").Returns(new Response<Language>
		{
			Success = true,
			Data = new Language { Code = "en", Name = "English", Native = "English" },
			Message = "OK"
		});

		// Act
		var response = substitute.GetLanguageByCode("en");

		// Assert
		Assert.True(response.Success);
		Assert.Equal("en", response.Data!.Code);
		substitute.Received(1).GetLanguageByCode("en");
	}
}

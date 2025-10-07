using Afrowave.SharedTools.I18N.Static;
using Afrowave.SharedTools.Models.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.I18N.Static
{
	[Collection("Non-Parallel Collection")]
	public class Afrowave_SharedTools_I18N_Static_LanguagesHelperTests
	{
		private static string LanguagesFilePath => Path.Combine(AppContext.BaseDirectory, "Jsons", "languages.json");

		private static void PrepareLanguagesJson(params Language[] languages)
		{
			var dir = Path.GetDirectoryName(LanguagesFilePath)!;
			Directory.CreateDirectory(dir);
			var json = JsonSerializer.Serialize(languages.ToList());
			File.WriteAllText(LanguagesFilePath, json, Encoding.UTF8);
		}

		private static void ResetInMemoryLanguages()
		{
			var helperType = typeof(LanguageHelper);
			var field = helperType.GetField("_languages", BindingFlags.NonPublic | BindingFlags.Static);
			field!.SetValue(null, new List<Language>());
		}

		private static void ReloadFromFileIfExists()
		{
			if(File.Exists(LanguagesFilePath))
			{
				var method = typeof(LanguageHelper).GetMethod("UpdateLanguagesFromJson", BindingFlags.NonPublic | BindingFlags.Static);
				method!.Invoke(null, null);
			}
		}

		[Fact]
		public void GetLanguages_Returns_Failure_When_File_Missing()
		{
			// Arrange
			if(File.Exists(LanguagesFilePath)) File.Delete(LanguagesFilePath);
			ResetInMemoryLanguages();

			// Act
			var response = LanguageHelper.GetLanguages();

			// Assert
			Assert.False(response.Success);
			Assert.Contains("No languages found.", response.Message ?? string.Empty);
		}

		[Fact]
		public void GetLanguages_Returns_All_With_Success()
		{
			// Arrange
			PrepareLanguagesJson(
				new Language { Code = "en", Name = "English", Native = "English", Rtl = false },
				new Language { Code = "ar", Name = "Arabic", Native = "العربية", Rtl = true }
			);
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();

			// Act
			var response = LanguageHelper.GetLanguages();

			// Assert
			Assert.True(response.Success);
			Assert.Equal(2, response.Data!.Count);
		}

		[Fact]
		public void GetLanguageByCode_Finds_Ignoring_Case()
		{
			// Arrange
			PrepareLanguagesJson(new Language { Code = "en", Name = "English", Native = "English", Rtl = false });
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();

			// Act
			var response = LanguageHelper.GetLanguageByCode("EN");

			// Assert
			Assert.True(response.Success);
			Assert.Equal("en", response.Data!.Code);
		}

		[Fact]
		public void GetLanguageByCode_Returns_NotFound_When_Missing()
		{
			// Arrange
			PrepareLanguagesJson(new Language { Code = "en", Name = "English", Native = "English" });
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();

			// Act
			var response = LanguageHelper.GetLanguageByCode("xx");

			// Assert
			Assert.False(response.Success);
			Assert.Contains("Language with code 'xx' not found.", response.Message ?? string.Empty);
		}

		[Fact]
		public void GetLanguagesByCodes_Returns_Warning_For_Missing_Codes()
		{
			// Arrange
			PrepareLanguagesJson(new Language { Code = "en", Name = "English", Native = "English" });
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();

			// Act
			var response = LanguageHelper.GetLanguagesByCodes(new List<string> { "en", "xx" });

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
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();

			// Act
			var result = LanguageHelper.AddLanguageToList(new Language { Code = "cs", Name = string.Empty, Native = "Čeština" });

			// Assert
			Assert.True(result.Success);
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();
			var cs = LanguageHelper.GetRawLanguageByCode("cs");
			Assert.Equal("cs", cs.Code);
			Assert.Equal("cs", cs.Name);
		}

		[Fact]
		public void UpdateLanguageByCode_Changes_Fields_And_Persists()
		{
			// Arrange
			PrepareLanguagesJson(new Language { Code = "ar", Name = "Arabic", Native = "العربية", Rtl = true });
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();

			// Act
			var result = LanguageHelper.UpdateLanguageByCode("ar", new Language { Name = "Arabic (Updated)", Native = "العربية", Rtl = false });

			// Assert
			Assert.True(result.Success);
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();
			var ar = LanguageHelper.GetRawLanguageByCode("ar");
			Assert.Equal("Arabic (Updated)", ar.Name);
			Assert.False(ar.Rtl);
		}

		[Fact]
		public void UpdateLanguageByName_Changes_Code_And_Persists()
		{
			// Arrange
			PrepareLanguagesJson(new Language { Code = "cz", Name = "Czech", Native = "Čeština" });
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();

			// Act
			var result = LanguageHelper.UpdateLanguageByName("Czech", new Language { Code = "cs", Native = "Čeština" });

			// Assert
			Assert.True(result.Success);
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();
			var cs = LanguageHelper.GetRawLanguageByCode("cs");
			Assert.Equal("cs", cs.Code);
			Assert.Equal("Czech", cs.Name);
		}

		[Fact]
		public void RemoveLanguageFromList_Removes_And_Persists()
		{
			// Arrange
			PrepareLanguagesJson(new Language { Code = "en", Name = "English", Native = "English" });
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();

			// Act
			var result = LanguageHelper.RemoveLanguageFromList("en");

			// Assert
			Assert.True(result.Success);
			ResetInMemoryLanguages();
			ReloadFromFileIfExists();
			var en = LanguageHelper.GetRawLanguageByCode("en");
			Assert.True(string.IsNullOrEmpty(en?.Code));
		}
	}
}

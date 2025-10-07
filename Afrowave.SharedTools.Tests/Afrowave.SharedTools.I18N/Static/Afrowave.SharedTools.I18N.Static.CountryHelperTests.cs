using Afrowave.SharedTools.I18N.Static;
using Afrowave.SharedTools.Models.Localization;
using Afrowave.SharedTools.Models.Results;
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
	public class Afrowave_SharedTools_I18N_Static_CountryHelperTests
	{
		private static string CountriesFilePath => Path.Combine(AppContext.BaseDirectory, "Jsons", "countries.json");

		private static void PrepareCountriesJson(params Country[] countries)
		{
			var dir = Path.GetDirectoryName(CountriesFilePath)!;
			Directory.CreateDirectory(dir);
			var json = JsonSerializer.Serialize(countries.ToList());
			File.WriteAllText(CountriesFilePath, json, Encoding.UTF8);
		}

		private static void ReloadCountryHelperFromFile()
		{
			var helperType = typeof(CountryHelper);
			// Reset the private static field _countries to ensure we reload from file
			var field = helperType.GetField("_countries", BindingFlags.NonPublic | BindingFlags.Static);
			field!.SetValue(null, new List<Country>());
			// Invoke the private static method UpdateCountriesFromJson to load from file
			var method = helperType.GetMethod("UpdateCountriesFromJson", BindingFlags.NonPublic | BindingFlags.Static);
			method!.Invoke(null, null);
		}

		[Fact]
		public void GetCountries_Returns_Failure_When_File_Missing()
		{
			// Arrange
			if (File.Exists(CountriesFilePath)) File.Delete(CountriesFilePath);
			ReloadCountryHelperFromFile();

			// Act
			var response = CountryHelper.GetCountries();

			// Assert
			Assert.False(response.Success);
			Assert.Contains("No countries found.", response.Message ?? string.Empty);
		}

		[Fact]
		public void GetCountries_Returns_All_With_Success()
		{
			// Arrange
			PrepareCountriesJson(
				new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" },
				new Country { Code = "CZ", Name = "Czech Republic", Dial_code = "+420", Emoji = "🇨🇿" }
			);
			ReloadCountryHelperFromFile();

			// Act
			var response = CountryHelper.GetCountries();

			// Assert
			Assert.True(response.Success);
			Assert.Equal(2, response.Data!.Count);
		}

		[Fact]
		public void GetCountryByCode_Finds_Ignoring_Case()
		{
			// Arrange
			PrepareCountriesJson(new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" });
			ReloadCountryHelperFromFile();

			// Act
			var response = CountryHelper.GetCountryByCode("us");

			// Assert
			Assert.True(response.Success);
			Assert.Equal("US", response.Data!.Code);
		}

		[Fact]
		public void GetCountryByCode_Returns_NotFound_When_Missing()
		{
			// Arrange
			PrepareCountriesJson(new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" });
			ReloadCountryHelperFromFile();

			// Act
			var response = CountryHelper.GetCountryByCode("XX");

			// Assert
			Assert.False(response.Success);
			Assert.Contains("Country with code 'XX' not found.", response.Message ?? string.Empty);
		}

		[Fact]
		public void GetCountriesByCodes_Returns_Warning_For_Missing()
		{
			// Arrange
			PrepareCountriesJson(new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" });
			ReloadCountryHelperFromFile();

			// Act
			var response = CountryHelper.GetCountriesByCodes(new List<string> { "US", "XX" });

			// Assert
			Assert.True(response.Success);
			Assert.True(response.Warning);
			Assert.Single(response.Data!);
			Assert.Contains("Missing codes: XX", response.Message ?? string.Empty);
		}

		[Fact]
		public void AddCountryToList_Adds_And_Persists()
		{
			// Arrange
			PrepareCountriesJson(new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" });
			ReloadCountryHelperFromFile();

			// Act
			var result = CountryHelper.AddCountryToList(new Country { Code = "GB", Name = string.Empty, Dial_code = "+44", Emoji = "🇬🇧" });

			// Assert
			Assert.True(result.Success);
			// Ensure persistence by clearing in-memory and reloading from file
			ReloadCountryHelperFromFile();
			var raw = CountryHelper.GetRawCountryByCode("GB");
			Assert.Equal("GB", raw.Code);
			Assert.Equal("GB", raw.Name);
		}

		[Fact]
		public void UpdateCountryByCode_Changes_Fields_And_Persists()
		{
			// Arrange
			PrepareCountriesJson(new Country { Code = "CZ", Name = "Czech Republic", Dial_code = "+420", Emoji = "🇨🇿" });
			ReloadCountryHelperFromFile();

			// Act
			var result = CountryHelper.UpdateCountryByCode("CZ", new Country { Name = "Czechia", Dial_code = "+420", Emoji = "🇨🇿" });

			// Assert
			Assert.True(result.Success);
			ReloadCountryHelperFromFile();
			var updated = CountryHelper.GetRawCountryByCode("CZ");
			Assert.Equal("Czechia", updated.Name);
		}

		[Fact]
		public void UpdateCountryByName_Changes_Code_And_Persists()
		{
			// Arrange
			PrepareCountriesJson(new Country { Code = "CZ", Name = "Czech Republic", Dial_code = "+420", Emoji = "🇨🇿" });
			ReloadCountryHelperFromFile();

			// Act
			var result = CountryHelper.UpdateCountryByName("Czech Republic", new Country { Code = "CZE", Dial_code = "+420", Emoji = "🇨🇿" });

			// Assert
			Assert.True(result.Success);
			ReloadCountryHelperFromFile();
			var updated = CountryHelper.GetRawCountryByCode("CZE");
			Assert.Equal("CZE", updated.Code);
			Assert.Equal("Czech Republic", updated.Name);
		}

		[Fact]
		public void RemoveCountryFromList_Removes_And_Persists()
		{
			// Arrange
			PrepareCountriesJson(new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" });
			ReloadCountryHelperFromFile();

			// Act
			var result = CountryHelper.RemoveCountryFromList("US");

			// Assert
			Assert.True(result.Success);
			ReloadCountryHelperFromFile();
			var removed = CountryHelper.GetRawCountryByCode("US");
			Assert.True(string.IsNullOrEmpty(removed.Code));
		}
	}
}

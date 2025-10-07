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
using Xunit;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.I18N.Services;

[Collection("Non-Parallel Collection")]
public class CountryServiceTest
{
	private static string CountriesFilePath => Path.Combine(AppContext.BaseDirectory, "Jsons", "countries.json");

	private static void PrepareCountriesJson(params Country[] countries)
	{
		var dir = Path.GetDirectoryName(CountriesFilePath)!;
		Directory.CreateDirectory(dir);
		var json = JsonSerializer.Serialize(countries.ToList());
		File.WriteAllText(CountriesFilePath, json, Encoding.UTF8);
	}

	[Fact]
	public void Constructor_Throws_When_File_Missing()
	{
		// Arrange
		if(File.Exists(CountriesFilePath))
		{
			File.Delete(CountriesFilePath);
		}

		// Act + Assert
		Assert.Throws<FileNotFoundException>(() => new CountryService());
	}

	[Fact]
	public void GetCountries_Returns_All_With_Success()
	{
		// Arrange
		PrepareCountriesJson(
			new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" },
			new Country { Code = "CZ", Name = "Czech Republic", Dial_code = "+420", Emoji = "🇨🇿" }
		);
		var service = new CountryService();

		// Act
		var response = service.GetCountries();

		// Assert
		Assert.True(response.Success);
		Assert.NotNull(response.Data);
		Assert.Equal(2, response.Data!.Count);
	}

	[Fact]
	public void GetCountryByCode_Finds_Ignoring_Case()
	{
		// Arrange
		PrepareCountriesJson(
			new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" }
		);
		var service = new CountryService();

		// Act
		var response = service.GetCountryByCode("us");

		// Assert
		Assert.True(response.Success);
		Assert.NotNull(response.Data);
		Assert.Equal("US", response.Data!.Code);
	}

	[Fact]
	public void GetCountryByCode_Returns_NotFound_When_Missing()
	{
		// Arrange
		PrepareCountriesJson(
			new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" }
		);
		var service = new CountryService();

		// Act
		var response = service.GetCountryByCode("XX");

		// Assert
		Assert.False(response.Success);
		Assert.Contains("Language with code 'XX' not found.", response.Message ?? string.Empty);
	}

	[Fact]
	public void GetCountriesByCodes_Returns_Warning_For_Missing_Codes()
	{
		// Arrange
		PrepareCountriesJson(
			new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" }
		);
		var service = new CountryService();

		// Act
		var response = service.GetCountriesByCodes(new List<string> { "US", "XX" });

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
		PrepareCountriesJson(
			new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" }
		);
		var service = new CountryService();

		// Act
		var result = service.AddCountryToList(new Country { Code = "GB", Name = string.Empty, Dial_code = "+44", Emoji = "🇬🇧" });

		// Assert
		Assert.True(result.Success);

		// Verify persisted by creating a new instance
		var reloaded = new CountryService();
		var gb = reloaded.GetRawCountryByCode("GB");
		Assert.Equal("GB", gb.Code);
		Assert.Equal("GB", gb.Name); // Name is auto-filled to Code when empty
	}

	[Fact]
	public void UpdateCountryByName_Changes_Code_And_Persists()
	{
		// Arrange
		PrepareCountriesJson(
			new Country { Code = "CZ", Name = "Czech Republic", Dial_code = "+420", Emoji = "🇨🇿" }
		);
		var service = new CountryService();

		// Act
		var result = service.UpdateCountryByName("Czech Republic", new Country { Code = "CZE", Dial_code = "+420", Emoji = "🇨🇿" });

		// Assert
		Assert.True(result.Success);
		var reloaded = new CountryService();
		var cze = reloaded.GetRawCountryByCode("CZE");
		Assert.Equal("CZE", cze.Code);
		Assert.Equal("Czech Republic", cze.Name); // Name is not changed by UpdateCountryByName
	}

	[Fact]
	public void RemoveCountryFromList_Removes_And_Persists()
	{
		// Arrange
		PrepareCountriesJson(
			new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" }
		);
		var service = new CountryService();

		// Act
		var result = service.RemoveCountryFromList("US");

		// Assert
		Assert.True(result.Success);
		var reloaded = new CountryService();
		var us = reloaded.GetRawCountryByCode("US");
		Assert.True(string.IsNullOrEmpty(us.Code));
	}

	[Fact]
	public void Substitute_For_ICountryService_Demonstrates_NSubstitute()
	{
		// Arrange
		var substitute = Substitute.For<ICountryService>();
		substitute.GetCountryByCode("US").Returns(new Response<Country>
		{
			Success = true,
			Data = new Country { Code = "US", Name = "United States", Dial_code = "+1", Emoji = "🇺🇸" },
			Message = "OK"
		});

		// Act
		var response = substitute.GetCountryByCode("US");

		// Assert
		Assert.True(response.Success);
		Assert.Equal("US", response.Data!.Code);
		substitute.Received(1).GetCountryByCode("US");
	}
}
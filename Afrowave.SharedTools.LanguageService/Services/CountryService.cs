using Afrowave.SharedTools.Models.Localization;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.I18N.Services
{
	/// <summary>
	/// Provides country metadata management backed by a JSON file. Allows reading, querying,
	/// adding, removing, and updating <see cref="Country"/> entries and persists changes to disk.
	/// </summary>
	/// <remarks>
	/// The service loads countries from a JSON file located at <c>Jsons/countries.json</c> under the application's base directory.
	/// All modifying operations update the in-memory list and persist the changes back to the JSON file.
	/// </remarks>
	public class CountryService : ICountryService
	{
		private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

		private readonly string filePath = Path.Combine(AppContext.BaseDirectory, "Jsons", "countries.json");
		private List<Country> _countries = new List<Country>();

		/// <summary>
		/// Initializes a new instance of the <see cref="CountryService"/> class and loads countries from the JSON file.
		/// </summary>
		/// <exception cref="FileNotFoundException">Thrown when the countries JSON file does not exist.</exception>
		public CountryService()
		{
			if(!File.Exists(filePath))
			{
				throw new FileNotFoundException($"The file at path {filePath} was not found.");
			}
			UpdateCountriesFromJson();
		}

		/// <summary>
		/// Gets the in-memory list of countries.
		/// </summary>
		/// <returns>The current list of <see cref="Country"/> entries.</returns>
		public List<Country> GetRawCountries()
		{
			return _countries;
		}

		/// <summary>
		/// Gets the countries wrapped in a standardized response object.
		/// </summary>
		/// <returns>A response containing the countries list and status information.</returns>
		public Response<List<Country>> GetCountries()
		{
			var response = new Response<List<Country>>()
			{
				Data = new List<Country>(_countries),
				Success = true,
				Message = "OK"
			};
			if(response.Data == null || response.Data.Count == 0)
			{
				response.Success = false;
				response.Message = "No countries found.";
			}

			return response;
		}

		/// <summary>
		/// Finds a country by its code and returns the raw object.
		/// </summary>
		/// <param name="code">The country ISO code to search for.</param>
		/// <returns>The matching <see cref="Country"/>, or a new empty instance if not found or input is invalid.</returns>
		public Country GetRawCountryByCode(string code)
		{
			if(string.IsNullOrEmpty(code))
			{
				return new Country();
			}
			var country = _countries.Find(coun => string.Equals(coun.Code, code, StringComparison.OrdinalIgnoreCase));
			return country ?? new Country();
		}

		/// <summary>
		/// Finds a country by its code and returns a standardized response.
		/// </summary>
		/// <param name="code">The country ISO code to search for.</param>
		/// <returns>A <see cref="Response{T}"/> with the found <see cref="Country"/> or an error message if not found.</returns>
		public Response<Country> GetCountryByCode(string code)
		{
			var response = new Response<Country>()
			{
				Data = GetRawCountryByCode(code),
				Success = true,
				Message = "OK"
			};
			if(response.Data == null || string.IsNullOrWhiteSpace(response.Data.Code))
			{
				response.Success = false;
				response.Message = $"Language with code '{code}' not found.";
			}
			return response;
		}

		/// <summary>
		/// Finds countries matching any of the provided codes and returns the raw list.
		/// </summary>
		/// <param name="codes">The list of country ISO codes to search for.</param>
		/// <returns>A list of matching <see cref="Country"/> entries; empty when none match or no codes are provided.</returns>
		public List<Country> GetRawCountriesByCodes(List<string> codes)
		{
			if(codes == null || codes.Count == 0)
			{
				return new List<Country>();
			}
			var countries = _countries.FindAll(lang => codes.Exists(code => string.Equals(lang.Code, code, StringComparison.OrdinalIgnoreCase)));
			return countries;
		}

		/// <summary>
		/// Finds countries matching the provided codes and returns a standardized response.
		/// </summary>
		/// <param name="codes">The list of country ISO codes to search for.</param>
		/// <returns>A <see cref="Response{T}"/> containing the matching countries and status information. Includes a warning if some codes are missing.</returns>
		public Response<List<Country>> GetCountriesByCodes(List<string> codes)
		{
			var response = new Response<List<Country>>()
			{
				Data = GetRawCountriesByCodes(codes),
				Success = true,
				Message = "OK"
			};
			if(response.Data == null || response.Data.Count == 0)
			{
				response.Success = false;
				response.Message = "No countries found for the provided codes.";
			}
			if(response.Data.Count != codes.Count)
			{
				var foundCodes = new HashSet<string>(response.Data.ConvertAll(lang => lang.Code), StringComparer.OrdinalIgnoreCase);
				var missingCodes = codes.FindAll(code => !foundCodes.Contains(code));
				response.Message += $" Missing codes: {string.Join(", ", missingCodes)}";
				response.Warning = true;
			}
			return response;
		}

		/// <summary>
		/// Adds a new country to the list and persists the change.
		/// </summary>
		/// <param name="country">The country to add.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public Result AddCountryToList(Country country)
		{
			if(country == null)
			{
				return Result.Fail("Country object can't be empty");
			}

			if(country.Code == null)
			{
				return Result.Fail("Country code can't be null");
			}
			if(country.Name == null || country.Name == string.Empty)
			{
				country.Name = country.Code;
			}

			if(_countries.Where(s => s.Code == country.Code).Any())
			{
				return Result.Fail("Country code already exists in the database");
			}

			try
			{
				_countries.Add(country);
				StoreCountriesToJson();
				UpdateCountriesFromJson();
				return Result.Ok("Country added");
			}
			catch(Exception ex)
			{
				return Result.Fail(ex.Message);
			}
		}

		/// <summary>
		/// Removes a country identified by its ISO code and persists the change.
		/// </summary>
		/// <param name="countryCode">The ISO code of the country to remove.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public Result RemoveCountryFromList(string countryCode)
		{
			if(string.IsNullOrEmpty(countryCode))
			{
				return Result.Fail("Can't delete the country as the object is null");
			}
			var toDelete = _countries.FirstOrDefault(s => s.Code.Equals(countryCode));
			if(toDelete == null)
			{
				return Result.Fail("Country code not found");
			}
			try
			{
				_countries.Remove(toDelete);
				StoreCountriesToJson();
				return Result.Ok("Country removed");
			}
			catch(Exception ex)
			{
				return Result.Fail(ex.Message);
			}
		}

		/// <summary>
		/// Updates a country found by its ISO code and persists the change.
		/// </summary>
		/// <param name="code">The current ISO code of the country to update.</param>
		/// <param name="country">The new country values to apply.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public Result UpdateCountryByCode(string code, Country country)
		{
			if(string.IsNullOrEmpty(code) || country == null)
			{
				return Result.Fail("Code and Country object is required for the update");
			}

			var toUpdate = _countries.FirstOrDefault(s => s.Code.Equals(code));

			if(toUpdate == null)
			{
				return Result.Fail("Language object was not found in the list");
			}

			if(!string.IsNullOrEmpty(country.Name))
			{
				toUpdate.Name = country.Name;
			}
			if(!string.IsNullOrEmpty(country.Dial_code))
			{
				toUpdate.Dial_code = country.Dial_code;
			}
			if(!string.IsNullOrEmpty(country.Emoji))
			{
				toUpdate.Emoji = country.Emoji;
			}
			StoreCountriesToJson();

			return Result.Ok("Country updated successfully");
		}

		/// <summary>
		/// Updates a country found by its name and persists the change.
		/// </summary>
		/// <param name="name">The current name of the country to update.</param>
		/// <param name="country">The new country values to apply.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public Result UpdateCountryByName(string name, Country country)
		{
			if(string.IsNullOrEmpty(name) || country == null)
			{
				return Result.Fail("Name and Country object is required for the update");
			}

			var toUpdate = _countries.FirstOrDefault(s => s.Name.Equals(name));

			if(toUpdate == null)
			{
				return Result.Fail("Country object was not found in the list");
			}

			if(!string.IsNullOrEmpty(country.Code))
			{
				toUpdate.Code = country.Code;
			}
			if(!string.IsNullOrEmpty(country.Dial_code))
			{
				toUpdate.Dial_code = country.Dial_code;
			}
			if(!string.IsNullOrEmpty(country.Emoji))
			{
				toUpdate.Emoji = country.Emoji;
			}

			StoreCountriesToJson();
			return Result.Ok("Language updated successfully");
		}

		/// <summary>
		/// Loads countries from the JSON file into the in-memory list.
		/// </summary>
		private void UpdateCountriesFromJson()
		{
			try
			{
				var jsonString = File.ReadAllText(filePath, Encoding.UTF8);
				_countries = JsonSerializer.Deserialize<List<Country>>(jsonString, _options) ?? new List<Country>();
				if(_countries == null)
				{
					_countries = new List<Country>();
				}
			}
			catch
			{
				_countries = new List<Country>();
			}
		}

		/// <summary>
		/// Persists the current in-memory countries list to the JSON file.
		/// </summary>
		/// <returns>A <see cref="Result"/> indicating success or failure of the persistence operation.</returns>
		private Result StoreCountriesToJson()
		{
			try
			{
				var jsonString = JsonSerializer.Serialize(_countries, _options);
				File.WriteAllText(filePath, jsonString);
				return Result.Ok("Countries file stored successfully");
			}
			catch(Exception ex)
			{
				return Result.Fail(ex.Message);
			}
		}
	}
}
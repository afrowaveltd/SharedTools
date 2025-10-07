using Afrowave.SharedTools.Models.Localization;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.I18N.Static
{
	/// <summary>
	/// Static helper for working with country metadata backed by a JSON file.
	/// Provides functionality similar to the country service interfaces, but without DI/lifetime management.
	/// </summary>
	/// <remarks>
	/// Countries are loaded from a JSON file located at <c>Jsons/countries.json</c> under the application's base directory.
	/// All write operations update the in-memory list and persist the changes back to the JSON file.
	/// </remarks>
	public static class CountryHelper
	{
		private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

		private static readonly string FilePath =
				Path.Combine(AppContext.BaseDirectory, "Jsons", "countries.json");

		private static List<Country> _countries = new List<Country>();

		static CountryHelper()
		{
			if(File.Exists(FilePath))
			{
				UpdateCountriesFromJson();
			}
		}

		// ---------------------------
		// 🧩 READ METHODS
		// ---------------------------

		/// <summary>
		/// Gets a shallow copy of all countries currently loaded in memory.
		/// </summary>
		/// <returns>A new <see cref="System.Collections.Generic.List{Country}"/> with the current entries.</returns>
		public static List<Country> GetRawCountries()
		{
			return new List<Country>(_countries);
		}

		/// <summary>
		/// Gets all countries wrapped in a standardized response object.
		/// </summary>
		/// <returns>A <see cref="Response{List{Country}}"/> containing the countries and status information.</returns>
		public static Response<List<Country>> GetCountries()
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
		/// Finds a country by its ISO code and returns the raw object.
		/// </summary>
		/// <param name="code">The country ISO code to search for.</param>
		/// <returns>The matching <see cref="Country"/>, or a new empty instance if not found or input is invalid.</returns>
		public static Country GetRawCountryByCode(string code)
		{
			if(string.IsNullOrWhiteSpace(code))
				return new Country();
			return _countries.Find(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase)) ?? new Country();
		}

		/// <summary>
		/// Finds a country by its ISO code and returns a standardized response.
		/// </summary>
		/// <param name="code">The country ISO code to search for.</param>
		/// <returns>A <see cref="Response{Country}"/> with the found country or an error message if not found.</returns>
		public static Response<Country> GetCountryByCode(string code)
		{
			var response = new Response<Country>
			{
				Data = GetRawCountryByCode(code),
				Success = true,
				Message = "OK"
			};
			if(response.Data == null || string.IsNullOrWhiteSpace(response.Data.Code))
			{
				response.Success = false;
				response.Message = $"Country with code '{code}' not found.";
			}

			return response;
		}

		/// <summary>
		/// Finds all countries matching any of the provided ISO codes and returns a raw list.
		/// </summary>
		/// <param name="codes">The list of country ISO codes to search for.</param>
		/// <returns>A list of matching <see cref="Country"/> entries; empty when none match or no codes are provided.</returns>
		public static List<Country> GetRawCountriesByCodes(List<string> codes)
		{
			if(codes == null || codes.Count == 0)
				return new List<Country>();

			return _countries.FindAll(count =>
			codes.Exists(code => string.Equals(count.Code, code, StringComparison.OrdinalIgnoreCase)));
		}

		/// <summary>
		/// Finds countries matching the provided ISO codes and returns a standardized response.
		/// </summary>
		/// <param name="codes">The list of country ISO codes to search for.</param>
		/// <returns>A <see cref="Response{List{Country}}"/> with the matching countries. Includes a warning if some codes are missing.</returns>
		public static Response<List<Country>> GetCountriesByCodes(List<string> codes)
		{
			var response = new Response<List<Country>>
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
				var foundCodes = new HashSet<string>(
						response.Data.ConvertAll(lang => lang.Code),
						StringComparer.OrdinalIgnoreCase);

				var missingCodes = codes.FindAll(code => !foundCodes.Contains(code));
				var missingCodesTxt = string.Join(", ", missingCodes);
				response.Message += $" Missing codes: {missingCodesTxt}";
				response.Warning = true;
			}
			return response;
		}

		// ---------------------------
		// ✍️ WRITE METHODS
		// ---------------------------

		/// <summary>
		/// Adds a new country to the list and persists the change.
		/// </summary>
		/// <param name="country">The country to add.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public static Result AddCountryToList(Country country)
		{
			if(country == null)
				return Result.Fail("Country object can't be empty");

			if(string.IsNullOrEmpty(country.Code))
			{
				if(string.IsNullOrEmpty(country.Name))
				{
					return Result.Fail("Missing both Country code and Country name");
				}
				else country.Code = country.Name;
			}
			if(string.IsNullOrEmpty(country.Name))
			{
				country.Name = country.Code;
			}
			if(_countries.Any(s => s.Code.Equals(country.Code, StringComparison.OrdinalIgnoreCase)))
				return Result.Fail("Country code already exists in the database");
			try
			{
				_countries.Add(country);
				StoreCountriesToJson();
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
		public static Result RemoveCountryFromList(string countryCode)
		{
			if(string.IsNullOrEmpty(countryCode))
				return Result.Fail("Can't delete the country as the code is empty");

			var toDelete = _countries.FirstOrDefault(s => s.Code.Equals(countryCode, StringComparison.OrdinalIgnoreCase));
			if(toDelete == null)
				return Result.Fail("Country code not found");

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
		/// <param name="country">The new values to apply.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public static Result UpdateCountryByCode(string code, Country country)
		{
			if(string.IsNullOrEmpty(code) || country == null)
				return Result.Fail("Code and country object are required for update");

			var toUpdate = _countries.FirstOrDefault(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
			if(toUpdate == null)
				return Result.Fail("Country object not found in the list");

			if(!string.IsNullOrEmpty(country.Name))
				toUpdate.Name = country.Name;

			if(!string.IsNullOrEmpty(country.Dial_code))
				toUpdate.Dial_code = country.Dial_code;

			if(!string.IsNullOrEmpty(country.Emoji))
				toUpdate.Emoji = country.Emoji;

			StoreCountriesToJson();
			return Result.Ok("Country updated successfully");
		}

		/// <summary>
		/// Updates a country found by its name and persists the change.
		/// </summary>
		/// <param name="name">The current name of the country to update.</param>
		/// <param name="country">The new values to apply.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public static Result UpdateCountryByName(string name, Country country)
		{
			if(string.IsNullOrEmpty(name) || country == null)
				return Result.Fail("Name and country object are required for update");

			var toUpdate = _countries.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
			if(toUpdate == null)
				return Result.Fail("Country object not found in the list");

			if(!string.IsNullOrEmpty(country.Code))
				toUpdate.Code = country.Code;

			if(!string.IsNullOrEmpty(country.Dial_code))
				toUpdate.Dial_code = country.Dial_code;

			if(!string.IsNullOrEmpty(country.Emoji))
				toUpdate.Emoji = country.Emoji;

			StoreCountriesToJson();
			return Result.Ok("Country updated successfully");
		}

		// ---------------------------
		// 💾 FILE HANDLING
		// ---------------------------

		/// <summary>
		/// Loads countries from the JSON file into the in-memory list.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when the JSON content cannot be deserialized.</exception>
		private static void UpdateCountriesFromJson()
		{
			try
			{
				var json = File.ReadAllText(FilePath);
				_countries = JsonSerializer.Deserialize<List<Country>>(json, _options) ?? new List<Country>();
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException("Failed to load countries from the JSON file.", ex);
			}
		}

		/// <summary>
		/// Persists the current in-memory countries list to the JSON file.
		/// </summary>
		/// <returns>A <see cref="Result"/> indicating success or failure of the persistence operation.</returns>
		private static Result StoreCountriesToJson()
		{
			try
			{
				var jsonString = JsonSerializer.Serialize(_countries, _options);
				File.WriteAllText(FilePath, jsonString);
				return Result.Ok("Countries file stored successfully");
			}
			catch(Exception ex)
			{
				return Result.Fail(ex.Message);
			}
		}
	}
}
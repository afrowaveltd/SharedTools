using Afrowave.SharedTools.Models.Localization;
using Afrowave.SharedTools.Models.Results;
using System.Collections.Generic;

namespace Afrowave.SharedTools.I18N.Services
{
    /// <summary>
    /// Defines operations for managing countries including retrieval, addition, removal,
    /// and updates. Methods provide both raw models and standardized responses.
    /// </summary>
    public interface ICountryService
    {
        /// <summary>
        /// Adds a new <see cref="Country"/> to the collection and persists the change.
        /// </summary>
        /// <param name="country">The country to add.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        Result AddCountryToList(Country country);

        /// <summary>
        /// Gets all available countries wrapped in a standardized response.
        /// </summary>
        /// <returns>A <see cref="Response{List{Country}}"/> containing the countries.</returns>
        Response<List<Country>> GetCountries();

        /// <summary>
        /// Gets countries that match the provided ISO codes wrapped in a standardized response.
        /// </summary>
        /// <param name="codes">The list of ISO codes to search for.</param>
        /// <returns>A <see cref="Response{List{Country}}"/> with the matching countries. May include a warning if some codes are missing.</returns>
        Response<List<Country>> GetCountriesByCodes(List<string> codes);

        /// <summary>
        /// Gets a country by its ISO code wrapped in a standardized response.
        /// </summary>
        /// <param name="code">The country ISO code to search for.</param>
        /// <returns>A <see cref="Response{Country}"/> containing the matching <see cref="Country"/> or error information.</returns>
        Response<Country> GetCountryByCode(string code);

        /// <summary>
        /// Gets all available countries as a raw list.
        /// </summary>
        /// <returns>A new <see cref="List{Country}"/> containing the current entries.</returns>
        List<Country> GetRawCountries();

        /// <summary>
        /// Gets countries that match the provided ISO codes as a raw list.
        /// </summary>
        /// <param name="codes">The list of ISO codes to search for.</param>
        /// <returns>A <see cref="List{Country}"/> of matching countries.</returns>
        List<Country> GetRawCountriesByCodes(List<string> codes);

        /// <summary>
        /// Gets a country by its ISO code as a raw object.
        /// </summary>
        /// <param name="code">The country ISO code to search for.</param>
        /// <returns>The matching <see cref="Country"/>, or a default instance if not found.</returns>
        Country GetRawCountryByCode(string code);

        /// <summary>
        /// Removes a country specified by its ISO code and persists the change.
        /// </summary>
        /// <param name="countryCode">The ISO code of the country to remove.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        Result RemoveCountryFromList(string countryCode);

        /// <summary>
        /// Updates a country identified by its ISO code and persists the change.
        /// </summary>
        /// <param name="code">The current ISO code of the country to update.</param>
        /// <param name="country">The new values to apply to the country.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        Result UpdateCountryByCode(string code, Country country);

        /// <summary>
        /// Updates a country identified by its name and persists the change.
        /// </summary>
        /// <param name="name">The current name of the country to update.</param>
        /// <param name="country">The new values to apply to the country.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        Result UpdateCountryByName(string name, Country country);
    }
}
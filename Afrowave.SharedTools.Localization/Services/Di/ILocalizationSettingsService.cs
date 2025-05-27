using Afrowave.SharedTools.Localization.Models;
using System;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Services.Di
{
	/// <summary>
	/// Service for managing localization settings.
	/// </summary>
	public interface ILocalizationSettingsService
	{
		/// <summary>
		/// Gets the current localization settings asynchronously.
		/// </summary>
		/// <returns>The current <see cref="LocalizationSettings"/>.</returns>
		Task<LocalizationSettings> GetSettingsAsync();

		/// <summary>
		/// Updates the localization settings asynchronously using the provided update function.
		/// </summary>
		/// <param name="update">A function that applies updates to the <see cref="LocalizationSettings"/>.</param>
		Task UpdateAsync(Func<LocalizationSettings, Task> update);

		/// <summary>
		/// Resets the localization settings to their default values asynchronously.
		/// </summary>
		Task ResetToDefaultAsync();
	}
}
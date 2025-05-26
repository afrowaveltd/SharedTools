using Afrowave.SharedTools.Localization.Models;
using System;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Services.Di
{
	public interface ILocalizationSettingsService
	{
		Task<LocalizationSettings> GetSettingsAsync();

		Task UpdateAsync(Func<LocalizationSettings, Task> update);

		Task ResetToDefaultAsync();
	}
}
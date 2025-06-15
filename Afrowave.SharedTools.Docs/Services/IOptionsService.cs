using Microsoft.AspNetCore.Mvc.Rendering;

namespace Afrowave.SharedTools.Docs.Services
{
	public interface IOptionsService
	{
		List<SelectListItem> BinaryOptions(bool selected = true);
		List<SelectListItem> GetLanguagesOptionsAsync(string selected);
		List<SelectListItem> GetThemes(string? selected);
	}
}
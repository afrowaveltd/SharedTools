using Afrowave.SharedTools.Docs.Models;

namespace Afrowave.SharedTools.Docs.Services
{
	public interface ILanguageService
	{
		Response<Language>? GetLanguageByCode(string code);
		List<Language> GetLanguages();
	}
}
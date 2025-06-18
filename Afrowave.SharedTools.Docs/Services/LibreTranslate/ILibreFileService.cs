
namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

public interface ILibreFileService
{
	bool DoesDefaultLanguageExist();
	bool DoesOldDefaultExist();
	Task<TranslationTree> GetAllTranslationsAsync();
	Task<Dictionary<string, string>> GetDefaultLanguageAsync();
	Task<Dictionary<string, string>> GetOldDefaultAsync();
	void SaveOldDefaultAsync();
	Task SaveTranslationsAsync(TranslationTree translations);
}
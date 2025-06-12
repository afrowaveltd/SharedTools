
namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

public interface ILibreTranslateService
{
	Task<Response<string[]>> GetAvailableLanguagesAsync();
}
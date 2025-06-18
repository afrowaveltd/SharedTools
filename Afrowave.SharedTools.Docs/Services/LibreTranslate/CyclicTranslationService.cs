namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

public class CyclicTranslationService(ILibreFileService fileService,
	ILibreTranslateService translateService,
	ILogger<CyclicTranslationService> logger) : ICyclicTranslationService
{
	public async Task RunCycleAsync()
	{
		Console.WriteLine("Running");
	}
}
using Afrowave.SharedTools.Docs.Hubs;

namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

public class CyclicTranslationService(ILibreFileService fileService,
	DocsDbContext context,
	IConfiguration configuration,
	IHubContext<OpenHub> openHub,
	IHubContext<RealtimeHub> realtimeHub,
	ILanguageService languageService,
	ILibreTranslateService translateService,
	ILogger<CyclicTranslationService> logger) : ICyclicTranslationService
{
	private readonly DocsDbContext _context = context;
	private readonly IHubContext<OpenHub> _openHub = openHub;
	private readonly TranslationsOptions _translationsOptions = configuration.GetSection("TranslationsOptions").Get<TranslationsOptions>() ?? new TranslationsOptions();
	private readonly IHubContext<RealtimeHub> _realtimeHub = realtimeHub;
	private readonly ILanguageService _languageService = languageService;
	private readonly ILibreFileService _fileService = fileService;
	private readonly ILibreTranslateService _translateService = translateService;
	private readonly ILogger<CyclicTranslationService> _logger = logger;

	public async Task RunCycleAsync()
	{
		// Server status

		HostedServiceStatus.Clear();
		HostedServiceStatus.Status = WorkerStatus.Checks;
		await _openHub.Clients.All.SendAsync("StatusChanged", "Cycle Started");

		var langArray = await _translateService.GetAvailableLanguagesAsync();
		if(langArray is null || langArray.Success == false)
		{
			HostedServiceStatus.Status = WorkerStatus.Iddle;
			await _openHub.Clients.All.SendAsync("StatusChanged", "Cycle Finished");
			return;
		}

		var languagesResponse = _languageService.GetSelectedLanguagesInfo(langArray.Data.ToList() ?? new());
		if(languagesResponse is null || languagesResponse.Success == false)
		{
			HostedServiceStatus.Status = WorkerStatus.Iddle;
			await _openHub.Clients.All.SendAsync("StatusChanged", "Cycle Finished");
			return;
		}

		var languages = languagesResponse.Data;
		if(languages is null || languages.Count == 0)
		{
			HostedServiceStatus.Status = WorkerStatus.Iddle;
			await _openHub.Clients.All.SendAsync("StatusChanged", "Cycle Finished");
			return;
		}
		HostedServiceStatus.LibreLanguages = languages;
		await _realtimeHub.Clients.All.SendAsync("ReceiveLanguages", languages);
		await _realtimeHub.Clients.All.SendAsync("ReceiveTranslationSettings", _translationsOptions);
		// A - JSON Dictionary translation

		HostedServiceStatus.Status = WorkerStatus.JsonBackendDataLoading;
		await _openHub.Clients.All.SendAsync("StatusChanged", "JSON backend data loading");

		// B - MD Files translation

		// C - Old DB dataCleaning
	}
}
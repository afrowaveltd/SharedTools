using Afrowave.SharedTools.Docs.Hubs;
using Afrowave.SharedTools.Docs.Models.SignalR;

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
	private List<JsonDictionaryTranslationLanguageStatus> _jsonTranslationStatuses = [];
	private readonly DocsDbContext _context = context;
	private readonly IHubContext<OpenHub> _openHub = openHub;
	private TranslationsOptions? _translationsOptions = configuration.GetSection("TranslationsOptions").Get<TranslationsOptions>() ?? new TranslationsOptions();
	private readonly IHubContext<RealtimeHub> _realtimeHub = realtimeHub;
	private readonly ILanguageService _languageService = languageService;
	private readonly ILibreFileService _fileService = fileService;
	private readonly ILibreTranslateService _translateService = translateService;
	private readonly ILogger<CyclicTranslationService> _logger = logger;
	private List<JsonTranslationRequest> jsonQueue = [];

	/// <summary>
	/// Executes a single operational cycle asynchronously, performing various tasks such as updating server status,
	/// retrieving available languages, and broadcasting translation settings.
	/// </summary>
	/// <remarks>This method performs the following steps: <list type="bullet"> <item>Notifies connected clients
	/// about the start of a new cycle.</item> <item>Retrieves available languages and selected language
	/// information.</item> <item>Updates the server's hosted service status based on the success or failure of language
	/// retrieval.</item> <item>Broadcasts the retrieved languages and translation settings to connected clients.</item>
	/// </list> If any critical step fails (e.g., language retrieval), the cycle is terminated early, and the status is
	/// updated accordingly.</remarks>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public async Task RunCycleAsync()
	{
		// Server status
		_logger.LogInformation("Cycle starts");
		HostedServiceStatus.Clear();
		await SendCycleStarts();
		HostedServiceStatus.Status = WorkerStatus.Checks;
		_translationsOptions = configuration.GetSection("TranslationsOptions").Get<TranslationsOptions>();
		HostedServiceStatus.TranslationOptions = _translationsOptions;
		if(_translationsOptions == null)
		{
			HostedServiceStatus.Status = WorkerStatus.Iddle;
			_logger.LogError("Translation options were not found - check your appsettings.json file");
			await SendError();
			return;
		}
		var langArray = await _translateService.GetAvailableLanguagesAsync();
		if(langArray is null || langArray.Success == false)
		{
			HostedServiceStatus.Status = WorkerStatus.Iddle;
			_logger.LogError("We were unable to receive the languages list from the server");
			await SendError();
			return;
		}

		var languagesResponse = _languageService.GetSelectedLanguagesInfo(langArray.Data.ToList() ?? new());
		if(languagesResponse is null || languagesResponse.Success == false)
		{
			HostedServiceStatus.Status = WorkerStatus.Iddle;
			_logger.LogError("Unable to get the language description due to {error}", languagesResponse.Message);
			await SendError();
			return;
		}

		var languages = languagesResponse.Data;
		if(languages is null || languages.Count == 0)
		{
			HostedServiceStatus.Status = WorkerStatus.Iddle;
			_logger.LogWarning("Connection was established to the translation server, but an empty list of Languages received");
			await SendError();
			return;
		}

		HostedServiceStatus.LibreLanguages = languages;
		await SendLanguagesAndSettingsLoaded(_translationsOptions ?? new(), languages);

		// A - JSON Dictionary translation //
		HostedServiceStatus.Status = WorkerStatus.JsonBackendDataLoading;
		await SendStatusChanged("JSON backend data loading");

		/* check presence of languages names in each language file */
		HostedServiceStatus.Status = WorkerStatus.CheckLanguageNames;
		await SendStatusChanged("Checking translation of the language names");

		var dictionary = await _fileService.GetDefaultLanguageAsync();

		List<string> englishLanguageNames = [.. languages.Select(s => s.Name)];
		List<string> translatedLangugeNames = [];

		if(_translationsOptions?.DefaultLanguage == "en")
		{
			// easier - just use what is in the table as a key and value
			translatedLangugeNames = englishLanguageNames;
		}
		else
		{
			int languageCount = englishLanguageNames.Count;
			int translatedCount = 0;
			int errorCount = 0;
			// harder - we must first translate name to the default language and then add them to its file
			foreach(var name in englishLanguageNames)
			{
				var translationResponse = await _translateService.TranslateTextAsync(name, "en", _translationsOptions?.DefaultLanguage ?? "en");
				if(translationResponse.Success)
				{
					translatedLangugeNames.Add(translationResponse.Data?.TranslatedText ?? name);
					translatedCount++;
				}
				else
				{
					errorCount++;
				}
				await SendLanguageNameTranslationProgress(languageCount, translatedCount, errorCount);
			}

			errorCount = 0;
		}
		/* now we have set of phrazes to add to the default language */
		foreach(var language in translatedLangugeNames)
		{
			try
			{
				string? value = dictionary?[language];
				if(string.IsNullOrEmpty(value))
				{
					dictionary?.Add(language, language);
				}
			}
			catch
			{
				dictionary?.Add(language, language);
			}
		}
		await SendLanguageNamesTranslationsDone(languages);
		/* default language dictionary is ready */

		HostedServiceStatus.Status = WorkerStatus.OldDictionaryLoading;
		await SendStatusChanged("Checking old translations");

		Task.Delay(200).Wait();
		var oldDictionary = await _fileService.GetOldDefaultAsync();

		var defaultLanguageData = new JsonDictionaryTranslationLanguageStatus()
		{
			LanguageCode = _translationsOptions?.DefaultLanguage,
			ExistingPhrases = dictionary.Count,
			ToAddPhrases = 0,
			ToDeletePhrases = 0,
			ToUpdatePhrases = 0,
			IsDefault = true,
			IsDone = true
		};

		if(oldDictionary.Any())
		{
			// old dictionary found, let compare its content with actual state
		}

		await _realtimeHub.Clients.All.SendAsync("JsonDictionaryTranslationStateChanged", defaultLanguageData);
		// now we send default language data

		/* Now we get information about the default language and old translation file*/

		// B - MD Files translation

		// C - Old DB dataCleaning
	}

	private async Task SendCycleStarts()
	{
		await _realtimeHub.Clients.All.SendAsync("NewCycle");
		await _openHub.Clients.All.SendAsync("StatusChanged", "Cycle Started");
	}

	private async Task SendError()
	{
		await SendStatusChanged("Error - Cycle skipped");
	}

	private async Task SendLanguagesAndSettingsLoaded(TranslationsOptions options, List<Language> languages)
	{
		await _realtimeHub.Clients.All.SendAsync("ReceiveTranslationSettings", options);
		await Task.Delay(50);
		await _realtimeHub.Clients.All.SendAsync("ReceiveLanguages", languages);
	}

	private async Task SendLanguageNamesTranslationsDone(List<Language> languages)
	{
		await _realtimeHub.Clients.All.SendAsync("LanguageNamesTranslationFinished", languages);
	}

	private async Task SendLanguageNameTranslationProgress(int all, int translated, int errors)
	{
		await _realtimeHub.Clients.All.SendAsync("LanguageNameTranslationChanged", all, translated);
		await _realtimeHub.Clients.All.SendAsync("LanguageNameTranslationError", errors);
	}

	private async Task SendStatusChanged(string statusText)
	{
		await _openHub.Clients.All.SendAsync("StatusChanged", statusText);
	}

	// here we put private SignalR methods and HostedServiceStatus updating stuff

	// first we define methods for an open hub

	// then we define methods for a realtime hub
}
namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

public class LibreFileService(ILogger<LibreFileService> logger, IConfiguration configuration, ILibreTranslateService translateService)
{
	private readonly ILogger<LibreFileService> _logger = logger;
	private readonly ILibreTranslateService _translateService = translateService;
	private readonly TranslationsOptions _translationsOptions = configuration.GetSection("TranslationsOptions").Get<TranslationsOptions>() ?? new();
	private readonly string afrowavePath = Path.Combine(Path.GetTempPath(), "afrowave");
	private readonly string oldTranslation = Path.Combine(Path.GetTempPath(), "afrowave", "old.json");
	private readonly string localesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0], "Locales");

	public async Task<TranslationTree> GetAllTranslationsAsync()
	{
		var langResponse = await _translateService.GetAvailableLanguagesAsync();
		if(langResponse == null || langResponse.Success == false)
		{
			return new TranslationTree();
		}
		var languages = langResponse.Data;
		TranslationTree result = new();
		result.Translations = [];
		foreach(var language in languages)
		{
			var path = Path.Combine(localesPath, language + ".json");
			if(!File.Exists(path))
			{
				result.Translations[language] = new();
			}
			else
			{
				try
				{
					string jsonDictionary = await File.ReadAllTextAsync(path);
					try
					{
						Dictionary<string, string> translation = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonDictionary) ?? new();
						result.Translations[language] = translation;
					}
					catch
					{
						// file was found, but json seems corrupted - we will create a backup
						try
						{
							var target = Path.Combine(afrowavePath, language + DateTime.Now.ToString() + "-backup.json");
							File.Copy(path, target);
							_logger.LogWarning("Lanuage file for {language} is corrupted. Backed up", language);
						}
						catch
						{
							_logger.LogError("Language file for {language} is corrupted and couldnt be backed up", language);
						}
						result.Translations[language] = new();
					}
				}
				catch(Exception ex)
				{
					result.Translations[language] = new();
					_logger.LogInformation("Dictionary for {language} was not found", language);
				}
			}
		}
		return result;
	}

	public async Task<Dictionary<string, string>> GetDefaultLanguageAsync()
	{
		string defaultLanguage = _translationsOptions.DefaultLanguage;
	}

	public bool DoesDefaultLanguageExist()
	{
	}

	public bool DoesOldDefaultExist()
	{
	}

	public async Task SaveTranslationsAsync(TranslationTree translations)
	{
	}

	public async Task<Dictionary<string, string>> GetOldDefaultAsync()
	{
	}

	public async Task SaveOldDefaultAsync()
	{
	}

	private bool CreateOrCheckTheTemporaryFolderAsync()
	{
		if(!Directory.Exists(afrowavePath))
		{
			try
			{
				Directory.CreateDirectory(afrowavePath);
				_logger.LogInformation("Temp folder created");
				return true;
			}
			catch(Exception ex)
			{
				_logger.LogError("Couldn't create temporary folder due to {error}", ex);
				return false;
			}
		}
		return true;
	}
}
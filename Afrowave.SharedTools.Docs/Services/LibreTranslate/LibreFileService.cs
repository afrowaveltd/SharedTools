namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

/// <summary>
/// Provides functionality for managing translation files and language data.
/// </summary>
/// <remarks>The <see cref="LibreFileService"/> class interacts with translation files stored locally and provides
/// methods to retrieve, save, and manage translations. It also integrates with the <see cref="ILibreTranslateService"/>
/// to fetch available languages and supports operations related to default and old translations.</remarks>
/// <param name="logger"></param>
/// <param name="configuration"></param>
/// <param name="translateService"></param>
public class LibreFileService(ILogger<LibreFileService> logger, IConfiguration configuration, ILibreTranslateService translateService)
{
	private readonly ILogger<LibreFileService> _logger = logger;
	private readonly ILibreTranslateService _translateService = translateService;
	private readonly TranslationsOptions _translationsOptions = configuration.GetSection("TranslationsOptions").Get<TranslationsOptions>() ?? new();
	private readonly string afrowavePath = Path.Combine(Path.GetTempPath(), "afrowave");
	private readonly string oldTranslation = Path.Combine(Path.GetTempPath(), "afrowave", "old.json");
	private readonly string localesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0], "Locales");

	/// <summary>
	/// Asynchronously retrieves all available translations for supported languages.
	/// </summary>
	/// <remarks>This method fetches the list of supported languages from the translation service and attempts to
	/// load the corresponding translation files from the local file system. If a translation file is missing or corrupted,
	/// an empty translation dictionary is created for the affected language. Corrupted files are backed up when
	/// possible.</remarks>
	/// <returns>A <see cref="TranslationTree"/> object containing the translations for each supported language. If no languages are
	/// available or an error occurs, the returned <see cref="TranslationTree"/> will contain empty translation
	/// dictionaries.</returns>
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

	/// <summary>
	/// Asynchronously retrieves the default language translations as a dictionary of key-value pairs.
	/// </summary>
	/// <remarks>This method attempts to load the default language file specified in the application's
	/// configuration. If the file does not exist, is empty, or cannot be read, an empty dictionary is returned.</remarks>
	/// <returns>A dictionary containing the translations for the default language, where the keys are translation identifiers and
	/// the values are the corresponding translated strings. Returns an empty dictionary if the file is missing,
	/// unreadable, or invalid.</returns>
	public async Task<Dictionary<string, string>> GetDefaultLanguageAsync()
	{
		string defaultLanguage = _translationsOptions.DefaultLanguage ?? "en";
		var filepath = Path.Combine(localesPath, defaultLanguage + ".json");
		if(!File.Exists(filepath))
		{
			return new();
		}
		try
		{
			string json = await File.ReadAllTextAsync(filepath);
			if(json == null)
				return new();
			return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
		}
		catch
		{
			return new();
		}
	}

	/// <summary>
	/// Determines whether the default language file exists in the specified locales path.
	/// </summary>
	/// <remarks>The default language is determined by the <c>DefaultLanguage</c> property of the translations
	/// options. If <c>DefaultLanguage</c> is null, the default language is assumed to be "en". The method checks for the
	/// existence of a JSON file named after the default language in the configured locales path.</remarks>
	/// <returns><see langword="true"/> if the default language file exists; otherwise, <see langword="false"/>.</returns>
	public bool DoesDefaultLanguageExist()
	{
		string defaultLanguage = _translationsOptions.DefaultLanguage ?? "en";
		var filepath = Path.Combine(localesPath, defaultLanguage + ".json");
		return File.Exists(filepath);
	}

	/// <summary>
	/// Determines whether the old default translation file exists.
	/// </summary>
	/// <returns><see langword="true"/> if the old default translation file exists; otherwise, <see langword="false"/>.</returns>
	public bool DoesOldDefaultExist()
	{
		return File.Exists(oldTranslation);
	}

	/// <summary>
	/// Saves the provided translations asynchronously.
	/// </summary>
	/// <param name="translations">The translation tree containing the translations to be saved.  Cannot be null.</param>
	/// <returns>A task that represents the asynchronous save operation.</returns>
	public async Task SaveTranslationsAsync(TranslationTree translations)
	{
	}

	/// <summary>
	/// Asynchronously retrieves a dictionary of old default translations from a JSON file.
	/// </summary>
	/// <remarks>If the specified file does not exist, is empty, or cannot be deserialized, an empty dictionary is
	/// returned. This method does not throw exceptions; any errors during file reading or deserialization are handled
	/// internally.</remarks>
	/// <returns>A <see cref="Dictionary{TKey, TValue}"/> containing the old default translations,  where the keys are strings
	/// representing translation identifiers and the values are their corresponding translations. Returns an empty
	/// dictionary if the file is missing, invalid, or deserialization fails.</returns>
	public async Task<Dictionary<string, string>> GetOldDefaultAsync()
	{
		if(!File.Exists(oldTranslation))
			return new();
		try
		{
			string json = await File.ReadAllTextAsync(oldTranslation);
			if(json == null)
				return new();
			return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
		}
		catch
		{
			return new();
		}
	}

	/// <summary>
	/// Saves the current default translation file as the old default translation.
	/// </summary>
	/// <remarks>This method copies the default translation file, determined by the application's configuration,  to
	/// a designated location for storing the old default translation. If a previous old default  translation file exists,
	/// it is deleted before the new file is saved.</remarks>
	/// <returns></returns>
	public void SaveOldDefaultAsync()
	{
		string defaultLanguage = _translationsOptions.DefaultLanguage ?? "en";
		var filepath = Path.Combine(localesPath, defaultLanguage + ".json");
		if(File.Exists(oldTranslation))
		{
			File.Delete(oldTranslation);
		}
		try
		{
			File.Copy(filepath, oldTranslation);
		}
		catch(Exception ex)
		{
			_logger.LogError("Exception occured {ex}", ex);
		}
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
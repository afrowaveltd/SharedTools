namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

/// <summary>
/// Provides methods for managing and retrieving translation data, including default and legacy language translations.
/// </summary>
/// <remarks>This service is designed to handle operations related to translation management, such as checking for
/// the existence of default languages, retrieving translations by language code, and saving updated translation data.
/// It supports asynchronous operations for efficient handling of translation-related tasks.</remarks>
public interface ILibreFileService
{
	/// <summary>
	/// Determines whether the default language is available in the system.
	/// </summary>
	/// <returns><see langword="true"/> if the default language exists; otherwise, <see langword="false"/>.</returns>
	bool DoesDefaultLanguageExist();

	/// <summary>
	/// Determines whether the old default configuration exists.
	/// </summary>
	/// <returns><see langword="true"/> if the old default configuration is present; otherwise, <see langword="false"/>.</returns>
	bool DoesOldDefaultExist();

	/// <summary>
	/// Asynchronously retrieves all translations organized in a hierarchical structure.
	/// </summary>
	/// <remarks>This method is useful for scenarios where translations need to be accessed in bulk and organized
	/// hierarchically, such as for localization or internationalization purposes.</remarks>
	/// <returns>A <see cref="TranslationTree"/> object representing the complete set of translations. The tree structure allows
	/// traversal of translations by their relationships.</returns>
	Task<TranslationTree> GetAllTranslationsAsync();

	/// <summary>
	/// Asynchronously retrieves the default language settings for the application.
	/// </summary>
	/// <remarks>The returned dictionary provides a mapping of supported language codes to their display names. This
	/// method is typically used to initialize language selection options or to display default language information in
	/// user interfaces.</remarks>
	/// <returns>A task that represents the asynchronous operation. The task result contains a dictionary where the keys are
	/// language codes (e.g., "en", "fr") and the values are the corresponding language names (e.g., "English", "French").</returns>
	Task<Dictionary<string, string>> GetDefaultLanguageAsync();

	/// <summary>
	/// Retrieves a dictionary containing language information based on the specified language code.
	/// </summary>
	/// <remarks>This method performs an asynchronous lookup to retrieve language details. Ensure the provided
	/// language code adheres to the expected format to avoid errors.</remarks>
	/// <param name="code">The language code to look up. This should be a valid ISO 639-1 or ISO 639-2 code.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains a dictionary where the keys are
	/// descriptive labels (e.g., "Name", "NativeName") and the values are the corresponding language details. If the
	/// language code is invalid or not found, the dictionary may be empty.</returns>
	Task<Dictionary<string, string>> GetLanguageByCode(string code);

	/// <summary>
	/// Asynchronously retrieves a dictionary containing default configuration values.
	/// </summary>
	/// <returns>A task that represents the asynchronous operation. The task result contains a dictionary where the keys are
	/// configuration names and the values are their corresponding default values. If no defaults are available, the
	/// dictionary will be empty.</returns>
	Task<Dictionary<string, string>> GetOldDefaultAsync();

	/// <summary>
	/// Saves the current default configuration asynchronously before it is replaced or updated.
	/// </summary>
	/// <remarks>This method is typically used to preserve the existing default configuration for backup or archival
	/// purposes. Ensure that the necessary permissions are in place to perform the save operation.</remarks>
	void SaveOldDefaultAsync();

	/// <summary>
	/// Saves the provided translation tree asynchronously.
	/// </summary>
	/// <param name="translations">The translation tree to be saved. This parameter must not be null and should contain the translations to persist.</param>
	/// <returns>A task that represents the asynchronous save operation.</returns>
	Task SaveTranslationsAsync(TranslationTree translations);
}
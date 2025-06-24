namespace Afrowave.SharedTools.Docs.Models.Enums;

public enum WorkerStatus
{
	/// <summary>
	/// Worker is idle and waiting for next cycle.
	/// </summary>
	Iddle = 0,

	/// <summary>
	/// Worker performs server checks, such as checking for updates or verifying the status of the server.
	/// </summary>
	Checks = 1,

	/// <summary>
	/// Loading JSON dictionaries from the backend.
	/// </summary>
	JsonBackendDataLoading = 2,

	/// <summary>
	/// Represents an option to check the availability of language names translation.
	/// </summary>
	/// <remarks>This enumeration value is typically used to specify a behavior or mode in operations that involve
	/// validating or processing language names.</remarks>
	CheckLanguageNames = 3,

	/// <summary>
	/// Checking if old translation exists and loading it if available.
	/// </summary>
	OldDictionaryLoading = 4,

	/// <summary>
	/// Check data in each file and generate requests for translations
	/// </summary>
	GenerateTranslationRequest = 5,

	/// <summary>
	/// Translating JSON dictionaries
	/// </summary>
	Translate = 6,

	/// <summary>
	/// Store new translations in the backend.
	/// </summary>
	SaveTranslation = 7,

	/// <summary>
	/// Search for folders with Markdown files and check if they are ready for translation.
	/// </summary>
	MdFoldersChecks = 8,

	/// <summary>
	/// Represents the Translate Markdown operation in the enumeration.
	/// </summary>
	/// <remarks>This value is used to specify the operation for translating Markdown content.</remarks>
	TranslateMd = 9,

	/// <summary>
	///	 Stores the translated content and records changes to the database
	/// </summary>
	SaveMd = 10,
}
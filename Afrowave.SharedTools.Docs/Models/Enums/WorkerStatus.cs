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
	/// Checking if old translation exists and loading it if available.
	/// </summary>
	OldDictionaryLoading = 3,

	/// <summary>
	/// Check data in each file and generate requests for translations
	/// </summary>
	GenerateTranslationRequest = 4,

	/// <summary>
	/// Translating JSON dictionaries
	/// </summary>
	Translate = 5,

	/// <summary>
	/// Store new translations in the backend.
	/// </summary>
	SaveTranslation = 6,

	/// <summary>
	/// Search for folders with Markdown files and check if they are ready for translation.
	/// </summary>
	MdFoldersChecks = 7,

	/// <summary>
	/// Represents the Translate Markdown operation in the enumeration.
	/// </summary>
	/// <remarks>This value is used to specify the operation for translating Markdown content.</remarks>
	TranslateMd = 8,

	/// <summary>
	///	 Stores the translated content and records changes to the database
	/// </summary>
	SaveMd = 9,
}
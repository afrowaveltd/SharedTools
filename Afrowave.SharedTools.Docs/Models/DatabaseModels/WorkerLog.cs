namespace Afrowave.SharedTools.Docs.Models.DatabaseModels;

/// <summary>
/// Represents a log of worker operations, including timing, status, and processing statistics for various tasks such as
/// JSON and Markdown processing.
/// </summary>
/// <remarks>This class is designed to track the performance and outcomes of worker tasks. It includes properties
/// for start and end times, durations, success indicators, and counts of processed items, errors, and updates. The log
/// is divided into general, JSON-specific, and Markdown-specific sections to provide detailed insights into each type
/// of operation.</remarks>
public class WorkerLog
{
	/// <summary>
	/// Gets or sets the unique identifier for the entity.
	/// </summary>
	[Key]
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the start date and time of the event or operation.
	/// </summary>
	public DateTime Start { get; set; } = DateTime.Now;

	/// <summary>
	/// Gets or sets the end date and time for the specified period or event.
	/// </summary>
	public DateTime End { get; set; } = DateTime.Now;

	/// <summary>
	/// Gets or sets the duration of a single cycle.
	/// </summary>
	public TimeSpan CycleTime { get; set; } = TimeSpan.FromSeconds(0);

	/// <summary>
	/// Gets or sets a value indicating whether the LibreTranslate service is operational.
	/// </summary>
	public bool LibreTranslateOk { get; set; } = true;

	// JSON related
	/// <summary>
	/// Gets or sets the start date and time for JSON-related operations.
	/// </summary>
	public DateTime StartJson { get; set; } = DateTime.Now;

	/// <summary>
	/// Gets or sets the end date and time for the JSON operation.
	/// </summary>
	public DateTime EndJson { get; set; } = DateTime.Now;

	/// <summary>
	/// Gets or sets the time duration represented in JSON format.
	/// </summary>
	public TimeSpan JsonTime { get; set; } = TimeSpan.FromSeconds(0);

	/// <summary>
	/// Gets or sets a value indicating whether the default value was found.
	/// </summary>
	public bool DefaultFound { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether an old translation was found.
	/// </summary>
	public bool OldTranslationFound { get; set; } = false;

	/// <summary>
	/// Gets or sets the total number of phrases.
	/// </summary>
	public int PhrazesCount { get; set; } = 0;

	/// <summary>
	/// Gets or sets the number of translated items.
	/// </summary>
	public int Translated { get; set; } = 0;

	/// <summary>
	/// Gets or sets the number of times the entity has been updated.
	/// </summary>
	public int Updated { get; set; } = 0;

	/// <summary>
	/// Gets or sets the number of items that have been removed.
	/// </summary>
	public int Removed { get; set; } = 0;

	/// <summary>
	/// Gets or sets the number of errors encountered during the operation.
	/// </summary>
	public int Errors { get; set; } = 0;

	// MD related
	/// <summary>
	/// Gets or sets the start date and time for the MD-related operation.
	/// </summary>
	public DateTime StartMd { get; set; } = DateTime.Now;

	/// <summary>
	/// Gets or sets the end date and time for the operation or event.
	/// </summary>
	public DateTime EndMd { get; set; } = DateTime.Now;

	/// <summary>
	/// Gets or sets the duration of the metadata processing time.
	/// </summary>
	public TimeSpan MdTime { get; set; } = TimeSpan.FromSeconds(0);

	/// <summary>
	/// Gets or sets the number of metadata folders.
	/// </summary>
	public int MDFoldersCount { get; set; } = 0;

	/// <summary>
	/// Gets or sets the number of files that have been successfully translated.
	/// </summary>
	public int TranslatedFilesCount { get; set; } = 0;

	/// <summary>
	/// Gets or sets the number of errors encountered while translating files.
	/// </summary>
	public int ErrorsTranslatingFilesCount { get; set; } = 0;
}
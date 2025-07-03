namespace Afrowave.SharedTools.Docs.Models.Enums;

/// <summary>
/// Status of the translation for a phrase
/// </summary>
public enum TranslationRequestStatus
{
	/// <summary>
	/// Represents the state of an operation or process that is currently waiting to proceed.
	/// </summary>
	/// <remarks>This state can be used to indicate that an operation is paused, delayed, or awaiting a specific
	/// condition before continuing. It is typically used in scenarios involving asynchronous workflows or state
	/// machines.</remarks>
	Waiting,

	/// <summary>
	/// Represents a service that provides translation functionality for converting text from one language to another.
	/// </summary>
	/// <remarks>This class is designed to handle text translation between supported languages.  It may include
	/// methods for specifying source and target languages, as well as options for translation quality or format.</remarks>
	Translating,

	/// <summary>
	/// Represents the success status of an operation.
	/// </summary>
	Success,

	/// <summary>
	/// Represents an error that occurred during application execution.
	/// </summary>
	/// <remarks>This class can be used to encapsulate details about an error, such as its type, message, or any
	/// additional context. It is intended to provide a standardized way to handle and propagate error
	/// information.</remarks>
	Error
}
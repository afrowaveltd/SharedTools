using Action = Afrowave.SharedTools.Docs.Models.Enums.Action;

namespace Afrowave.SharedTools.Docs.Models.Dto;

/// <summary>
/// Represents a request for translating a JSON resource into a specified language.
/// </summary>
/// <remarks>This class encapsulates the details of a translation request, including the resource key,  target
/// language, action to be performed, current status, and the time taken for translation.</remarks>
public class JsonTranslationRequest
{
	/// <summary>
	/// Gets or sets the unique identifier associated with this instance.
	/// It should be the plain text in the default language as set in appsettings.json
	/// In default language dictionary all Key, Value pairs should be equal.
	/// </summary>
	public string Key { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the language code associated with the current context.
	/// Use two characters ISO language code
	/// </summary>
	public string? LanguageCode { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the action to be executed.
	/// </summary>
	public Action Action { get; set; }

	/// <summary>
	/// Gets or sets the current status of the translation request.
	/// </summary>
	public TranslationRequestStatus Status { get; set; }

	/// <summary>
	/// Gets or sets the time taken to perform the translation operation.
	/// </summary>
	public TimeSpan TranslationTime { get; set; } = TimeSpan.Zero;
}
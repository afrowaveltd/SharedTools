namespace Afrowave.SharedTools.Docs.Models.SignalR;

/// <summary>
/// Represents the translation status for a specific language in a JSON dictionary.
/// </summary>
public class JsonDictionaryTranslationLanguageStatus
{
    /// <summary>
    /// Gets or sets the language code (e.g., 'en', 'cs').
    /// </summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this language is the default language.
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether this language is ignored for translation.
    /// </summary>
    public bool IsIgnored { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the translation for this language is complete.
    /// </summary>
    public bool IsDone { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of existing phrases for this language.
    /// </summary>
    public int ExistingPhrases { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of phrases to add for this language.
    /// </summary>
    public int ToAddPhrases { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of phrases to update for this language.
    /// </summary>
    public int ToUpdatePhrases { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of phrases to delete for this language.
    /// </summary>
    public int ToDeletePhrases { get; set; } = 0;
}
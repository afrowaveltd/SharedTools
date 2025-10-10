using System;
using System.Collections.Generic;
using System.Text;

namespace Afrowave.SharedTools.I18N.EventHandler
{
	/// <summary>
	/// Event arguments for dictionary change events.
	/// </summary>
	public class DictionaryChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the name of the event that is raised when the dictionary changes.
		/// </summary>
		public string EventName => "DictionaryChanged";

		/// <summary>
		/// Gets the language code associated with the content or resource.
		/// </summary>
		/// <remarks>The language code typically follows the ISO 639-1 standard (e.g., "en" for English, "fr"
		/// for French). This property is read-only.</remarks>
		public string LanguageCode { get; }

		/// <summary>
		/// Gets the date and time when the last change occurred.
		/// </summary>
		public DateTime ChangeTime { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryChangedEventArgs"/> class with the specified language code.
		/// </summary>
		/// <param name="languageCode">The language code associated with the content or resource.
		/// <param name="languageCode"></param>
		public DictionaryChangedEventArgs(string languageCode)
		{
			LanguageCode = languageCode;
			ChangeTime = DateTime.UtcNow;
		}
	}
}
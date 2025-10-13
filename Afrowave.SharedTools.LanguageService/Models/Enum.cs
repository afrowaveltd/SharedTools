using System;
using System.Collections.Generic;
using System.Text;

namespace Afrowave.SharedTools.I18N.Models
{
	/// <summary>
	/// Specifies available translation service providers.
	/// </summary>
	public enum TranslationProvider
	{
		/// <summary>
		/// Specifies that no options are set.
		/// </summary>
		None = 0,

		/// <summary>
		/// Google Translate service.
		/// </summary>
		GoogleTranslate = 1,

		/// <summary>
		/// LibreTranslate open-source service.
		/// </summary>
		LibreTranslate = 2,

		/// <summary>
		/// Microsoft Translator service.
		/// </summary>
		MicrosoftTranslator = 3,

		/// <summary>
		/// Deep learning-based custom translation.
		/// </summary>
		DeepLearning = 4,

		/// <summary>
		/// Amazon Translate service.
		/// </summary>
		AmazonTranslate = 5,

		/// <summary>
		/// Custom or user-defined provider.
		/// </summary>
		CustomProvider = 6
	}

	/// <summary>
	/// Specifies supported dictionary or resource file formats for localization.
	/// </summary>
	public enum DictionaryFormat
	{
		/// <summary>
		/// Flat JSON format (key-value pairs).
		/// </summary>
		JSON_FLAT = 1,

		/// <summary>
		/// Nested JSON format (hierarchical structure).
		/// </summary>
		JSON_NESTED = 2,

		/// <summary>
		/// XML format.
		/// </summary>
		XML = 3,

		/// <summary>
		/// CSV (Comma-separated values) format.
		/// </summary>
		CSV = 4,

		/// <summary>
		/// YAML format.
		/// </summary>
		YAML = 5,

		/// <summary>
		/// .NET RESX resource format.
		/// </summary>
		RESX = 6,

		/// <summary>
		/// Portable Object (.po) format.
		/// </summary>
		PO = 7,

		/// <summary>
		/// Machine Object (.mo) format.
		/// </summary>
		MO = 8,

		/// <summary>
		/// SQL database format.
		/// </summary>
		SQL = 9,

		/// <summary>
		/// Mongo database format.
		/// </summary>
		MongoDB = 10,

		/// <summary>
		/// Custom or user-defined format.
		/// </summary>
		CustomFormat = 100
	}
}
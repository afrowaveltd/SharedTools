using System;

namespace Afrowave.SharedTools.Localization.Common.Models.Enums
{
	/// <summary>
	/// Specifies the distinct roles/plugins that can be combined to build the localization pipeline.
	/// </summary>
	/// <remarks>
	/// This enumeration supports bitwise combination of its values using the <see langword="|"/> operator.
	/// Plugins may implement multiple roles (e.g. <see cref="Backend"/> | <see cref="Cache"/>).
	/// </remarks>
	[Flags]
	public enum PluginType
	{
		/// <summary>
		/// Represents a data accessor: provides low-level access to localization resources
		/// (filesystem, HTTP, FTP, SQL, etc.), but does not process their content.
		/// </summary>
		DataAccessor = 1,

		/// <summary>
		/// Represents a backend plugin that implements localization/business logic
		/// (parsing, saving, validating dictionaries, handling structured/flat models).
		/// </summary>
		Backend = 2,

		/// <summary>
		/// Represents a translator plugin that performs machine or human translation
		/// between languages (e.g. LibreTranslate, DeepL, Google Translate).
		/// </summary>
		Translator = 4,

		/// <summary>
		/// Represents a frontend plugin (UI, middleware, or bridge for presenting/consuming localized content).
		/// </summary>
		Frontend = 8,

		/// <summary>
		/// Represents a utility or tool plugin (e.g. migration, validation, batch editing).
		/// </summary>
		Tool = 16,

		/// <summary>
		/// Represents a cache plugin that provides temporary storage and fast access
		/// to frequently used data.
		/// </summary>
		Cache = 32,

		/// <summary>
		/// Represents a plugin for diagnostics, monitoring, or health checking of the system.
		/// </summary>
		Diagnostics = 64,

		/// <summary>
		/// Represents a plugin dedicated to logging events, errors, and system activity.
		/// </summary>
		Logger = 128
	}
}
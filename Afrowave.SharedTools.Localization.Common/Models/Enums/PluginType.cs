using System;

namespace Afrowave.SharedTools.Localization.Common.Models.Enums
{
	/// <summary>
	/// Specifies the types of plugins that can be used within the application.
	/// </summary>
	/// <remarks>This enumeration supports bitwise combination of its values using the <see langword="|"/> operator,
	/// as it is marked with the <see cref="FlagsAttribute"/>. For example, a plugin can be both  <see cref="Backend"/> and
	/// <see cref="Cache"/>.</remarks>
	[Flags]
	public enum PluginType
	{
		/// <summary>
		/// Represents the absence of a specific value or state.
		/// </summary>
		None = 0,

		/// <summary>
		/// Represents a backend plugin used for processing and management tasks.
		/// </summary>
		Backend = 1,

		/// <summary>
		/// Represents the role of a translator in the system.
		/// </summary>
		/// <remarks>This enumeration value is used to identify users or entities responsible for translating
		/// content.</remarks>
		Translator = 2,

		/// <summary>
		/// Represents the frontend layer in a multi-tier application architecture.
		/// </summary>
		Frontend = 4,

		/// <summary>
		/// Represents a tool category in the application.
		/// </summary>
		Tool = 8,

		/// <summary>
		/// Represents a caching behavior or option in the system.
		/// </summary>
		/// <remarks>This value is typically used to indicate that caching should be applied to improve performance by
		/// storing frequently accessed data temporarily. The specific caching mechanism depends on the
		/// implementation.</remarks>
		Cache = 16,

		/// <summary>
		/// Represents diagnostic logging level, used to capture detailed information for debugging purposes.
		/// </summary>
		Diagnostics = 32,

		/// <summary>
		/// Represents a logging flag with a value of 64.
		/// </summary>
		/// <remarks>This flag is typically used to enable or identify logging functionality within the
		/// application.</remarks>
		Logger = 64
	}
}
namespace Afrowave.SharedTools.Models.Enums
{
	/// <summary>
	/// Enum for newline conversion options.
	/// </summary>
	public enum TextNewLineStyle
	{
		/// <summary>
		/// Represents Windows-style newlines (CRLF).
		/// </summary>
		/// <remarks>This class or member is intended for use in Windows environments.  Ensure compatibility with
		/// the target operating system before using this functionality.</remarks>
		Windows,

		/// <summary>
		/// Represents a Unix-specific implementation or functionality.
		/// </summary>
		/// <remarks>This class or member is intended for use in Unix-based environments.  Ensure compatibility with
		/// the target operating system before using this functionality.</remarks>
		Unix,

		/// <summary>
		/// Represents a Mac computer with properties and methods specific to its functionality.
		/// </summary>
		/// <remarks>This class or member is intended for use in Unix-based environments.  Ensure compatibility with
		/// the target operating system before using this functionality.</remarks>
		Mac
	}
}
namespace Afrowave.SharedTools.Localization.Common.Models.Enums
{
	/// <summary>
	/// Represents the severity of a log message.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Represents the trace level of logging, used for detailed diagnostic information.
		/// </summary>
		Trace = 0,

		/// <summary>
		/// Represents the debug logging level, typically used for diagnostic information.
		/// </summary>
		Debug = 1,

		/// <summary>
		/// Represents informational messages that highlight the progress of the application at a coarse-grained level.
		/// </summary>
		Info = 2,

		/// <summary>
		/// Represents a warning severity level in a logging or diagnostic system.
		/// </summary>
		Warning = 3,

		/// <summary>
		/// Represents an error state or condition.
		/// </summary>
		/// <remarks>This enumeration value is typically used to indicate that an operation has failed or encountered
		/// an error.</remarks>
		Error = 4,

		/// <summary>
		/// Represents a critical log level, typically used for logging severe errors or issues that require immediate
		/// attention.
		/// </summary>
		Critical = 5
	}
}
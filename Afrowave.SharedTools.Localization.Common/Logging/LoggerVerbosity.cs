namespace Afrowave.SharedTools.Localization.Common.Logging
{
	/// <summary>
	/// Defines the verbosity level for plugin or system logging.
	/// </summary>
	public enum LoggerVerbosity
	{
		/// <summary>
		/// Only fatal errors are logged.
		/// </summary>
		Fatal = 0,

		/// <summary>
		/// Errors and critical failures are logged.
		/// </summary>
		Error = 1,

		/// <summary>
		/// Warnings are included alongside errors.
		/// </summary>
		Warning = 2,

		/// <summary>
		/// Informational messages, warnings, and errors.
		/// </summary>
		Info = 3,

		/// <summary>
		/// Full diagnostic log including internal steps.
		/// </summary>
		Debug = 4,

		/// <summary>
		/// Ultra-verbose logging for tracing all activity.
		/// </summary>
		LoudSpeaker = 5
	}
}
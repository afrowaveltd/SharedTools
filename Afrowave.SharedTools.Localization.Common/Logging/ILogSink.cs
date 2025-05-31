namespace Afrowave.SharedTools.Localization.Common.Logging
{
	/// <summary>
	/// Represents a target that can receive and process log messages.
	/// </summary>
	public interface ILogSink
	{
		/// <summary>
		/// Logs the specified event to the configured logging system.
		/// </summary>
		/// <remarks>The behavior of this method depends on the logging configuration.  Ensure that the <paramref
		/// name="entry"/> contains all necessary information,  such as the log level, message, and any associated
		/// metadata.</remarks>
		/// <param name="entry">The log event to record. Must not be <see langword="null"/>.</param>
		void Log(LogEvent entry);
	}
}
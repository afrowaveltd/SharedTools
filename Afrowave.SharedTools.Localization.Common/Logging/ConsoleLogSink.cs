using System;

namespace Afrowave.SharedTools.Localization.Common.Logging
{
	/// <summary>
	/// Simple log sink that writes to the console – useful for development.
	/// </summary>
	public sealed class ConsoleLogSink : ILogSink
	{
		/// <summary>
		/// Logs the details of a <see cref="LogEvent"/> to the console.
		/// </summary>
		/// <remarks>This method outputs the log event information in a structured format, including optional
		/// exception  and context details if provided. The timestamp is formatted using the ISO 8601 standard.</remarks>
		/// <param name="entry">The log event to be written. Must contain a valid timestamp, level, source, and message.</param>
		public void Log(LogEvent entry)
		{
			Console.WriteLine($"[{entry.Timestamp:O}] [{entry.Level}] {entry.Source} :: {entry.Message}");

			if(!string.IsNullOrEmpty(entry.Exception))
				Console.WriteLine($"  Exception: {entry.Exception}");

			if(entry.Context != null)
				Console.WriteLine($"  Context: {entry.Context}");
		}
	}
}
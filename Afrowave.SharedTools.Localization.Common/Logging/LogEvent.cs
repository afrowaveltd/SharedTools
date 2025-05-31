using Afrowave.SharedTools.Localization.Common.Models.Enums;
using System;

namespace Afrowave.SharedTools.Localization.Common.Logging
{
	/// <summary>
	/// Represents a structured log event emitted by a plugin.
	/// </summary>
	public sealed class LogEvent
	{
		/// <summary>
		/// Gets or sets the timestamp indicating when the associated event or data was created or last updated.
		/// </summary>
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Gets or sets the log level that indicates the severity of the log event.
		/// </summary>
		public LogLevel Level { get; set; } = LogLevel.Info;

		/// <summary>
		/// Gets or sets the source of the log event, indicating where the log originated.
		/// </summary>
		public string Source { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the message associated with the log event, providing additional context.
		/// </summary>
		public string Message { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the exception message associated with the current operation.
		/// </summary>
		public string Exception { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the context object associated with the current operation.
		/// </summary>
		public object? Context { get; set; }
	}
}
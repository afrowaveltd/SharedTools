using Afrowave.SharedTools.Localization.Common.Models.Enums;
using System;

namespace Afrowave.SharedTools.Localization.Common.Logging
{
	/// <summary>
	/// Represents a structured log event emitted by a plugin.
	/// </summary>
	public sealed class LogEvent
	{
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;

		public LogLevel Level { get; set; } = LogLevel.Info;

		public string Source { get; set; } = string.Empty;

		public string Message { get; set; } = string.Empty;

		public string Exception { get; set; }

		public object? Context { get; set; }
	}
}
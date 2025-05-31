using System;

namespace Afrowave.SharedTools.Localization.Common.Logging
{
	/// <summary>
	/// Simple log sink that writes to the console – useful for development.
	/// </summary>
	public sealed class ConsoleLogSink : ILogSink
	{
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
namespace Afrowave.SharedTools.Localization.Common.Logging
{
	/// <summary>
	/// Represents a target that can receive and process log messages.
	/// </summary>
	public interface ILogSink
	{
		void Log(LogEvent entry);
	}
}
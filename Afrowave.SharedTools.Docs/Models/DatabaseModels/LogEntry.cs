namespace Afrowave.SharedTools.Docs.Models.DatabaseModels
{
	/// <summary>
	/// Represents a single log entry containing details about a logged event.
	/// </summary>
	/// <remarks>A log entry typically includes information such as the event's unique identifier,  timestamp,
	/// severity level, message, and any associated exception or additional properties. This class can be used to store and
	/// process log data in logging frameworks or systems.</remarks>
	public class LogEntry
	{
		/// <summary>
		/// Gets or sets the unique identifier for the entity.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the timestamp representing the date and time of the associated event or operation.
		/// </summary>
		public DateTimeOffset Timestamp { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Gets or sets the logging level for the application.
		/// </summary>
		public string Level { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the message associated with the current operation or context.
		/// </summary>
		public string Message { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the exception message associated with the current operation.
		/// </summary>
		public string Exception { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the properties associated with the current object.
		/// </summary>
		public string Properties { get; set; } = string.Empty;
	}
}
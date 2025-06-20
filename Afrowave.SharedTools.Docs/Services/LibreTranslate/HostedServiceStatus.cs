namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

/// <summary>
/// Represents the status of a hosted service and provides methods to manage its state.
/// </summary>
public class HostedServiceStatus
{
	/// <summary>
	/// Clears all items from the collection, resetting it to an empty state.
	/// </summary>
	/// <remarks>This method removes all elements from the collection. After calling this method,  the collection
	/// will contain no items and its count will be zero.</remarks>

	public static WorkerStatus Status { get; set; } = WorkerStatus.Iddle;
	public static DateTime CycleStart { get; set; } = DateTime.Now;
	public static List<Language> LibreLanguages { get; set; } = new List<Language>();

	/// <summary>
	/// Resets the worker's status to its default idle state.
	/// </summary>
	/// <remarks>This method sets the <see cref="Status"/> property to <see cref="WorkerStatus.Iddle"/>. It can be
	/// used to clear the current state of the worker and prepare it for a new operation.</remarks>
	public static void Clear()
	{
		Status = WorkerStatus.Checks;
		CycleStart = DateTime.Now;
		LibreLanguages.Clear();
	}
}
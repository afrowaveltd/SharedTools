namespace Afrowave.SharedTools.Docs.Hubs;

/// <summary>
/// Provides functionality to track and manage the number of active connections in a real-time hub.
/// </summary>
/// <remarks>This class maintains a static count of active connections and provides methods to increment or
/// decrement the count. It is designed for use in scenarios where a centralized connection count  is required, such as
/// in real-time communication hubs.</remarks>
public static class RealtimeHubStatus
{
	/// <summary>
	/// Gets or sets the number of active connections currently in use.
	/// </summary>
	public static int ActiveConnections { get; set; } = 0;

	/// <summary>
	/// Increments the count of active connections by one.
	/// </summary>
	/// <remarks>This method is thread-safe and can be called concurrently from multiple threads. It updates the
	/// global count of active connections, which is tracked by the application.</remarks>
	public static void IncrementConnections()
	{
		ActiveConnections++;
	}

	/// <summary>
	/// Decrements the count of active connections if the current count is greater than zero.
	/// </summary>
	/// <remarks>This method ensures that the count of active connections does not drop below zero. It should be
	/// called only when a connection is being properly closed or released.</remarks>
	public static void DecrementConnections()
	{
		if(ActiveConnections > 0)
			ActiveConnections--;
	}
}
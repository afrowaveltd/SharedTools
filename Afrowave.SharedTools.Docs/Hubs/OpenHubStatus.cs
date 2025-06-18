namespace Afrowave.SharedTools.Docs.Hubs
{
	/// <summary>
	/// Provides functionality to track and manage the number of active connections in the system.
	/// </summary>
	/// <remarks>This class maintains a static count of active connections and provides methods to increment or
	/// decrement the count. It is designed for scenarios where a centralized connection tracking mechanism  is required.
	/// The <see cref="ActiveConnections"/> property reflects the current number of active  connections at any given
	/// time.</remarks>
	public static class OpenHubStatus
	{
		/// <summary>
		/// Gets or sets the number of active connections currently in use.
		/// </summary>
		public static int ActiveConnections { get; set; } = 0;

		/// <summary>
		/// Increments the count of active connections by one.
		/// </summary>
		/// <remarks>This method is thread-safe and updates the global count of active connections. It should be
		/// called whenever a new connection is established to ensure the count remains accurate.</remarks>
		public static void IncrementConnections()
		{
			ActiveConnections++;
		}

		/// <summary>
		/// Decrements the count of active connections if the current count is greater than zero.
		/// </summary>
		/// <remarks>This method ensures that the count of active connections does not drop below zero. It should be
		/// called only when an active connection is being closed or terminated.</remarks>
		public static void DecrementConnections()
		{
			if(ActiveConnections > 0)
				ActiveConnections--;
		}
	}
}
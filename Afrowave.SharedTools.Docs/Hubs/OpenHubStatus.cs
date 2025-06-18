namespace Afrowave.SharedTools.Docs.Hubs
{
	/// <summary>
	/// Provides functionality to track and manage the number of active connections in the system.
	/// </summary>
	/// <remarks>This class maintains a static count of active connections, allowing for incrementing and
	/// decrementing the count as connections are established or closed. It is thread-unsafe, so external synchronization
	/// is required if accessed concurrently.</remarks>
	public static class OpenHubStatus
	{
		/// <summary>
		/// Gets or sets the number of active connections currently established.
		/// </summary>
		public static int ActiveConnections { get; set; } = 0;

		/// <summary>
		/// Increments the count of active connections.
		/// </summary>
		/// <remarks>This method increases the value of the active connections counter by one.  It is thread-safe if
		/// the <see cref="ActiveConnections"/> property is properly synchronized.</remarks>
		public static void IncrementConnections()
		{
			ActiveConnections++;
		}

		/// <summary>
		/// Decrements the count of active connections if the current count is greater than zero.
		/// </summary>
		/// <remarks>This method ensures that the count of active connections does not drop below zero. It should be
		/// called only when it is certain that a connection has been closed.</remarks>
		public static void DecrementConnections()
		{
			if(ActiveConnections > 0)
				ActiveConnections--;
		}
	}
}
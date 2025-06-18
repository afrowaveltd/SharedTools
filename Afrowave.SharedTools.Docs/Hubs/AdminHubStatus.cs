namespace Afrowave.SharedTools.Docs.Hubs
{
	/// <summary>
	/// Provides functionality to track and manage the number of active connections in the admin hub.
	/// </summary>
	/// <remarks>This class is designed to maintain a count of active connections in a thread-safe manner.  Use <see
	/// cref="IncrementConnections"/> to increase the count and <see cref="DecrementConnections"/>  to decrease it. The
	/// <see cref="ActiveConnections"/> property reflects the current number of active connections.</remarks>
	public static class AdminHubStatus
	{
		/// <summary>
		/// Gets or sets the number of active connections currently established.
		/// </summary>
		public static int ActiveConnections { get; set; } = 0;

		/// <summary>
		/// Increments the count of active connections.
		/// </summary>
		/// <remarks>This method increases the value of the active connections counter by one.  It is thread-safe if
		/// the <c>ActiveConnections</c> property is properly synchronized.</remarks>
		public static void IncrementConnections()
		{
			ActiveConnections++;
		}

		/// <summary>
		/// Decrements the count of active connections if the current count is greater than zero.
		/// </summary>
		/// <remarks>This method ensures that the count of active connections does not drop below zero. It should be
		/// called when a connection is properly closed or no longer active.</remarks>
		public static void DecrementConnections()
		{
			if(ActiveConnections > 0)
				ActiveConnections--;
		}
	}
}
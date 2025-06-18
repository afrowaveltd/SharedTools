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
		public static int ActiveConnections { get; set; } = 0;

		public static void IncrementConnections()
		{
			ActiveConnections++;
		}

		public static void DecrementConnections()
		{
			if(ActiveConnections > 0)
				ActiveConnections--;
		}
	}
}
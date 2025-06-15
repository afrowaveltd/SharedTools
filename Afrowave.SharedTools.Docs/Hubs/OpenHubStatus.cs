namespace Afrowave.SharedTools.Docs.Hubs
{
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
namespace Afrowave.SharedTools.Docs.Hubs
{
	public class AdminHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, "Authenticated users");
			AdminHubStatus.IncrementConnections();
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Authenticated users");
			AdminHubStatus.DecrementConnections();
			await base.OnDisconnectedAsync(exception);
		}
	}
}
namespace Afrowave.SharedTools.Docs.Hubs
{
	public class OpenHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
			OpenHubStatus.IncrementConnections();
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalRUsers");
			OpenHubStatus.DecrementConnections();
			await base.OnDisconnectedAsync(exception);
		}

		public async Task SendMessage(string message)
		{
			await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", message);
		}
	}
}
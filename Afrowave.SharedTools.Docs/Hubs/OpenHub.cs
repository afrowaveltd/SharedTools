namespace Afrowave.SharedTools.Docs.Hubs
{
	public class OpenHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalRUsers");
			await base.OnDisconnectedAsync(exception);
		}
	}
}
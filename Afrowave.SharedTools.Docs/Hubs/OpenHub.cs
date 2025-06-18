namespace Afrowave.SharedTools.Docs.Hubs
{
	/// <summary>
	/// Represents a SignalR hub that manages user connections and facilitates message broadcasting to a specific group of
	/// users.
	/// </summary>
	/// <remarks>This hub automatically adds connected users to the "SignalR Users" group upon connection and
	/// removes them from the group upon disconnection. It also provides functionality to send messages to all users in the
	/// group.</remarks>
	public class OpenHub : Hub
	{
		/// <summary>
		/// Handles the event when a client connects to the SignalR hub.
		/// </summary>
		/// <remarks>This method adds the connecting client to the "SignalR Users" group and increments the connection
		/// count in the <see cref="OpenHubStatus"/>. It then invokes the base implementation of <see
		/// cref="OnConnectedAsync"/>.</remarks>
		/// <returns></returns>
		public override async Task OnConnectedAsync()
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
			OpenHubStatus.IncrementConnections();
			await base.OnConnectedAsync();
		}

		/// <summary>
		/// Handles the event when a client disconnects from the SignalR hub.
		/// </summary>
		/// <remarks>This method removes the client from the "SignalRUsers" group and decrements the connection count.
		/// It then invokes the base implementation of <see cref="OnDisconnectedAsync(Exception?)"/>.</remarks>
		/// <param name="exception">The exception that caused the disconnection, if any; otherwise, <see langword="null"/>.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalRUsers");
			OpenHubStatus.DecrementConnections();
			await base.OnDisconnectedAsync(exception);
		}

		/// <summary>
		/// Sends a message to all clients in the "SignalR Users" group.
		/// </summary>
		/// <remarks>This method uses SignalR to broadcast the specified message to all clients subscribed to the
		/// "SignalR Users" group. Ensure that the group exists and clients are properly subscribed to receive the
		/// message.</remarks>
		/// <param name="message">The message to be sent to the clients. Cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		public async Task SendMessage(string message)
		{
			await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", message);
		}
	}
}
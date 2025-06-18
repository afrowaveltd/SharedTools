namespace Afrowave.SharedTools.Docs.Hubs;

/// <summary>
/// Represents a SignalR hub that manages real-time communication for clients in the "Realtime" group.
/// </summary>
/// <remarks>This hub automatically adds clients to the "Realtime" group upon connection and removes them upon
/// disconnection. It also provides functionality for broadcasting messages to all clients in the group.</remarks>
public class RealtimeHub : Hub
{
	/// <summary>
	/// Handles the event when a client successfully connects to the hub.
	/// </summary>
	/// <remarks>This method adds the connecting client to the "Realtime" group and increments the connection count
	/// in the <see cref="OpenHubStatus"/>. It then invokes the base implementation of <see
	/// cref="OnConnectedAsync"/>.</remarks>
	/// <returns></returns>
	public override async Task OnConnectedAsync()
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, "Realtime");
		OpenHubStatus.IncrementConnections();
		await base.OnConnectedAsync();
	}

	/// <summary>
	/// Handles the event when a client disconnects from the hub.
	/// </summary>
	/// <remarks>This method removes the client from the "Realtime" group and decrements the connection count. It
	/// then invokes the base implementation of <see cref="OnDisconnectedAsync"/>.</remarks>
	/// <param name="exception">The exception that caused the disconnection, if any; otherwise, <see langword="null"/>.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Realtime");
		OpenHubStatus.DecrementConnections();
		await base.OnDisconnectedAsync(exception);
	}

	/// <summary>
	/// Sends a message to all clients in the "Realtime" group.
	/// </summary>
	/// <remarks>This method broadcasts the specified message to all connected clients that are part of the
	/// "Realtime" group. Ensure that the message is properly formatted and non-empty before calling this method.</remarks>
	/// <param name="message">The message to be sent. Cannot be null or empty.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public async Task SendMessage(string message)
	{
		await Clients.Group("Realtime").SendAsync("ReceiveMessage", message);
	}
}
namespace Afrowave.SharedTools.Docs.Hubs
{
	/// <summary>
	/// Represents a SignalR hub for managing administrative connections and tracking connection status.
	/// </summary>
	/// <remarks>The <see cref="AdminHub"/> class handles client connections and disconnections, automatically
	/// adding authenticated users to a designated group and updating connection status metrics.</remarks>
	public class AdminHub : Hub
	{
		/// <summary>
		/// Handles the event when a client successfully connects to the hub.
		/// </summary>
		/// <remarks>Adds the connected client to the "Authenticated users" group and increments the connection count
		/// in the <see cref="AdminHubStatus"/>. This method also invokes the base implementation of <see
		/// cref="OnConnectedAsync"/>.</remarks>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
		public override async Task OnConnectedAsync()
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, "Authenticated users");
			AdminHubStatus.IncrementConnections();
			await base.OnConnectedAsync();
		}

		/// <summary>
		/// Handles the event when a client disconnects from the hub.
		/// </summary>
		/// <remarks>This method removes the client from the "Authenticated users" group and decrements the connection
		/// count in the <see cref="AdminHubStatus"/>. It then invokes the base implementation of <see
		/// cref="OnDisconnectedAsync"/>.</remarks>
		/// <param name="exception">The exception that caused the disconnection, if any; otherwise, <see langword="null"/>.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Authenticated users");
			AdminHubStatus.DecrementConnections();
			await base.OnDisconnectedAsync(exception);
		}
	}
}
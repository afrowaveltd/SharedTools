namespace Afrowave.SharedTools.Docs.Hubs
{
	/// <summary>
	/// Represents a SignalR hub that manages user connections and facilitates message broadcasting to a specific group of
	/// users.
	/// </summary>
	/// <remarks>This hub automatically adds connected users to the "SignalR Users" group upon connection and
	/// removes them from the group upon disconnection. It also provides functionality to send messages to all users in the
	/// group.</remarks>
	/// <param name="localizer"></param>
	public class OpenHub(IStringLocalizer<OpenHub> localizer) : Hub
	{
		private readonly IStringLocalizer<OpenHub> _localizer = localizer;

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

		/// <summary>
		/// Notifies all connected clients about a change in the worker's status.
		/// </summary>
		/// <remarks>This method maps the provided <paramref name="status"/> to a localized string representation and
		/// broadcasts it to all connected clients using the "StatusChanged" event.</remarks>
		/// <param name="status">The current status of the worker, represented as a <see cref="WorkerStatus"/> enumeration value.</param>
		/// <returns>A task that represents the asynchronous operation of notifying clients.</returns>
		public async Task WorkerStatusChanged(WorkerStatus status)
		{
			var actualStatus = string.Empty;
			switch(status)
			{
				case WorkerStatus.Iddle:
					actualStatus = _localizer["Iddle"];
					break;

				case WorkerStatus.Checks:
					actualStatus = _localizer["Checking files"];
					break;

				case WorkerStatus.JsonBackendDataLoading:
					actualStatus = _localizer["Loading data"];
					break;

				case WorkerStatus.OldDictionaryLoading:
					actualStatus = _localizer["Loading previous stage"];
					break;

				case WorkerStatus.GenerateTranslationRequest:
					actualStatus = _localizer["Generating Translation Request"];
					break;

				case WorkerStatus.Translate:
					actualStatus = _localizer["Translating localization data"];
					break;

				case WorkerStatus.SaveTranslation:
					actualStatus = _localizer["Saving new translations"];
					break;

				case WorkerStatus.MdFoldersChecks:
					actualStatus = _localizer["Searching for folders condaining MD files"];
					break;

				case WorkerStatus.TranslateMd:
					actualStatus = _localizer["Translating files"];
					break;

				case WorkerStatus.SaveMd:
					actualStatus = _localizer["Saving files"];
					break;
			}

			await Clients.All.SendAsync("StatusChanged", actualStatus);
		}
	}
}
namespace Afrowave.SharedTools.Docs.Hubs
{
	public class OpenHub : Hub
	{
		public async Task SendMessage(string user, string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}
	}
}
namespace Afrowave.SharedTools.Localization.Common.Options
{
	/// <summary>
	/// Represents configuration options for SignalR integration.
	/// </summary>
	/// <remarks>This class provides settings for connecting to a SignalR server, including the server URL,  access
	/// key, timeout duration, and whether to use compression. These options can be used  to customize the behavior of
	/// SignalR-based communication in a plugin.</remarks>
	public sealed class SignalROptions : PluginOptionSet
	{
		/// <summary>
		/// Gets the unique key associated with the SignalR configuration.
		/// </summary>
		public override string Key => "SignalR";

		/// <summary>
		/// Gets or sets the URL of the SignalR server.
		/// </summary>
		public string Url { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the access key used for authentication or authorization purposes.
		/// </summary>
		public string AccessKey { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the timeout duration, in seconds, for the operation.
		/// </summary>
		public int TimeoutSeconds { get; set; } = 10;

		/// <summary>
		/// Gets or sets a value indicating whether compression should be used for data transfer.
		/// </summary>
		public bool UseCompression { get; set; } = false;
	}
} // namespace Afrowave.SharedTools.Localization.Common.Options
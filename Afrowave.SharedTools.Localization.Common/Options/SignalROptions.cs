using Afrowave.SharedTools.Localization.Common.Options;

namespace Afrowave.SharedTools.Localization.Common.Handshake
{
	public sealed class SignalROptions : PluginOptionSet
	{
		public override string Key => "SignalR";

		public string Url { get; set; } = string.Empty;
		public string AccessKey { get; set; } = string.Empty;
		public int TimeoutSeconds { get; set; } = 10;
		public bool UseCompression { get; set; } = false;
	}
}
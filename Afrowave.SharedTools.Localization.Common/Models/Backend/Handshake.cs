namespace Afrowave.SharedTools.Localization.Common.Models.Backend
{
	public class Handshake
	{
		public string Id { get; set; } = string.Empty;
		public BackendManifest Manifest { get; set; } = new BackendManifest();
		public object Extra { get; set; } = new { };
	}
}
namespace Afrowave.SharedTools.Localization.Common.Models.Backend
{
	public class BackendManifest
	{
		public Metadata Metadata { get; set; } = new Metadata();
		public Capabilities Capabilities { get; set; } = new Capabilities();
		public Behavior Behavior { get; set; } = new Behavior();
		public object Extra { get; set; } = new object();
	}
}
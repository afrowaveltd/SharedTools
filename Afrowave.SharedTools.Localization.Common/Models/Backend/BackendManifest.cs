namespace Afrowave.SharedTools.Localization.Common.Models.Backend
{
	/// <summary>
	/// Represents the manifest information for a localization backend, including metadata, capabilities, behavior, and extra data.
	/// </summary>
	public class BackendManifest
	{
		/// <summary>
		/// Gets or sets the metadata information for the backend.
		/// </summary>
		public Metadata Metadata { get; set; } = new Metadata();

		/// <summary>
		/// Gets or sets the capabilities of the backend.
		/// </summary>
		public Capabilities Capabilities { get; set; } = new Capabilities();

		/// <summary>
		/// Gets or sets the behavior configuration of the backend.
		/// </summary>
		public Behavior Behavior { get; set; } = new Behavior();

		/// <summary>
		/// Gets or sets any extra data associated with the backend.
		/// </summary>
		public object Extra { get; set; } = new object();
	}
}
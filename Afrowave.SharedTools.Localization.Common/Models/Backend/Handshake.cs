using Afrowave.SharedTools.Localization.Common.Models.Enums;

namespace Afrowave.SharedTools.Localization.Common.Models.Backend
{
	/// <summary>
	/// Represents the handshake information for a localization backend, including an identifier, manifest, and extra data.
	/// </summary>
	public class Handshake
	{
		/// <summary>
		/// Gets or sets the unique identifier for the handshake.
		/// </summary>
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the manifest information for the backend.
		/// </summary>
		public BackendManifest Manifest { get; set; } = new BackendManifest();

		public PluginType PluginTypes { get; set; } = PluginType.None;

		/// <summary>
		/// Gets or sets any extra data associated with the handshake.
		/// </summary>
		public object Extra { get; set; } = new { };
	}
}
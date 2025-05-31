namespace Afrowave.SharedTools.Localization.Communication
{
	using Afrowave.SharedTools.Localization.Common.Models.Backend;
	using Afrowave.SharedTools.Localization.Common.Models.Enums;

	/// <summary>
	/// Represents a plugin and its declared capabilities, metadata, and runtime status.
	/// </summary>
	public sealed class PluginManifest
	{    /// <summary>
		  /// Metadata: name, version, license, author etc.
		  /// </summary>
		public Metadata Metadata { get; set; } = new Metadata();

		/// <summary>
		/// Plugin type(s) – determines the functional class of this plugin.
		/// </summary>
		public PluginType Type { get; set; } = PluginType.None;

		/// <summary>
		/// Declared capabilities supported by this plugin.
		/// </summary>
		public Capabilities Capabilities { get; set; } = new Capabilities();

		/// <summary>
		/// Optional behavior configuration (e.g. priorities, chaining).
		/// </summary>
		public Behavior Behavior { get; set; } = new Behavior();

		/// <summary>
		/// Runtime status – used for control and health tracking.
		/// </summary>
		public Status Status { get; set; } = Status.Ready;

		/// <summary>
		/// Optional extended data.
		/// </summary>
		public object Extra { get; set; } = new object();
	}
}
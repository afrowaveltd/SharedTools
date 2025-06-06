using Afrowave.SharedTools.Localization.AssemblyTools.Models.Enums;
using System;
using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.AssemblyTools.Models.Plugin
{
	/// <summary>
	/// Static metadata descriptor for a plugin.
	/// Used for discovery, validation, and registration.
	/// </summary>
	public class PluginManifest
	{
		/// <summary>
		/// Unique runtime identifier of this specific plugin instance.
		/// Typically a GUID string.
		/// </summary>
		public string Id { get; set; } = new Guid().ToString();

		/// <summary>
		/// Normalized technical name shared across all instances of this plugin type.
		/// Recommended format: Namespace.Style (e.g. LocalizationPlugin.JsonFlat).
		/// </summary>
		public string NormalizedName { get; set; } = string.Empty;

		/// <summary>
		/// Human-readable name of the plugin for UI or logs.
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Version of the plugin, e.g., "1.0.0".
		/// </summary>
		public string Version { get; set; } = "1.0.0";

		/// <summary>
		/// Name or identifier of the author.
		/// </summary>
		public string Author { get; set; } = string.Empty;

		/// <summary>
		/// Short description of what the plugin does.
		/// </summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Declares the functional type of the plugin.
		/// </summary>
		public PluginType Type { get; set; } = PluginType.Analytics;

		/// <summary>
		/// Declares supported runtime behaviors and abilities.
		/// </summary>
		public PluginBehavior[] Behavior { get; set; } = new PluginBehavior[] { };

		/// <summary>
		/// Detailed plugin capabilities, like events or SignalR support.
		/// </summary>
		public PluginCapabilities Capabilities { get; set; } = new PluginCapabilities();

		/// <summary>
		/// Plugin-specific options, loaded at startup.
		/// </summary>
		public Dictionary<string, object> Options { get; set; } = new Dictionary<string, object>();

		/// <summary>
		/// Licensing information for distribution.
		/// </summary>
		public string License { get; set; } = string.Empty;

		/// <summary>
		/// URL to the license file or page.
		/// </summary>
		public string LicenseUrl { get; set; } = string.Empty;

		/// <summary>
		/// Suggests whether this plugin instance should be enabled by default
		/// at startup. Dispatcher can override this value based on orchestration
		/// logic or failure states. Plugin must still report its runtime status.
		/// </summary>
		public bool EnabledByDefault { get; set; } = true;
	}
}
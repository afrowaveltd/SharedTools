namespace Afrowave.SharedTools.Localization.Common.Models
{
	/// <summary>
	/// Represents a temporary or scoped exclusion of a plugin from participating in execution.
	/// </summary>
	public sealed class PluginScopeExclusion
	{
		/// <summary>
		/// Unique ID of the plugin to be excluded.
		/// </summary>
		public string PluginId { get; set; } = string.Empty;

		/// <summary>
		/// Optional key or scope for which this exclusion applies (e.g. "Translate:Hello").
		/// </summary>
		public string ForKey { get; set; } = string.Empty;

		/// <summary>
		/// Human-readable reason or technical explanation for the exclusion.
		/// </summary>
		public string Reason { get; set; } = string.Empty;
	}
}
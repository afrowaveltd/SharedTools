namespace Afrowave.SharedTools.Localization.AssemblyTools.Models.Plugin;

/// <summary>
/// Declares advanced capabilities supported by a plugin.
/// These are used for validation, orchestration, and UI behavior.
/// Capabilities are static and must be explicitly defined here.
/// </summary>
public class PluginCapabilities
{
	// 🔧 Caching
	public bool SupportsInternalCache { get; set; } = false;

	public bool SupportsCachePlugins { get; set; } = false;

	// 🔁 Event system
	public bool SupportsEvents { get; set; } = false;

	public bool SupportsEventPublishing { get; set; } = false;

	// 🔌 Integration
	public bool SupportsSignalR { get; set; } = false;

	public bool SupportsAnalytics { get; set; } = false;

	// 🔍 Data flow and enrichment
	public bool SupportsHumanReview { get; set; } = false;

	public bool SupportsModeration { get; set; } = false;
	public bool SupportsCorrections { get; set; } = false;
	public bool SupportsDataSniffing { get; set; } = false;

	// ⏲️ Background work
	public bool SupportsScheduling { get; set; } = false;

	// 📊 System behavior
	public bool SupportsDiagnostics { get; set; } = false;

	public bool SupportsStatistics { get; set; } = false;

	// 🔐 Trust boundaries
	public bool SupportsSecurityFiltering { get; set; } = false;

	public bool SupportsTrustScoring { get; set; } = false;

	// 🎯 AI / Prediction
	public bool SupportsInference { get; set; } = false;
}
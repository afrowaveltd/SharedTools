using Afrowave.SharedTools.Localization.Common.Options;

namespace Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Options
{
	/// <summary>
	/// Configuration options for the JSON backend plugin.
	/// This plugin supports both flat and structured JSON, optional caching, event propagation, and SignalR notifications.
	/// </summary>
	public sealed class JsonBackendOptions : PluginOptionSet
	{
		/// <summary>
		/// Gets the unique key identifying the JSON backend.
		/// </summary>
		public override string Key => "json-backend";

		// 📁 File I/O

		/// <summary>
		/// Path to the folder where translation files are stored.
		/// Defaults to: "Locales" (project root, not bin folder).
		/// </summary>
		public string TranslationFolder { get; set; } = "Locales";

		/// <summary>
		/// Pattern used to locate translation files.
		/// Example: "{lang}.json"
		/// </summary>
		public string FilePattern { get; set; } = "{lang}.json";

		// 🔁 Caching

		/// <summary>
		/// Enables local in-memory caching of translations.
		/// </summary>
		public bool UseCache { get; set; } = true;

		/// <summary>
		/// Enables detection of changes to translation files (e.g., FileSystemWatcher).
		/// </summary>
		public bool EnableChangeDetection { get; set; } = true;

		// ⚡ SignalR & Eventing

		/// <summary>
		/// Enables dispatching of translation change events over SignalR.
		/// </summary>
		public bool EnableSignalR { get; set; } = false;

		/// <summary>
		/// Custom SignalR group name for events sent from this plugin.
		/// </summary>
		public string SignalRGroup { get; set; } = string.Empty;

		/// <summary>
		/// Enables propagation of internal events to the core/event bus.
		/// </summary>
		public bool EnableEventRaising { get; set; } = false;

		/// <summary>
		/// Enables logging of all raised events.
		/// </summary>
		public bool LogEvents { get; set; } = false;

		// ✍ Write Capabilities

		/// <summary>
		/// Enables writing new translation keys.
		/// </summary>
		public bool AllowWrite { get; set; } = true;

		/// <summary>
		/// Enables updating of existing translation values.
		/// </summary>
		public bool AllowUpdate { get; set; } = true;

		/// <summary>
		/// Enables deletion of translation keys.
		/// </summary>
		public bool AllowDelete { get; set; } = true;

		// 🌳 Structured JSON

		/// <summary>
		/// Enables support for structured JSON formats (e.g., nested translation trees).
		/// </summary>
		public bool StructuredJsonSupport { get; set; } = true;

		/// <summary>
		/// Falls back to flat JSON if structured parsing fails.
		/// </summary>
		public bool FallbackToFlat { get; set; } = true;
	}
}
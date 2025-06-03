namespace Afrowave.SharedTools.Localization.AssemblyTools.Models.Enums
{
	/// <summary>
	/// Specifies all plugin types supported in the Afrowave.Localization ecosystem.
	/// </summary>
	public enum PluginType
	{
		/// <summary>
		/// Provides access to underlying storage systems (e.g., files, DB, API).
		/// </summary>
		StorageAccessor = 0,

		/// <summary>
		/// Manages dictionary logic (validation, parsing, merging, cache, rules).
		/// </summary>
		DictionaryLogic = 1,

		/// <summary>
		/// Handles translation operations (machine, AI, user-assisted, hybrid).
		/// </summary>
		TranslationProvider = 2,

		/// <summary>
		/// Integrates with external systems or modifies pipeline (web, API, middleware).
		/// </summary>
		Middleware = 3,

		/// <summary>
		/// Moderates, reviews, or filters language data (blacklist, approval, admin).
		/// </summary>
		Moderation = 4,

		/// <summary>
		/// Publishes events/notifications to other systems (webhook, SignalR, etc.).
		/// </summary>
		EventPublisher = 5,

		/// <summary>
		/// Observes and analyzes data streams (logging, sniffing, triggering actions).
		/// </summary>
		DataSniffer = 6,

		/// <summary>
		/// Executes scheduled background tasks (sync, cleanup, reporting, etc.).
		/// </summary>
		Scheduler = 7,

		/// <summary>
		/// Provides automated corrections/self-healing and fixes for plugin configurations.
		/// </summary>
		Corrections = 8,

		/// <summary>
		/// Provides analytics, reporting, statistics, business intelligence, monitoring.
		/// </summary>
		Analytics = 9,

		/// <summary>
		/// Administrative tools for migration, batch operations, upgrades, etc.
		/// </summary>
		AdminTool = 10,

		/// <summary>
		/// Specialized plugin for request/response tracking and audit trails.
		/// </summary>
		Tracker = 11,

		/// <summary>
		/// Provides caching services for other plugins (in-memory, disk, distributed, etc.)
		/// </summary>
		Cache = 12
	}
}
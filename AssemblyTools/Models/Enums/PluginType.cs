namespace Afrowave.SharedTools.Localization.AssemblyTools.Models.Enums
{
	/// <summary>
	/// Specifies the core plugin types supported by the Afrowave.Localization ecosystem.
	/// Each plugin type defines its main role and expected capabilities in the localization pipeline.
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
		Scheduler = 7
	}
}
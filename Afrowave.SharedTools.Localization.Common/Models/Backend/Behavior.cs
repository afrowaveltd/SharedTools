namespace Afrowave.SharedTools.Localization.Common.Models.Backend
{
	/// <summary>
	/// Represents the behavior configuration for a localization backend.
	/// </summary>
	public class Behavior
	{
		/// <summary>
		/// Gets or sets a value indicating whether the backend uses the key as the default value if no translation is found.
		/// </summary>
		public bool UsesKeyAsDefaultValue { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the backend has an internal cache.
		/// </summary>
		public bool HasInternalCache { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the backend supports cache misses.
		/// </summary>
		public bool SupportsCacheMisses { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the backend supports manual cache cleaning.
		/// </summary>
		public bool SupportsManualCacheCleaner { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the backend can detect external changes.
		/// </summary>
		public bool CanDetectExternalChanges { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the backend supports automatic reloading.
		/// </summary>
		public bool SupportsAutoReload { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the backend supports logging.
		/// </summary>
		public bool SupportsLogging { get; set; } = false;

		/// <summary>
		/// Gets or sets the expected response time in milliseconds.
		/// </summary>
		public int ExpectedResponseTimeMs { get; set; } = 500;
	}
}
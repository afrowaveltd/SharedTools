namespace Afrowave.SharedTools.Localization.Common.Models.Backend
{
	/// <summary>
	/// Describes the capabilities and metadata of a localization backend.
	/// </summary>
	public class LocalizationBackendCapabilities
	{
		/// <summary>
		/// Gets or sets the type of the backend (e.g., API, File, Memory).
		/// </summary>
		public string BackendType { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets a human-readable description of the backend.
		/// </summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets a value indicating whether the backend supports reading operations.
		/// </summary>
		public bool CanRead { get; set; } = true;

		/// <summary>
		/// Gets or sets a value indicating whether the backend supports writing operations.
		/// </summary>
		public bool CanWrite { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the backend supports bulk read operations.
		/// </summary>
		public bool CanBulkRead { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the backend supports bulk write operations.
		/// </summary>
		public bool CanBulkWrite { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the backend supports listing available languages.
		/// </summary>
		public bool SupportsLanguagesListing { get; set; } = false;

		/// <summary>
		/// Gets or sets the version of the backend implementation.
		/// </summary>
		public string Version { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the author of the backend implementation.
		/// </summary>
		public string Author { get; set; } = string.Empty;

		/// <summary>
		/// Uses the key as the default value if no translation is found.
		/// </summary>
		public bool UsesKeyAsDefaultValue { get; set; } = false;

		/// <summary>
		/// Indicates whether this backend implementation includes its own internal caching mechanism.
		/// </summary>
		public bool HasInternalCache { get; set; } = false;

		/// <summary>
		/// Indicates whether this backend is able to detect external data changes (e.g., file watcher, SQL triggers).
		/// </summary>
		public bool CanDetectExternalChanges { get; set; } = false;

		/// <summary>
		/// Indicates whether this backend supports clearing or reloading its internal cache manually.
		/// </summary>
		public bool SupportsManualCacheClear { get; set; } = false;
	}
}
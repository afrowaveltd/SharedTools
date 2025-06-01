using Afrowave.SharedTools.Localization.Common.Models.Enums;

namespace Afrowave.SharedTools.Localization.Common.Communication
{
	/// <summary>
	/// Describes the declared capabilities supported by a plugin.
	/// </summary>
	public sealed class Capabilities
	{
		// General operations
		/// <summary>
		/// Indicates whether the capability to read is supported.
		/// </summary>
		public bool CanRead { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the current object supports write operations.
		/// </summary>
		public bool CanWrite { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether updates are allowed.
		/// </summary>
		public bool CanUpdate { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the current item can be deleted.
		/// </summary>
		public bool CanDelete { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether bulk read operations are supported.
		/// </summary>
		public bool CanBulkRead { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether bulk write operations are supported.
		/// </summary>
		public bool CanBulkWrite { get; set; } = false;

		// Extended capabilities
		/// <summary>
		/// Gets or sets a value indicating whether the system supports listing available languages.
		/// </summary>
		public bool CanListLanguages { get; set; } = false;

		/// <summary>
		/// Indicates whether caching is supported.
		/// </summary>
		public bool CanCache { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the system can track cache misses.
		/// </summary>
		public bool CanTrackCacheMisses { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the cache can be cleared manually by the user.
		/// </summary>
		public bool CanClearCacheManually { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the system can detect changes made externally.
		/// </summary>
		public bool CanDetectExternalChanges { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether streaming is supported.
		/// </summary>
		public bool CanStream { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether SignalR functionality is enabled.
		/// </summary>
		public bool CanSignalR { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the current instance can handle events.
		/// </summary>
		public bool CanHandleEvents { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether logging is enabled.
		/// </summary>
		public bool CanLog { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether formatting is supported.
		/// </summary>
		// Future / optional
		public bool CanFormat { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether formatting is supported.
		/// </summary>
		public bool CanVisualize { get; set; } = false;

		/// <summary>
		/// Gets or sets the expected data format for processing input or output.
		/// </summary>
		public DataFormat ExpectedDataFormat { get; set; } = DataFormat.RawJson;
	}
}
namespace Afrowave.SharedTools.Localization.Common.Communication
{
	/// <summary>
	/// Describes the declared capabilities supported by a plugin.
	/// </summary>
	public sealed class Capabilities
	{
		// General operations
		public bool CanRead { get; set; } = false;

		public bool CanWrite { get; set; } = false;
		public bool CanUpdate { get; set; } = false;
		public bool CanDelete { get; set; } = false;
		public bool CanBulkRead { get; set; } = false;
		public bool CanBulkWrite { get; set; } = false;

		// Extended capabilities
		public bool CanListLanguages { get; set; } = false;

		public bool CanCache { get; set; } = false;
		public bool CanTrackCacheMisses { get; set; } = false;
		public bool CanClearCacheManually { get; set; } = false;
		public bool CanDetectExternalChanges { get; set; } = false;

		public bool CanStream { get; set; } = false;
		public bool CanSignalR { get; set; } = false;
		public bool CanHandleEvents { get; set; } = false;
		public bool CanLog { get; set; } = false;

		// Future / optional
		public bool CanFormat { get; set; } = false;

		public bool CanVisualize { get; set; } = false;
	}
}
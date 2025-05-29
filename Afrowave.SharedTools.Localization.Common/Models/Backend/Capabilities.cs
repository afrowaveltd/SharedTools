namespace Afrowave.SharedTools.Localization.Common.Models.Backend
{
	/// <summary>
	/// Represents the capabilities of a localization backend.
	/// </summary>
	public class Capabilities
	{
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
		/// Gets or sets a value indicating whether the backend supports delete operations.
		/// </summary>
		public bool CanDelete { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the backend supports update operations.
		/// </summary>
		public bool CanUpdate { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the backend supports listing available languages.
		/// </summary>
		public bool SupportsLanguageListing { get; set; } = false;
	}
}
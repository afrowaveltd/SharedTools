namespace Afrowave.SharedTools.Localization.Common.Models.Backend
{
	/// <summary>
	/// Represents metadata information for a localization backend.
	/// </summary>
	public class Metadata
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
		/// Gets or sets the version of the backend implementation.
		/// </summary>
		public string Version { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the author of the backend implementation.
		/// </summary>
		public string Author { get; set; } = string.Empty;
	}
}
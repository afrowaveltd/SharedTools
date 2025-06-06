namespace Afrowave.SharedTools.Docs.Models
{
	/// <summary>
	/// Represents the configuration settings for documentation, including metadata and related links.
	/// </summary>
	/// <remarks>This class is used to store and manage settings related to documentation, such as descriptive
	/// information and associated links. The <see cref="Name"/> property serves as the unique identifier  for each
	/// configuration.</remarks>
	public class DocsSettings
	{
		/// <summary>
		/// Gets or sets the name associated with the entity.
		/// </summary>
		[Key]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the Markdown-formatted content that provides additional information or context.
		/// </summary>
		public string? MdAbout { get; set; }

		/// <summary>
		/// Gets or sets the Markdown-formatted link to the "About" section.
		/// </summary>
		public string? MdAboutLink { get; set; }
	}
}
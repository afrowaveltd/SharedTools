namespace Afrowave.SharedTools.Docs.Models.DatabaseModels;

/// <summary>
/// Represents a folder in a Markdown-based file structure, including its path, name, last text content, and last
/// modified timestamp.
/// </summary>
/// <remarks>This class is designed to encapsulate metadata about a folder in a Markdown file system.  It provides
/// properties to access the folder's path, name, last text content, and the date and time it was last
/// modified.</remarks>
public class MdFolder
{
	/// <summary>
	/// Get or sets an unique ID of the entity
	/// </summary>
	[Key]
	public int Id { get; set; } = 0;

	/// <summary>
	/// Gets or sets the path to the folder.
	/// </summary>
	public string Path { get; set; } = string.Empty; // The path to the folder, e.g., "docs/folder1"

	/// <summary>
	/// Gets or sets the name of the folder.
	/// </summary>
	public string Name { get; set; } = string.Empty; // The name of the folder, e.g., "folder1"

	/// <summary>
	/// Gets or sets the last text content in the folder.
	/// </summary>
	public string LastText { get; set; } = string.Empty; // The last text content in the folder, e.g., "Last text in folder1"

	/// <summary>
	/// Gets or sets the date and time when the folder was last modified.
	/// </summary>
	public DateTime LastModified { get; set; } = DateTime.UtcNow; // The last modified date of the folder, e.g., "2023-10-01T12:00:00Z"
}
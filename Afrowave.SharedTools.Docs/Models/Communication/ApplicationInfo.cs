namespace Afrowave.SharedTools.Docs.Models.Communication
{
	/// <summary>
	/// Represents information about an application, including its name and description.
	/// </summary>
	public class ApplicationInfo
	{
		/// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		public string ApplicationName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the description associated with the object.
		/// </summary>
		public string? Description { get; set; }
	}
}
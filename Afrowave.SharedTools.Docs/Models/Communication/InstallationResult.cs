namespace Afrowave.SharedTools.Docs.Models.Communication
{
	/// <summary>
	/// Represents the result of an installation process, including application settings and administrative details.
	/// </summary>
	/// <remarks>This class encapsulates the outcome of an installation, providing access to the configured
	/// application settings and administrative information. It is typically used to retrieve the state of the system after
	/// an installation has been completed.</remarks>
	public class InstallationResult
	{
		/// <summary>
		/// Gets or sets the application settings used to configure the behavior of the application.
		/// </summary>
		public ApplicationSettings ApplicationSettings { get; set; } = new();

		/// <summary>
		/// Gets or sets the administrative settings for the application.
		/// </summary>
		public Admin Admin { get; set; } = new();
	}
}
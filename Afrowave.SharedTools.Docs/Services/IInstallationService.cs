namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Defines methods for managing the installation of applications.
	/// </summary>
	/// <remarks>This service provides functionality to install applications and check their installation
	/// status.</remarks>
	public interface IInstallationService
	{
		/// <summary>
		/// Installs the specified application on the target system.
		/// </summary>
		/// <remarks>This method performs an asynchronous installation of the application. Ensure that the provided
		/// <paramref name="application"/> object contains all required information for the installation process.</remarks>
		/// <param name="application">The application installation details, including the application package and configuration settings. This parameter
		/// cannot be <see langword="null"/>.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/>  object
		/// with the installation result, including success status and any relevant messages.</returns>
		Task<Response<InstallationResult>> InstallApplication(ApplicationInstall application);

		/// <summary>
		/// Determines whether the application is installed on the current device.
		/// </summary>
		/// <remarks>This method performs an asynchronous check to verify the installation status of the application.
		/// The result can be used to conditionally execute logic based on the application's presence.</remarks>
		/// <returns><see langword="true"/> if the application is installed; otherwise, <see langword="false"/>.</returns>
		Task<bool> IsInstalledAsync();
	}
}
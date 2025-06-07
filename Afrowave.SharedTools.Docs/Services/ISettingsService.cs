namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Provides methods for managing and accessing application settings.
	/// </summary>
	/// <remarks>This service allows checking the installation status, loading and saving settings,  and verifying
	/// the existence of settings. All operations are asynchronous.</remarks>
	public interface ISettingsService
	{
		/// <summary>
		/// Determines whether the application is installed on the current device.
		/// </summary>
		/// <remarks>This method performs an asynchronous check to verify the installation status of the application.
		/// The result can be used to conditionally execute logic based on the application's presence.</remarks>
		/// <returns><see langword="true"/> if the application is installed; otherwise, <see langword="false"/>.</returns>
		Task<bool> IsInstalledAsync();

		/// <summary>
		/// Asynchronously loads the current documentation settings.
		/// </summary>
		/// <remarks>The returned <see cref="Response{T}"/> object provides additional metadata about the operation,
		/// such as success status or error details. Ensure to check the response status before using the settings.</remarks>
		/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.  The task result contains a <see
		/// cref="Response{T}"/> object that wraps the loaded <see cref="ApplicationSettings"/>.</returns>
		Task<Response<ApplicationSettings>> LoadSettingsAsync();

		/// <summary>
		/// Saves the specified settings asynchronously and returns the updated settings.
		/// </summary>
		/// <remarks>This method performs an asynchronous save operation. Ensure that the provided <see
		/// cref="ApplicationSettings"/> object is valid and contains all required values before calling this method.</remarks>
		/// <param name="settings">The <see cref="ApplicationSettings"/> object containing the settings to be saved. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with the updated <see cref="ApplicationSettings"/> after the save operation completes.</returns>
		Task<Response<ApplicationSettings>> SaveSettingsAsync(ApplicationSettings settings);

		/// <summary>
		/// Determines whether the settings exist in the current context.
		/// </summary>
		/// <remarks>This method performs an asynchronous check to verify the presence of settings. The result
		/// indicates whether the settings are available for use.</remarks>
		/// <returns><see langword="true"/> if the settings exist; otherwise, <see langword="false"/>.</returns>
		Task<bool> SettingsExists();
	}
}
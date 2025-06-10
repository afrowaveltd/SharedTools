namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Defines methods for managing application settings, including API keys, SMTP settings, and application information.
	/// </summary>
	/// <remarks>This service provides functionality to retrieve, update, and reset various application settings. It
	/// is designed to handle operations related to configuration management in a consistent and asynchronous
	/// manner.</remarks>
	public interface ISettingsService
	{
		/// <summary>
		/// Asynchronously retrieves the API key associated with the current user or application.
		/// </summary>
		/// <remarks>The API key is typically used for authenticating requests to external services.  Ensure that the
		/// returned key is securely stored and not exposed to unauthorized access.</remarks>
		/// <returns>A <see cref="Response{T}"/> object containing the API key as a string if the operation is successful. The <see
		/// cref="Response{T}"/> may include error details if the retrieval fails.</returns>
		Task<Response<string>> GetApiKeyAsync();

		/// <summary>
		/// Asynchronously retrieves information about the current application.
		/// </summary>
		/// <remarks>Use this method to obtain metadata or configuration details about the application. The returned
		/// <see cref="Response{T}"/> object provides additional context, such as status or error information, alongside the
		/// application data.</remarks>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with the application information encapsulated in an <see cref="ApplicationInfo"/> instance.</returns>
		Task<Response<ApplicationInfo>> GetApplicationInfoAsync();

		/// <summary>
		/// Retrieves the current SMTP settings for the application.
		/// </summary>
		/// <remarks>Use this method to fetch the SMTP configuration, such as server address, port, and credentials,
		/// required for sending emails. Ensure that the caller handles any potential errors in the response.</remarks>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with the SMTP settings. If the operation fails, the response may include error details.</returns>
		Task<Response<SmtpSettings>> GetSmtpSettingsAsync();

		/// <summary>
		/// Resets the API key and generates a new one.
		/// </summary>
		/// <remarks>This method invalidates the current API key and creates a new one.  Use this method when the
		/// existing API key is compromised or needs to be refreshed. The new API key is returned in the response.</remarks>
		/// <returns>A <see cref="Response{T}"/> containing the newly generated API key as a string. The response will include the new
		/// key if the operation is successful.</returns>
		Task<Response<string>> ResetApiKeyAsync();

		/// <summary>
		/// Saves the specified SMTP settings asynchronously.
		/// </summary>
		/// <remarks>Use this method to persist SMTP settings, such as server address, port, and credentials. Ensure
		/// that the provided <paramref name="settings"/> object contains valid values before calling this method.</remarks>
		/// <param name="settings">The SMTP settings to be saved. Cannot be null.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with the saved <see cref="SmtpSettings"/> if the operation is successful.</returns>
		Task<Response<SmtpSettings>> SaveSmtpSettingsAsync(SmtpSettings settings);

		/// <summary>
		/// Updates the application information with the provided details asynchronously.
		/// </summary>
		/// <remarks>Use this method to update the details of an existing application. Ensure that the <paramref
		/// name="info"/> parameter contains valid and complete information before calling this method. The operation is
		/// performed asynchronously and may involve network communication or database updates.</remarks>
		/// <param name="info">The <see cref="ApplicationInfo"/> object containing the updated application details. This parameter cannot be
		/// null.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with the updated <see cref="ApplicationInfo"/> if the operation is successful.</returns>
		Task<Response<ApplicationInfo>> UpdateApplicationInformationsAsync(ApplicationInfo info);
	}
}
namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Defines methods for managing administrative accounts, including registration, email confirmation,  authentication,
	/// and retrieval of administrative details.
	/// </summary>
	/// <remarks>This interface provides functionality for handling administrative user accounts, such as
	/// registering  new administrators, confirming email addresses, retrieving administrator details, and managing
	/// authentication tokens. Each method is asynchronous and returns a task, allowing for non-blocking  operations in
	/// applications.</remarks>
	public interface IAdminService
	{
		/// <summary>
		/// Determines whether the specified email address is eligible for registration.
		/// </summary>
		/// <remarks>This method performs validation to determine if the provided email address meets the criteria for
		/// registration.  The criteria may include checks for format, uniqueness, or other business rules.</remarks>
		/// <param name="email">The email address to check for registration eligibility. Must be a valid, non-null, and non-empty string.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the email
		/// address can be registered; otherwise, <see langword="false"/>.</returns>
		Task<bool> CanEmailRegister(string email);

		/// <summary>
		/// Updates the administrator's email address.
		/// </summary>
		/// <remarks>Ensure that the <paramref name="newEmail"/> is unique and not already associated with another
		/// account.</remarks>
		/// <param name="oldEmail">The current email address of the administrator. This value must match the existing email on record.</param>
		/// <param name="newEmail">The new email address to replace the current one. This value must be a valid email format and cannot be null or
		/// empty.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with a string message indicating the outcome of the operation.</returns>
		Task<Response<string>> ChangeAdminEmail(string oldEmail, string newEmail);

		/// <summary>
		/// Confirms the email address by verifying its validity and updating the user's email confirmation status.
		/// </summary>
		/// <remarks>Use this method to confirm a user's email address as part of an account verification process.
		/// The method returns a response object that encapsulates the success status of the operation.</remarks>
		/// <param name="email">The email address to confirm. Must be a valid, non-null, and non-empty string.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with a boolean value indicating whether the email confirmation was successful.</returns>
		Task<Response<bool>> ConfirmEmail(string email);

		/// <summary>
		/// Deletes an administrator account associated with the specified email address.
		/// </summary>
		/// <remarks>Use this method to remove an administrator account by their email address. Ensure that the email
		/// address provided corresponds to an existing administrator. The operation is asynchronous and may involve network
		/// or database interactions.</remarks>
		/// <param name="email">The email address of the administrator to delete. Must be a valid, non-null, and non-empty string.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with a boolean value indicating whether the deletion was successful.</returns>
		Task<Response<bool>> DeleteAdminByEmailAsync(string email);

		/// <summary>
		/// Retrieves an administrator's details based on their email address.
		/// </summary>
		/// <remarks>Use this method to fetch information about an administrator by their unique email address. Ensure
		/// the email provided is valid and corresponds to an existing administrator in the system.</remarks>
		/// <param name="email">The email address of the administrator to retrieve. Cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with the administrator's details if found, or an appropriate error response if not.</returns>
		Task<Response<Admin>> GetAdminByEmailAsync(string email);

		/// <summary>
		/// Retrieves a bearer token associated with the specified email address.
		/// </summary>
		/// <remarks>The returned bearer token can be used for authentication in subsequent API requests.  Ensure the
		/// email provided is valid and registered in the system.</remarks>
		/// <param name="email">The email address for which to retrieve the bearer token.  This parameter cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a  <see cref="Response{T}"/> object
		/// with the bearer token as a string if the operation succeeds.</returns>
		Task<Response<string>> GetBearer(string email);

		/// <summary>
		/// Registers a new admin user asynchronously.
		/// </summary>
		/// <remarks>Ensure that the <paramref name="model"/> contains all required fields and that they meet the
		/// validation criteria before calling this method. The returned <see cref="Response{T}"/> object will indicate the
		/// success or failure of the operation.</remarks>
		/// <param name="model">The data model containing the details required to register the admin, such as username, email, and password.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> object
		/// with the registered <see cref="Admin"/> details if the operation is successful.</returns>
		Task<Response<Admin>> RegisterAdminAsync(RegisterAdminModel model);

		/// <summary>
		/// Sets the bearer token for the specified email address.
		/// </summary>
		/// <param name="email">The email address associated with the bearer token. Cannot be null or empty.</param>
		/// <param name="bearer">The bearer token to associate with the email address. Cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the bearer
		/// token was successfully set; otherwise, <see langword="false"/>.</returns>
		Task<Response<bool>> SetBearer(string email, string bearer);
	}
}
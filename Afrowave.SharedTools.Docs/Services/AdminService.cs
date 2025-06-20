namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Provides administrative services for managing admin accounts, including registration, email confirmation,  and
	/// token management. This service interacts with the underlying database and logs relevant operations.
	/// </summary>
	/// <remarks>The <see cref="AdminService"/> class is designed to handle common administrative tasks such as
	/// registering  new admins, retrieving admin details, updating admin information, and managing authentication tokens.
	/// It ensures proper validation and logging for all operations and uses localization for user-facing
	/// messages.</remarks>
	/// <param name="logger"></param>
	/// <param name="context"></param>
	/// <param name="localizer"></param>
	public class AdminService(ILogger<AdminService> logger,
		DocsDbContext context,
		IStringLocalizer<AdminService> localizer) : IAdminService
	{
		private readonly ILogger<AdminService> _logger = logger;
		private readonly DocsDbContext _context = context;
		private readonly IStringLocalizer<AdminService> _localizer = localizer;

		/// <summary>
		/// Registers a new admin with the provided email and optional display name.
		/// </summary>
		/// <param name="model">Admin registration model</param>
		/// <returns>Response indicating the result of the registration process.</returns>
		public async Task<Response<Admin>> RegisterAdminAsync(RegisterAdminModel model)
		{
			if(model == null)
			{
				_logger.LogError("RegisterAdminAsync: Model is null.");
				return Response<Admin>.Fail(_localizer["Invalid model"]);
			}
			var existingAdmin = await _context.Admins
				.FirstOrDefaultAsync(a => a.Email == model.Email);
			if(existingAdmin != null)
			{
				_logger.LogWarning("RegisterAdminAsync: Admin with email {Email} already exists.", model.Email);
				return Response<Admin>.Fail(_localizer["Email already registered."]);
			}
			if(string.IsNullOrWhiteSpace(model.DisplayName))
			{
				model.DisplayName = model.Email; // Default to email if display name is not provided
			}
			var newAdmin = new Admin
			{
				Email = model.Email,
				DisplayName = model.DisplayName,
				IsActive = true
			};
			await _context.Admins.AddAsync(newAdmin);
			await _context.SaveChangesAsync();
			_logger.LogInformation("RegisterAdminAsync: New admin registered with email {Email}.", model.Email);
			return Response<Admin>.Successful(newAdmin, _localizer["Registration was successful"]);
		}

		/// <summary>
		/// Retrieves an admin by their email address.
		/// </summary>
		/// <param name="email">The email of the admin to retrieve.</param>
		/// <returns>A response with the admin details if found.</returns>
		public async Task<Response<Admin>> GetAdminByEmailAsync(string email)
		{
			if(string.IsNullOrWhiteSpace(email))
			{
				_logger.LogError("GetAdminByEmailAsync: Email is null or empty.");
				return Response<Admin>.Fail(_localizer["Email is mandatory."]);
			}
			var admin = await _context.Admins
				.FirstOrDefaultAsync(a => a.Email == email);
			if(admin == null)
			{
				_logger.LogWarning("GetAdminByEmailAsync: No admin found with email {Email}.", email);
				return Response<Admin>.Fail(_localizer["Admin not found."]);
			}
			return Response<Admin>.Successful(admin, _localizer["Admin retrieved successfully."]);
		}

		/// <summary>
		/// Deletes an admin user identified by their email address.
		/// </summary>
		/// <remarks>This method searches for an admin user with the specified email address. If no admin is found,
		/// the operation fails and returns an appropriate error message. If an admin is found, they are removed from the
		/// database, and the changes are saved. Logging is performed to record the outcome of the operation.</remarks>
		/// <param name="email">The email address of the admin to delete. Cannot be null, empty, or whitespace.</param>
		/// <returns>A <see cref="Response{T}"/> object containing a boolean value indicating whether the operation was successful. If
		/// the operation fails, the response includes an error message describing the reason.</returns>
		public async Task<Response<bool>> DeleteAdminByEmailAsync(string email)
		{
			if(string.IsNullOrWhiteSpace(email))
			{
				_logger.LogError("DeleteAdminByEmailAsync: Email is null or empty.");
				return Response<bool>.Fail(_localizer["Email is mandatory."]);
			}
			var admin = await _context.Admins
				.FirstOrDefaultAsync(a => a.Email == email);
			if(admin == null)
			{
				_logger.LogWarning("DeleteAdminByEmailAsync: No admin found with email {Email}.", email);
				return Response<bool>.Fail(_localizer["Admin not found."]);
			}
			_context.Admins.Remove(admin);
			await _context.SaveChangesAsync();
			_logger.LogInformation("DeleteAdminByEmailAsync: Admin with email {Email} deleted.", email);
			return Response<bool>.Successful(true, _localizer["Admin deleted successfully."]);
		}

		/// <summary>
		/// Confirms the email address of an admin user.
		/// </summary>
		/// <remarks>If the specified email address is not associated with any admin user, the operation will
		/// fail.</remarks>
		/// <param name="email">The email address of the admin to confirm. Cannot be null, empty, or whitespace.</param>
		/// <returns>A <see cref="Response{T}"/> object containing a boolean value indicating whether the email confirmation was
		/// successful. Returns <see langword="true"/> if the email was successfully confirmed; otherwise, <see
		/// langword="false"/>.</returns>
		public async Task<Response<bool>> ConfirmEmail(string email)
		{
			if(string.IsNullOrWhiteSpace(email))
			{
				_logger.LogError("ConfirmEmail: Email is null or empty.");
				return Response<bool>.Fail(_localizer["Email is mandatory."]);
			}
			var admin = await _context.Admins
				.FirstOrDefaultAsync(a => a.Email == email);
			if(admin == null)
			{
				_logger.LogWarning("ConfirmEmail: No admin found with email {Email}.", email);
				return Response<bool>.Fail(_localizer["Admin not found."]);
			}
			admin.IsEmailConfirmed = true;
			await _context.SaveChangesAsync();
			_logger.LogInformation("ConfirmEmail: Email for admin with email {Email} confirmed.", email);
			return Response<bool>.Successful(true, _localizer["Email confirmed successfully."]);
		}

		/// <summary>
		/// Changes the email address of an admin user.
		/// </summary>
		/// <remarks>This method updates the email address of an admin user in the system. If the provided <paramref
		/// name="oldEmail"/>  does not match any existing admin, the operation will fail. The email confirmation status is
		/// reset after the email  is changed, requiring the admin to confirm the new email address.</remarks>
		/// <param name="oldEmail">The current email address of the admin. This value cannot be null, empty, or whitespace.</param>
		/// <param name="newEmail">The new email address to assign to the admin. This value cannot be null, empty, or whitespace.</param>
		/// <returns>A <see cref="Response{T}"/> containing the new email address if the operation is successful,  or an error message if
		/// the operation fails.</returns>
		public async Task<Response<string>> ChangeAdminEmail(string oldEmail, string newEmail)
		{
			if(string.IsNullOrWhiteSpace(oldEmail) || string.IsNullOrWhiteSpace(newEmail))
			{
				_logger.LogError("ChangeAdminEmail: Old or new email is null or empty.");
				return Response<string>.Fail(_localizer["Old and new email cannot be null or empty."]);
			}
			var admin = await _context.Admins
				.FirstOrDefaultAsync(a => a.Email == oldEmail);
			if(admin == null)
			{
				_logger.LogWarning("ChangeAdminEmail: No admin found with email {OldEmail}.", oldEmail);
				return Response<string>.Fail(_localizer["Admin not found."]);
			}
			admin.Email = newEmail;
			admin.IsEmailConfirmed = false; // Reset email confirmation status
			await _context.SaveChangesAsync();
			_logger.LogInformation("ChangeAdminEmail: Admin email changed from {OldEmail} to {NewEmail}.", oldEmail, newEmail);
			return Response<string>.Successful(newEmail, _localizer["Admin email changed successfully."]);
		}

		/// <summary>
		/// Sets the bearer token for the admin associated with the specified email address.
		/// </summary>
		/// <remarks>If no admin is found with the specified email address, the operation fails and a localized error
		/// message is returned.</remarks>
		/// <param name="email">The email address of the admin whose bearer token is to be set. Cannot be null, empty, or whitespace.</param>
		/// <param name="bearer">The bearer token to assign to the admin. Cannot be null, empty, or whitespace.</param>
		/// <returns>A <see cref="Response{T}"/> object containing a boolean value indicating whether the operation was successful.
		/// Returns <see langword="true"/> if the bearer token was successfully set; otherwise, <see langword="false"/>.</returns>
		public async Task<Response<bool>> SetBearer(string email, string bearer)
		{
			if(string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(bearer))
			{
				_logger.LogError("SetBearer: Email or bearer is null or empty.");
				return Response<bool>.Fail(_localizer["Email and bearer cannot be null or empty."]);
			}
			var admin = await _context.Admins
				.FirstOrDefaultAsync(a => a.Email == email);
			if(admin == null)
			{
				_logger.LogWarning("SetBearer: No admin found with email {Email}.", email);
				return Response<bool>.Fail(_localizer["Admin not found."]);
			}
			admin.Bearer = bearer;
			await _context.SaveChangesAsync();
			_logger.LogInformation("SetBearer: Bearer token set for admin with email {Email}.", email);
			return Response<bool>.Successful(true, _localizer["Bearer token set successfully."]);
		}

		/// <summary>
		/// Retrieves the bearer token associated with the specified admin email.
		/// </summary>
		/// <remarks>This method logs an error if the provided email is null or empty, and logs a warning if no admin is
		/// found with the specified email.</remarks>
		/// <param name="email">The email address of the admin whose bearer token is to be retrieved. Cannot be null, empty, or whitespace.</param>
		/// <returns>A <see cref="Response{T}"/> containing the bearer token as a string if the admin is found and the email is valid. If
		/// the email is null, empty, or whitespace, or if no admin is found with the specified email, the response will
		/// indicate failure with an appropriate error message.</returns>
		public async Task<Response<string>> GetBearer(string email)
		{
			if(string.IsNullOrWhiteSpace(email))
			{
				_logger.LogError("GetBearer: Email is null or empty.");
				return Response<string>.Fail(_localizer["Email is mandatory."]);
			}
			var admin = await _context.Admins
				.FirstOrDefaultAsync(a => a.Email == email);
			if(admin == null)
			{
				_logger.LogWarning("GetBearer: No admin found with email {Email}.", email);
				return Response<string>.Fail(_localizer["Admin not found."]);
			}
			return Response<string>.Successful(admin.Bearer, _localizer["Bearer token retrieved successfully."]);
		}

		/// <summary>
		/// Determines whether the specified email address can be registered.
		/// </summary>
		/// <remarks>This method checks the database to determine if the provided email address is already associated with
		/// an admin account.</remarks>
		/// <param name="email">The email address to check. This value must not be null or empty and will be normalized to lowercase and trimmed
		/// before validation.</param>
		/// <returns><see langword="true"/> if the email address is not already registered; otherwise, <see langword="false"/>.</returns>
		public async Task<bool> CanEmailRegister(string email)
		{
			email = email.ToLowerInvariant().Trim();
			return !await _context.Admins.AnyAsync(s => s.Email == email);
		}
	}
}
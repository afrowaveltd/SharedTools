namespace Afrowave.SharedTools.Docs.Models.Dto
{
	/// <summary>
	/// Represents the data required to register a new administrator.
	/// </summary>
	/// <remarks>This model is used to capture the necessary information for creating a new administrator account.
	/// Both <see cref="Email"/> and <see cref="DisplayName"/> are required fields.</remarks>
	public class RegisterAdminModel
	{
		/// <summary>
		/// Gets or sets the email address of the user.
		/// </summary>
		[Required]
		[EmailAddress(ErrorMessage = "Invalid email address format.")]
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the display name of the entity.
		/// </summary>
		[Required]
		public string? DisplayName { get; set; } = string.Empty;
	}
}
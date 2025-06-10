namespace Afrowave.SharedTools.Docs.Models.DatabaseModels
{
	/// <summary>
	/// Represents an administrative user with properties for authentication and status management.
	/// </summary>
	/// <remarks>The <see cref="Admin"/> class includes properties for managing authentication details such as
	/// one-time passwords (OTPs), email, and bearer tokens, as well as tracking the user's active status.</remarks>
	public class Admin
	{
		/// <summary>
		/// Gets or sets the unique identifier for the entity.
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the registration occurred.
		/// </summary>
		public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Gets or sets the one-time password (OTP) associated with the current operation.
		/// </summary>
		public string Otp { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the expiration date and time for the OTP (One-Time Password).
		/// </summary>
		public DateTime OtpValidUntil { get; set; } = DateTime.UtcNow.AddMinutes(15);

		/// <summary>
		/// Gets or sets the email address associated with the user.
		/// </summary>
		[Required]
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets a value indicating whether the user's email address has been confirmed.
		/// </summary>
		public bool IsEmailConfirmed { get; set; } = false;

		/// <summary>
		/// Gets or sets the display name associated with the object.
		/// </summary>
		public string? DisplayName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets a value indicating whether the entity is active.
		/// </summary>
		public bool IsActive { get; set; } = true;

		/// <summary>
		/// Gets or sets a value indicating whether the current user is the owner.
		/// </summary>
		public bool IsOwner { get; set; } = false;

		/// <summary>
		/// Gets or sets the bearer token used for authentication or authorization purposes.
		/// </summary>
		public string Bearer { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the total time the user has been online.
		/// </summary>
		public TimeSpan TimeOnline { get; set; } = TimeSpan.Zero;

		/// <summary>
		/// Gets or sets the timestamp of the last recorded activity.
		/// </summary>
		public DateTime LastSeen { get; set; } = DateTime.UtcNow;
	}
}
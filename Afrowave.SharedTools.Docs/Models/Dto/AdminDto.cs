namespace Afrowave.SharedTools.Docs.Models.Dto
{
	/// <summary>
	/// Represents an administrator in the system, including their account details, status, and activity information.
	/// </summary>
	/// <remarks>This data transfer object (DTO) is used to encapsulate information about an administrator for use
	/// in API responses or other data exchanges. It includes properties such as the administrator's unique identifier,
	/// email, display name, account status, and activity metrics.</remarks>
	public class AdminDto
	{
		/// <summary>
		/// Gets or sets the unique identifier for the entity.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the registration date for the administrator.
		/// </summary>
		public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Gets or sets the email address associated with the user.
		/// </summary>
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
		/// Gets or sets the total time the user has been online.
		/// </summary>
		public TimeSpan TimeOnLine { get; set; } = TimeSpan.Zero;

		/// <summary>
		/// Gets or sets the timestamp of the last recorded activity.
		/// </summary>
		public DateTime LastSeen { get; set; } = DateTime.UtcNow;
	}
}
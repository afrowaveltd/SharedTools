using MailKit.Security;

namespace Afrowave.SharedTools.Docs.Models.DatabaseModels
{
	/// <summary>
	/// Represents the application settings required for configuring the application and SMTP server connection.
	/// </summary>
	/// <remarks>This class encapsulates configuration settings for the application, including general application
	/// metadata  (such as the application name and description) and SMTP server settings for email communication.  The
	/// SMTP-related properties include server host, port, security options, and authentication details.</remarks>
	public class ApplicationSettings
	{
		/// <summary>
		/// Represents the configuration settings required for connecting to an SMTP server.
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		public string ApplicationName { get; set; } = "Afrowave.SharedTools.Docs";

		/// <summary>
		/// Gets or sets the description of the Afrowave Shared Tools.
		/// </summary>
		public string? Description { get; set; } = "Documentation for Afrowave Shared Tools";

		// Next setting are for SMTP service - necessary for administration
		/// <summary>
		/// Gets or sets the host name or address of the server.
		/// </summary>
		[Required]
		public string SmtpHost { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the port number used for network communication.
		/// </summary>
		[Range(0, 65535, ErrorMessage = "Port must be between 0 and 65535.")]
		public int Port { get; set; } = 0;

		/// <summary>
		/// Gets or sets the API key used for authenticating requests.
		/// </summary>
		public string ApiKey { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the options used to configure the security settings for the socket connection.
		/// </summary>
		/// <remarks>Use this property to specify the desired level of security for the socket connection, such as
		/// whether to use SSL/TLS encryption or to allow unencrypted connections. The <see cref="SecureSocketOptions.Auto"/>
		/// option automatically selects the appropriate security settings based on the context.</remarks>
		public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.Auto;

		/// <summary>
		/// Gets or sets the email address associated with the user.
		/// </summary>
		[Required]
		[EmailAddress(ErrorMessage = "Invalid email address format.")]
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the name of the sender.
		/// </summary>
		public string SenderName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets a value indicating whether authentication is required for the operation.
		/// </summary>
		public bool UseAuthentication { get; set; } = false;

		/// <summary>
		/// Gets or sets the login identifier for the user.
		/// </summary>
		public string? Login { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the encrypted password.
		/// </summary>
		public string? EncryptedPassword { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the information if the settings were successfully tested.
		/// </summary>
		public bool SuccessfullyTested { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the entity is active.
		/// Entity can't be enabled if it is not tested successfully.
		/// </summary>
		public bool IsActive { get; set; } = false;
	}
}
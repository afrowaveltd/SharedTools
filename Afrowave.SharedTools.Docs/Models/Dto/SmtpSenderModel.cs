using MailKit.Security;

namespace Afrowave.SharedTools.Docs.Models.Dto
{
	/// <summary>
	/// Represents the configuration settings required for sending emails using an SMTP server.
	/// </summary>
	/// <remarks>This model encapsulates the necessary properties for configuring an SMTP client, including server
	/// details, authentication credentials, and sender information. It is typically used to provide the required settings
	/// for email-sending services.</remarks>
	public class SmtpSenderModel
	{
		/// <summary>
		/// Gets or sets the host.
		/// </summary>
		public string Host { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		public int? Port { get; set; } = 0;

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		public string? Username { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		public string? Password { get; set; }

		/// <summary>
		/// Gets or sets the sender email.
		/// </summary>
		[Required]
		public string SenderEmail { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the sender name.
		/// </summary>
		[Required]
		public string SenderName { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the secure socket options.
		/// </summary>
		public SecureSocketOptions Secure { get; set; } = SecureSocketOptions.Auto;

		/// <summary>
		/// Gets or sets a value indicating whether authorization required.
		/// </summary>
		public bool AuthorizationRequired { get; set; } = true;

		/// <summary>
		/// Gets or sets the target for testing.
		/// </summary>
		public string? TargetForTesting { get; set; }
	}
}
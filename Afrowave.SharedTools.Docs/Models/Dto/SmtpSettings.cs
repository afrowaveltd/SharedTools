using MailKit.Security;

namespace Afrowave.SharedTools.Docs.Models.Dto
{
	/// <summary>
	/// Represents the configuration settings required to send emails using an SMTP server.
	/// </summary>
	/// <remarks>This class encapsulates the necessary properties for configuring an SMTP client,  including server
	/// details, authentication credentials, and security options.  It is typically used to configure email-sending
	/// functionality in an application.</remarks>
	public class SmtpSettings
	{
		/// <summary>
		/// Gets or sets the hostname or IP address of the server to connect to.
		/// </summary>
		public string Host { get; set; } = "localhost";

		/// <summary>
		/// Gets or sets the port number used for the connection.
		/// </summary>
		public int Port { get; set; } = 25;

		/// <summary>
		/// Gets or sets the options for configuring the security of the socket connection.
		/// </summary>
		/// <remarks>Use this property to specify the desired level of security for the socket connection, such as SSL
		/// or TLS.</remarks>
		public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.Auto;

		/// <summary>
		/// Gets or sets the email address associated with the user.
		/// </summary>
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the name of the sender.
		/// </summary>
		public string SenderName { get; set; } = "Afrowave Shared Tools";

		/// <summary>
		/// Gets or sets a value indicating whether authentication is required for the operation.
		/// </summary>
		public bool UseAuthentication { get; set; } = false;

		/// <summary>
		/// Gets or sets the login identifier for the user.
		/// </summary>
		public string? Login { get; set; }

		/// <summary>
		/// Gets or sets the password associated with the user or system.
		/// </summary>
		/// <remarks>Ensure that the password is stored and transmitted securely to prevent unauthorized
		/// access.</remarks>
		public string? Password { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the test was successfully completed.
		/// </summary>
		public bool SuccessfullyTested { get; set; } = false;
	}
}
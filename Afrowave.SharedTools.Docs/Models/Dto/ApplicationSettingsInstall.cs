using MailKit.Security;

namespace Afrowave.SharedTools.Docs.Models.Dto
{
	/// <summary>
	/// Represents the configuration settings required for installing and configuring an application,  including email
	/// server settings and authentication options.
	/// </summary>
	/// <remarks>This class is typically used to store and manage application-specific settings, such as  email
	/// server configuration, authentication credentials, and metadata about the application. It provides properties for
	/// specifying the host, port, security options, and other details  necessary for email communication and application
	/// setup.</remarks>
	public class ApplicationSettingsInstall
	{
		/// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		public string ApplicationName { get; set; } = "Documentation";

		/// <summary>
		/// Gets or sets the description of the Afrowave Shared Tools.
		/// </summary>
		public string? Description { get; set; } = "# Documentation for Afrowave Shared Tools";

		/// <summary>
		/// Gets or sets the hostname or IP address of the server to connect to.
		/// </summary>
		public string Host { get; set; } = "localhost";

		/// <summary>
		/// Gets or sets the port number used for the connection.
		/// </summary>
		/// <remarks>Ensure the port number is within the valid range of 1 to 65535.  Commonly used for SMTP
		/// connections.</remarks>
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
		public string Email { get; set; } = "";

		/// <summary>
		/// Gets or sets the name of the sender to be displayed in communications.
		/// </summary>
		public string SenderName { get; set; } = "Afrowave Shared Tools";

		/// <summary>
		/// Gets or sets a value indicating whether authentication is required.
		/// </summary>
		public bool UseAuthentication { get; set; } = false;

		/// <summary>
		/// Gets or sets the login identifier for the user.
		/// </summary>
		public string? Login { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the password associated with the user or system.
		/// </summary>
		public string? Password { get; set; } = string.Empty;
	}
}
using MailKit.Security;

namespace Afrowave.SharedTools.Docs.Models.Dto
{
	/// <summary>
	/// Represents the configuration settings for an application, including email server details, authentication options,
	/// and general application metadata.
	/// </summary>
	/// <remarks>This class is typically used to store and transfer application-level settings, such as email server
	/// configuration, application metadata, and authentication details. It is designed to be a data transfer object (DTO)
	/// and does not include any behavior or logic.</remarks>
	public class ApplicationSettingsDto
	{
		/// <summary>
		/// Gets or sets the unique identifier for the entity.
		/// </summary>
		public int Id { get; set; } = 0;

		/// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		public string ApplicationName { get; set; } = "Afrowave.SharedTools.Docs";

		/// <summary>
		/// Gets or sets the description of the Afrowave Shared Tools.
		/// </summary>
		public string? Description { get; set; } = "Documentation for Afrowave Shared Tools";

		/// <summary>
		/// Gets or sets the host name or IP address of the server.
		/// </summary>
		public string SmtpHost { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the port number used for network communication.
		/// </summary>
		public int Port { get; set; } = 0;

		/// <summary>
		/// Gets or sets the API key used for authenticating requests.
		/// </summary>
		public string ApiKey { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the secure socket options for the connection.
		/// </summary>
		/// <remarks>This property determines the security settings used for the socket connection.  Valid values may
		/// depend on the specific implementation or context in which this property is used.</remarks>
		public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.Auto; // Assuming this is a string representation of SecureSocketOptions

		/// <summary>
		/// Gets or sets the email address associated with the user.
		/// </summary>
		public string? Email { get; set; }

		/// <summary>
		/// Gets or sets the name of the sender.
		/// </summary>
		public string? SenderName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether authentication is required for the operation.
		/// </summary>
		public bool UseAuthentication { get; set; }

		/// <summary>
		/// Gets or sets the login identifier for the user.
		/// </summary>
		public string? Login { get; set; } = null;

		/// <summary>
		/// Gets or sets the password associated with the user or system.
		/// </summary>
		public string? Password { get; set; } = null;

		/// <summary>
		/// Gets or sets a value indicating whether the test was successfully completed.
		/// </summary>
		public bool SuccessfullyTested { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the entity is active.
		/// </summary>
		public bool IsActive { get; set; } = false;
	}
}
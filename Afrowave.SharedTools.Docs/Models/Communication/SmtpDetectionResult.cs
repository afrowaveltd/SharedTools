using MailKit.Security;

namespace Afrowave.SharedTools.Docs.Models.Communication
{
	/// <summary>
	/// Represents the result of an SMTP detection operation.
	/// </summary>
	public class SmtpDetectionResult
	{
		/// <summary>
		/// Gets or sets a value indicating whether the detection was successful.
		/// </summary>
		public bool Successful { get; set; } = false;

		/// <summary>
		/// Gets or sets the detected SMTP host.
		/// </summary>
		public string Message { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the detected SMTP port.
		/// </summary>
		public int Port { get; set; } = 25;

		/// <summary>
		/// Gets or sets a value indicating whether the SMTP server requires authentication.
		/// </summary>
		public bool RequiresAuthentication { get; set; } = false;

		/// <summary>
		/// Gets or sets the secure socket options.
		/// </summary>
		public SecureSocketOptions Secure { get; set; } = SecureSocketOptions.None;
	}
}
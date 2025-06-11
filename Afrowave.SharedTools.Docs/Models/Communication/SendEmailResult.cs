namespace Afrowave.SharedTools.Docs.Models.Communication
{
	/// <summary>
	/// Represents the result of an email-sending operation, including details about the recipient,  success status, and
	/// any error information.
	/// </summary>
	public class SendEmailResult
	{
		/// <summary>
		/// The email address of the recipient.
		/// </summary>
		public string TargetEmail { get; set; } = string.Empty;

		/// <summary>
		/// Indicates whether the email was sent successfully.
		/// </summary>
		public bool Success { get; set; } = false;

		/// <summary>
		/// The subject of the email.
		/// </summary>
		public string Subject { get; set; } = string.Empty;

		/// <summary>
		/// The error message if the email was not sent successfully.
		/// </summary>
		public string? ErrorMessage { get; set; }
	}
}
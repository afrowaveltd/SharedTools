namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Defines methods for email-related operations, including SMTP settings detection, email sending, and SMTP
	/// configuration testing.
	/// </summary>
	/// <remarks>This interface provides functionality for detecting SMTP settings, sending emails, and testing SMTP
	/// configurations. Implementations of this interface are expected to handle the underlying email communication
	/// logic.</remarks>
	public interface IEmailService
	{
		/// <summary>
		/// Attempts to automatically detect the SMTP settings for the specified email domain.
		/// </summary>
		/// <remarks>This method performs a series of network operations to infer the SMTP server settings  for the
		/// provided email domain. The detection process may involve DNS lookups,  common provider heuristics, and other
		/// techniques. <para> The operation is asynchronous and may take some time to complete depending on network
		/// conditions. </para></remarks>
		/// <param name="input">An object containing the email address and optional configuration parameters used for detection. Cannot be null.</param>
		/// <returns>A <see cref="SmtpDetectionResult"/> object containing the detected SMTP settings,  or information about why
		/// detection failed.</returns>
		Task<SmtpDetectionResult> AutodetectSmtpSettingsAsync(DetectSmtpSettingsInput input);

		/// <summary>
		/// Sends an email asynchronously to the specified recipient with the given subject and body.
		/// </summary>
		/// <remarks>The method performs the email sending operation asynchronously. Ensure that the provided email
		/// address is valid and properly formatted.</remarks>
		/// <param name="target">The email address of the recipient. This cannot be null or empty.</param>
		/// <param name="subject">The subject of the email. This cannot be null or empty.</param>
		/// <param name="body">The body content of the email. This cannot be null or empty.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="SendEmailResult"/>
		/// indicating the outcome of the email sending operation.</returns>
		Task<SendEmailResult> SendEmailAsync(string target, string subject, string body);

		/// <summary>
		/// Tests the provided SMTP settings by attempting to send a test email.
		/// </summary>
		/// <remarks>This method performs a non-persistent test to validate the SMTP configuration. It does not send
		/// an actual email to a recipient but verifies the ability to connect and authenticate with the SMTP
		/// server.</remarks>
		/// <param name="input">An <see cref="SmtpSenderModel"/> containing the SMTP configuration to test, including server address, port,
		/// credentials, and other settings.</param>
		/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The result contains an <see
		/// cref="SmtpTestResult"/> indicating whether the test was successful and any associated error details.</returns>
		Task<SmtpTestResult> TestSmtpSettingsAsync(SmtpSenderModel input);
	}
}
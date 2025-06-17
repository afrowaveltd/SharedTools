using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Provides functionality for sending emails, testing SMTP settings, and detecting SMTP server configurations.
	/// </summary>
	/// <remarks>This service is designed to handle email-related operations, including sending emails, validating
	/// SMTP settings, and automatically detecting SMTP server configurations. It relies on injected dependencies for
	/// logging, localization, and application settings. The service supports asynchronous operations and ensures proper
	/// resource management during SMTP interactions.</remarks>
	/// <remarks>
	/// Initializes a new instance of the <see cref="EmailService"/> class with the specified logger, localizer, and
	/// settings service.
	/// </remarks>
	/// <remarks>This constructor initializes the email service with the provided dependencies and retrieves the
	/// SMTP settings from the settings service. If no SMTP settings are available, default settings are used.</remarks>
	/// <param name="logger">The logger used to log messages related to email operations.</param>
	/// <param name="localizer">The localizer used for retrieving localized strings for email content or messages.</param>
	/// <param name="setting"></param>
	/// <param name="installation"></param>

	public class EmailService(ILogger<EmailService> logger, IStringLocalizer<EmailService> localizer, ISettingsService setting, IInstallationService installation) : IEmailService
	{
		private readonly ILogger<EmailService> _logger = logger;
		private readonly IStringLocalizer<EmailService> _localizer = localizer;
		private readonly IInstallationService _installation = installation;
		private readonly SmtpSettings smtpSettings = setting.GetSmtpSettingsAsync().Result.Data ?? new SmtpSettings();

		/// <summary>
		/// Attempts to automatically detect the SMTP settings for a given host and credentials.
		/// </summary>
		/// <remarks>This method tests multiple common port and security combinations to determine the correct SMTP
		/// settings for the specified host.  If the server requires authentication and no credentials are provided, the
		/// detection will fail with an appropriate message.</remarks>
		/// <param name="input">An object containing the host, username, and password to use for detection. The <paramref name="input"/> cannot be
		/// null.</param>
		/// <returns>A <see cref="SmtpDetectionResult"/> object containing the detection result.  If successful, the result includes
		/// the detected port, security options, and a success message.  If unsuccessful, the result includes an error
		/// message.</returns>
		public async Task<SmtpDetectionResult> AutodetectSmtpSettingsAsync(DetectSmtpSettingsInput input)
		{
			List<(int Port, SecureSocketOptions Security)> portSecurityCombinations =
			[
				(25, SecureSocketOptions.None),
		(587, SecureSocketOptions.StartTls),
		(465, SecureSocketOptions.SslOnConnect),
		(25, SecureSocketOptions.Auto),
		(587, SecureSocketOptions.Auto),
		(465, SecureSocketOptions.Auto),
		(2525, SecureSocketOptions.Auto) // failback
			];

			SmtpDetectionResult response = new();

			var tasks = portSecurityCombinations.Select(async combination =>
			{
				try
				{
					using SmtpClient client = new();
					await client.ConnectAsync(input.Host, combination.Port, combination.Security);

					if(client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
					{
						if(string.IsNullOrEmpty(input.Username) || string.IsNullOrEmpty(input.Password))
						{
							return new SmtpDetectionResult
							{
								Successful = false,
								Message = "Server requires authentication, but no credentials provided"
							};
						}
						await client.AuthenticateAsync(input.Username, input.Password);
					}

					await client.DisconnectAsync(true);
					_logger.LogInformation("SMTP settings detected successfully: {Host}, Port: {Port}, Security: {Security}", input.Host, combination.Port, combination.Security);
					return new SmtpDetectionResult
					{
						Successful = true,
						Port = combination.Port,
						Secure = combination.Security,
						Message = "SMTP settings successfully detected"
					};
				}
				catch
				{
					_logger.LogWarning("Failed to detect SMTP settings for {Host} on Port: {Port}, Security: {Security}", input.Host, combination.Port, combination.Security);
					return null;
				}
			});

			var completedTask = await Task.WhenAny(tasks);
			return completedTask.Result ?? new SmtpDetectionResult { Successful = false, Message = "SMTP settings not found" };
		}

		/// <summary>
		/// Sends an email asynchronously to the specified target email address with the given subject and body.
		/// </summary>
		/// <remarks>This method checks several preconditions before attempting to send the email: <list
		/// type="bullet"> <item>If the application is not installed, the email will not be sent, and an error message will be
		/// returned.</item> <item>If the <paramref name="target"/>, <paramref name="subject"/>, or <paramref name="body"/> is
		/// null or empty,  the email will not be sent, and an appropriate error message will be returned.</item> </list> The
		/// email is sent using the configured SMTP settings, and the body is formatted as HTML.</remarks>
		/// <param name="target">The recipient's email address. Cannot be null or empty.</param>
		/// <param name="subject">The subject of the email. Cannot be null or empty.</param>
		/// <param name="body">The body content of the email. Cannot be null or empty.</param>
		/// <returns>A <see cref="SendEmailResult"/> object containing the result of the email sending operation,  including success
		/// status, error messages (if any), and the target email and subject.</returns>
		public async Task<SendEmailResult> SendEmailAsync(string target, string subject, string body)
		{
			if(!await _installation.IsInstalledAsync())
			{
				_logger.LogWarning("Application is not installed, cannot send email.");
				return new SendEmailResult
				{
					ErrorMessage = _localizer["Application is not installed"],
					Success = false,
					TargetEmail = target,
					Subject = subject
				};
			}

			if(string.IsNullOrEmpty(target))
			{
				_logger.LogWarning("Target email is empty, cannot send email.");
				return new SendEmailResult
				{
					ErrorMessage = _localizer["Target email is empty"],
					Success = false,
					TargetEmail = target,
					Subject = subject
				};
			}
			if(string.IsNullOrEmpty(subject))
			{
				_logger.LogError("Subject is empty");
				return new SendEmailResult
				{
					TargetEmail = target,
					Subject = subject,
					Success = false,
					ErrorMessage = _localizer["Subject is empty"]
				};
			}
			if(string.IsNullOrEmpty(body))
			{
				_logger.LogError("Body is empty");
				return new SendEmailResult
				{
					TargetEmail = target,
					Subject = subject,
					Success = false,
					ErrorMessage = _localizer["Body is empty"]
				};
			}
			MimeMessage message = new();
			message.From.Add(new MailboxAddress(smtpSettings.SenderName, smtpSettings.Email));
			message.To.Add(new MailboxAddress("", target));
			message.Subject = subject;
			message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = body
			};

			return await SendEmailAsync(message);
		}

		/// <summary>
		/// Tests the provided SMTP settings by attempting to send a test email.
		/// </summary>
		/// <remarks>This method validates the provided SMTP settings by connecting to the SMTP server, optionally
		/// authenticating, and sending a test email to the specified target address. If the test fails, the result will
		/// include an error message and the log of the SMTP communication for debugging purposes. <para> The <paramref
		/// name="input"/> parameter must include a valid host, port, and sender email. If authentication is required, a
		/// username and password must also be provided. </para></remarks>
		/// <param name="input">The SMTP settings to test, including host, port, sender email, and optional authentication details.</param>
		/// <returns>A <see cref="SmtpTestResult"/> object containing the result of the test, including whether the test was
		/// successful, any error messages, and the log of the SMTP communication.</returns>
		public async Task<SmtpTestResult> TestSmtpSettingsAsync(SmtpSenderModel input)
		{
			string targetEmail = input.TargetForTesting ?? input.SenderEmail;
			SmtpTestResult result = new();

			if(string.IsNullOrEmpty(input.Host))
			{
				result.Error = "Host is empty";
				return result;
			}
			if(input.Port == 0)
			{
				result.Error = "Port is empty";
				return result;
			}
			if(string.IsNullOrEmpty(input.SenderEmail))
			{
				result.Error = "Sender email is empty";
				return result;
			}
			if(string.IsNullOrEmpty(input.SenderName))
			{
				input.SenderName = input.SenderEmail;
			}

			string logPath = await CreateTemporarySmtpLogFile();

			try
			{
				using SmtpClient client = new SmtpClient(new ProtocolLogger(logPath));
				await client.ConnectAsync(input.Host, input.Port ?? 25, input.Secure);

				if(input.AuthorizationRequired)
				{
					if(string.IsNullOrEmpty(input.Username) || string.IsNullOrEmpty(input.Password))
					{
						result.Error = "Server requires authentication, but no credentials provided";
						return result;
					}
					await client.AuthenticateAsync(input.Username, input.Password);
				}

				MimeMessage message = new MimeMessage();
				message.From.Add(new MailboxAddress(input.SenderName, input.SenderEmail));
				message.To.Add(new MailboxAddress("", targetEmail));
				message.Subject = _localizer["SMTP test"];
				message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
				{
					Text = _localizer["SMTP test email"]
				};

				_ = await client.SendAsync(message);
				await client.DisconnectAsync(true);

				result.Success = true;
			}
			catch(Exception ex)
			{
				result.Success = false;
				result.Error = ex.Message;
			}
			finally
			{
				result.Log = await GetLogAndDisposeFileAsync(logPath);
			}

			return result;
		}

		// private methods
		private async Task<SendEmailResult> SendEmailAsync(MimeMessage message)
		{
			SendEmailResult result = new()
			{
				Subject = message.Subject,
				TargetEmail = message.To.ToString()
			};
			try
			{
				using SmtpClient client = new();
				await client.ConnectAsync(smtpSettings.Host, smtpSettings.Port, smtpSettings.SecureSocketOptions);
				if(smtpSettings.UseAuthentication)
				{
					await client.AuthenticateAsync(smtpSettings.Login, smtpSettings.Password);
				}
				_ = await client.SendAsync(message);
				_logger.LogInformation("Email sent successfully to {TargetEmail} with subject: {Subject}", result.TargetEmail, result.Subject);
				await client.DisconnectAsync(true);
				result.Success = true;
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Error sending email");
				result.ErrorMessage = _localizer["Error sending email"];
			}
			return result;
		}

		private static async Task<string> CreateTemporarySmtpLogFile()
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			await File.WriteAllTextAsync(path, "");
			return path;
		}

		private static async Task<string> GetLogAndDisposeFileAsync(string path)
		{
			string log = await File.ReadAllTextAsync(path);
			File.Delete(path);
			return log;
		}
	}
}
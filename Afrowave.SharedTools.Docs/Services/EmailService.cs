using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace Afrowave.SharedTools.Docs.Services
{
	public class EmailService
	{
		private readonly ILogger<EmailService> _logger;
		private readonly IStringLocalizer<EmailService> _localizer;
		private readonly ISettingsService _settings;
		private readonly SmtpSettings _smtpSettings;

		public EmailService(ILogger<EmailService> logger, IStringLocalizer<EmailService> localizer, ISettingsService settings)
		{
			_logger = logger;
			_localizer = localizer;
			_settings = settings;
			_smtpSettings = _settings.GetSmtpSettingsAsync().Result.Data ?? new SmtpSettings();
		}

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
		private async Task<SendEmailResult> SendEmailAsync(MimeMessage message, SmtpSettings settings)
		{
			SendEmailResult result = new()
			{
				Subject = message.Subject,
				TargetEmail = message.To.ToString()
			};
			try
			{
				using SmtpClient client = new();
				await client.ConnectAsync(settings.Host, settings.Port, settings.SecureSocketOptions);
				if(settings.UseAuthentication)
				{
					await client.AuthenticateAsync(settings.Login, settings.Password);
				}
				_ = await client.SendAsync(message);
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
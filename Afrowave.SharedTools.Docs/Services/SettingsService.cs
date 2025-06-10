namespace Afrowave.SharedTools.Docs.Services;

public class SettingsService(DocsDbContext context,
	IStringLocalizer<SettingsService> localizer,
	ILogger<SettingsService> logger,
	IEncryptionService encryption,
	IInstallationService installation)
{
	private readonly DocsDbContext _context = context;
	private readonly IStringLocalizer<SettingsService> _localizer = localizer;
	private readonly ILogger<SettingsService> _logger = logger;
	private readonly IEncryptionService _encryption = encryption;
	private readonly IInstallationService _installation = installation;

	public async Task<Response<SmtpSettings>> GetSmtpSettingsAsync()
	{
		SmtpSettings settings = new();
		try
		{
			var appSettings = await _context.ApplicationSettings.FirstOrDefaultAsync();
			if(appSettings == null)
			{
				return Response<SmtpSettings>.Fail(_localizer["No settings found."]);
			}
			settings.Host = appSettings.SmtpHost;
			settings.Port = appSettings.Port;
			settings.SecureSocketOptions = appSettings.SecureSocketOptions;
			settings.Email = appSettings.Email;
			settings.SenderName = appSettings.SenderName;
			settings.UseAuthentication = appSettings.UseAuthentication;
			settings.Login = appSettings.Login;
			settings.Password = _encryption.DecryptTextAsync(appSettings.EncryptedPassword ?? string.Empty, appSettings.ApiKey);
			settings.SuccessfullyTested = appSettings.SuccessfullyTested;
			return Response<SmtpSettings>.Successful(settings, "");
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "Error retrieving SMTP settings.");
			return Response<SmtpSettings>.Fail(_localizer["An error occurred while retrieving SMTP settings."]);
		}
	}

	private async Task<Response<ApplicationSettings>> SaveRawSettings(ApplicationSettingsDto settings)
	{
		if(settings == null)
		{
			return Response<ApplicationSettings>.Fail(_localizer["Settings cannot be null."]);
		}
		ApplicationSettings? actualSettings = await _context.ApplicationSettings.FirstOrDefaultAsync();
		if(actualSettings == null)
		{
			return Response<ApplicationSettings>.Fail(_localizer["No settings found"]);
		}

		// actualize each value
		actualSettings.ApplicationName = settings.ApplicationName;
		actualSettings.SenderName = settings.SenderName ?? string.Empty;
		actualSettings.Description = settings.Description;
		actualSettings.SmtpHost = settings.SmtpHost;
		actualSettings.Port = settings.Port;
		actualSettings.ApiKey = settings.ApiKey;
		actualSettings.SecureSocketOptions = settings.SecureSocketOptions;
		actualSettings.Email = settings.Email ?? string.Empty;
		actualSettings.SenderName = settings.SenderName ?? string.Empty;
		actualSettings.UseAuthentication = settings.UseAuthentication;
		actualSettings.Login = settings.Login ?? string.Empty;
		actualSettings.EncryptedPassword = settings.Password ?? string.Empty;
		actualSettings.SuccessfullyTested = settings.SuccessfullyTested;
		actualSettings.IsActive = settings.IsActive;

		try
		{
			await _context.SaveChangesAsync();
			_logger.LogInformation("Application settings updated");
			return Response<ApplicationSettings>.Successful(actualSettings, "");
		}
		catch(Exception ex)
		{
			_logger.LogError("Error saving settings {ex}", ex);
			return Response<ApplicationSettings>.Fail(ex);
		}
	}

	private async Task<Response<ApplicationSettings>> LoadRawSettings()
	{
		try
		{
			var settings = await _context.ApplicationSettings.FirstOrDefaultAsync();
			if(settings == null)
			{
				return Response<ApplicationSettings>.Fail(_localizer["No settings found."]);
			}
			return Response<ApplicationSettings>.Successful(settings, "");
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "Error loading application settings.");
			return Response<ApplicationSettings>.Fail(_localizer["An error occurred while loading application settings."]);
		}
	}

	private async Task<bool> InServiceAsync()
	{
		try
		{
			return await _installation.IsInstalledAsync();
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "Error checking if settings exist.");
			return false;
		}
	}
}
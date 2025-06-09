using Afrowave.SharedTools.Docs.Models.Communication;
using Afrowave.SharedTools.Docs.Models.DatabaseModels;

namespace Afrowave.SharedTools.Docs.Services;

public class SettingsService(DocsDbContext context,
	IStringLocalizer<SettingsService> localizer,
	ILogger<SettingsService> logger,
	IEncryptionService encryption)
{
	private readonly DocsDbContext _context = context;
	private readonly IStringLocalizer<SettingsService> _localizer = localizer;
	private readonly ILogger<SettingsService> _logger = logger;
	private readonly IEncryptionService _encryption = encryption;

	public async Task<Response<ApplicationSettingsDto>> GetApplicationSettingsAsync()
	{
		try
		{
			var settings = await _context.ApplicationSettings.FirstOrDefaultAsync();
			if(settings == null)
			{
				return Response<ApplicationSettingsDto>.SuccessWithWarning(new(), _localizer["No settings found."]);
			}
			var dto = new ApplicationSettingsDto
			{
				Id = settings.Id,
				ApplicationName = settings.ApplicationName,
				Description = settings.Description,
				Host = settings.Host,
				Port = settings.Port,
				ApiKey = settings.ApiKey,
				SecureSocketOptions = settings.SecureSocketOptions,
				Email = settings.Email,
				SenderName = settings.SenderName,
				UseAuthentication = settings.UseAuthentication,
				Login = settings.Login,
				Password = _encryption.DecryptTextAsync(settings.EncryptedPasswoord ?? string.Empty, settings.ApiKey),
				SuccessfullyTested = settings.SuccessfullyTested,
				IsActive = settings.IsActive
			};
			return Response<ApplicationSettingsDto>.Successful(dto, "");
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "Error retrieving application settings.");
			return Response<ApplicationSettingsDto>.Fail(_localizer["An error occurred while retrieving application settings."]);
		}
	}

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
			settings.Host = appSettings.Host;
			settings.Port = appSettings.Port;
			settings.SecureSocketOptions = appSettings.SecureSocketOptions;
			settings.Email = appSettings.Email;
			settings.SenderName = appSettings.SenderName;
			settings.UseAuthentication = appSettings.UseAuthentication;
			settings.Login = appSettings.Login;
			settings.Password = _encryption.DecryptTextAsync(appSettings.EncryptedPasswoord ?? string.Empty, appSettings.ApiKey);
			settings.SuccessfullyTested = appSettings.SuccessfullyTested;
			return Response<SmtpSettings>.Successful(settings, "");
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "Error retrieving SMTP settings.");
			return Response<SmtpSettings>.Fail(_localizer["An error occurred while retrieving SMTP settings."]);
		}
	}

	public async Task<Response<ApplicationSettings>> SaveApplicationSettingsAsync(ApplicationSettingsDto settingsDto)
	{
	}

	private async Task<Response<ApplicationSettings>> SaveRawSettings(ApplicationSettings settings)
	{
		bool isNew = !await SettingsExists();
		if(settings == null)
		{
			return Response<ApplicationSettings>.Fail(_localizer["Settings cannot be null."]);
		}
		if(isNew)
		{
			// it means, that we are installing the application and need to consider it OK
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

	private async Task<bool> SettingsExists()
	{
		try
		{
			return await _context.ApplicationSettings.AnyAsync();
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "Error checking if settings exist.");
			return false;
		}
	}
}
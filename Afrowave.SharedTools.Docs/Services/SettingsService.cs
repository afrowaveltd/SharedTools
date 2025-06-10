namespace Afrowave.SharedTools.Docs.Services;

/// <summary>
/// Provides functionality for managing application settings, including API keys, SMTP settings, and application
/// information. This service interacts with the database to retrieve, update, and save settings, and ensures proper
/// encryption and validation where necessary.
/// </summary>
/// <remarks>This service is designed to handle application-level settings, such as API keys and SMTP
/// configurations, and provides methods for retrieving and updating these settings. It also integrates with encryption
/// and installation services to ensure secure and consistent behavior.</remarks>
/// <param name="context"></param>
/// <param name="localizer"></param>
/// <param name="logger"></param>
/// <param name="encryption"></param>
/// <param name="installation"></param>
public class SettingsService(DocsDbContext context,
	IStringLocalizer<SettingsService> localizer,
	ILogger<SettingsService> logger,
	IEncryptionService encryption,
	IInstallationService installation) : ISettingsService
{
	private readonly DocsDbContext _context = context;
	private readonly IStringLocalizer<SettingsService> _localizer = localizer;
	private readonly ILogger<SettingsService> _logger = logger;
	private readonly IEncryptionService _encryption = encryption;
	private readonly IInstallationService _installation = installation;

	/// <summary>
	/// Asynchronously retrieves the API key from the application settings.
	/// </summary>
	/// <remarks>The returned API key is retrieved from the application's settings. If the key is not found,  the
	/// method returns a failure response with an appropriate error message.</remarks>
	/// <returns>A <see cref="Response{T}"/> object containing the API key as a string if successful;  otherwise, a failure response
	/// with an error message.</returns>
	public async Task<Response<string>> GetApiKeyAsync()
	{
		var apiKey = await _context.ApplicationSettings.Select(s => s.ApiKey).FirstOrDefaultAsync();
		if(apiKey == null)
		{
			return Response<string>.Fail(_localizer["An error occured while retrieving the Api key"]);
		}
		return Response<string>.Successful(apiKey.ToString() ?? string.Empty);
	}

	/// <summary>
	/// Asynchronously retrieves application information, including the application name and description.
	/// </summary>
	/// <remarks>This method queries the application settings to retrieve the relevant information.  If no settings
	/// are found, a default <see cref="ApplicationInfo"/> object is returned.</remarks>
	/// <returns>A <see cref="Response{T}"/> containing the application information if successful,  or an error message if the
	/// information could not be retrieved.</returns>
	public async Task<Response<ApplicationInfo>> GetApplicationInfoAsync()
	{
		ApplicationInfo info = await _context.ApplicationSettings.Select(s =>
		new ApplicationInfo
		{
			ApplicationName = s.ApplicationName,
			Description = s.Description
		}).FirstOrDefaultAsync() ?? new();
		if(info == null)
		{
			return Response<ApplicationInfo>.Fail("Application informations couldn't be retreived");
		}
		return Response<ApplicationInfo>.Successful(info);
	}

	/// <summary>
	/// Asynchronously retrieves the SMTP settings configured in the application.
	/// </summary>
	/// <remarks>This method fetches the SMTP settings from the application's data store and returns them as a <see
	/// cref="SmtpSettings"/> object.  If no settings are found, or if an error occurs during retrieval, an appropriate
	/// failure response is returned.</remarks>
	/// <returns>A <see cref="Response{T}"/> containing the SMTP settings if successfully retrieved; otherwise, a failure response
	/// with an error message.</returns>
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

	/// <summary>
	/// Resets the API key for the application and returns the newly generated key.
	/// </summary>
	/// <remarks>This method generates a new API key, updates the application settings, and saves the changes to the
	/// database. If no application settings are found, the operation fails. If an error occurs during the save operation,
	/// the method returns a failure response with the exception details.</remarks>
	/// <returns>A <see cref="Response{T}"/> containing the newly generated API key if the operation is successful. If the operation
	/// fails, the response contains an error message or exception details.</returns>
	public async Task<Response<string>> ResetApiKeyAsync()
	{
		var actualSettings = await _context.ApplicationSettings.FirstOrDefaultAsync();
		if(actualSettings == null)
			return Response<string>.Fail(_localizer["No settings found"]);
		actualSettings.ApiKey = _encryption.GenerateApplicationSecret();
		try
		{
			await _context.SaveChangesAsync();
			return Response<string>.Successful(actualSettings.ApiKey);
		}
		catch(Exception ex)
		{
			return Response<string>.Fail(ex);
		}
	}

	/// <summary>
	/// Saves the specified SMTP settings asynchronously and updates the application's configuration.
	/// </summary>
	/// <remarks>This method validates and encrypts sensitive information, such as the SMTP password, before saving
	/// the settings.  It ensures that the application's configuration is updated with the provided SMTP details.  If the
	/// operation fails, an appropriate error message is returned in the response.</remarks>
	/// <param name="settings">The <see cref="SmtpSettings"/> object containing the SMTP configuration to be saved.  This includes properties such
	/// as the host, port, authentication details, and sender information.</param>
	/// <returns>A <see cref="Response{T}"/> object containing the updated <see cref="SmtpSettings"/> if the operation succeeds.  If
	/// the operation fails, the response will include an error message.</returns>
	public async Task<Response<SmtpSettings>> SaveSmtpSettingsAsync(SmtpSettings settings)
	{
		var applicationSettings = await LoadRawSettings();
		if(!applicationSettings.Success)
		{
			return Response<SmtpSettings>.Fail(_localizer["An error occurred while retrieving SMTP settings."]);
		}
		if(applicationSettings.Data == null)
		{
			return Response<SmtpSettings>.Fail(_localizer["An error occurred while retrieving SMTP settings."]);
		}

		var data = applicationSettings.Data;
		var apiKey = data.ApiKey;

		data.SmtpHost = settings.Host;
		data.Port = settings.Port;
		data.SecureSocketOptions = settings.SecureSocketOptions;
		data.Email = settings.Email;
		data.SenderName = settings.SenderName;
		data.UseAuthentication = settings.UseAuthentication;
		data.Login = settings.Login;
		data.EncryptedPassword = _encryption.EncryptTextAsync(settings.Password ?? string.Empty, apiKey);
		var finalResponse = await SaveRawSettings(data);
		if(!finalResponse.Success)
			return Response<SmtpSettings>.Fail(finalResponse.Message ?? _localizer["An error occurred while retrieving SMTP settings."]);

		SmtpSettings smtpSettings = new()
		{
			Host = data.SmtpHost,
			Port = data.Port,
			SecureSocketOptions = data.SecureSocketOptions,
			Email = data.Email,
			SenderName = data.SenderName,
			UseAuthentication = data.UseAuthentication,
			Login = data.Login,
			Password = settings.Password,
			SuccessfullyTested = data.SuccessfullyTested,
		};
		return Response<SmtpSettings>.Successful(smtpSettings);
	}

	/// <summary>
	/// Updates the application information with the provided details.
	/// </summary>
	/// <remarks>This method validates the provided application information and updates the stored settings
	/// accordingly.  Ensure that the service is ready before calling this method, as it will fail if the service is not
	/// yet initialized.</remarks>
	/// <param name="info">The <see cref="ApplicationInfo"/> object containing the updated application details.  The <see
	/// cref="ApplicationInfo.ApplicationName"/> property must not be null, empty, or whitespace.</param>
	/// <returns>A <see cref="Response{T}"/> object containing the updated <see cref="ApplicationInfo"/> if the operation is
	/// successful.  If the service is not ready or the input is invalid, the response will indicate failure with an
	/// appropriate error message.</returns>
	public async Task<Response<ApplicationInfo>> UpdateApplicationInformationsAsync(ApplicationInfo info)
	{
		if(!await InServiceAsync())
		{
			return Response<ApplicationInfo>.Fail("Service is not yet ready");
		}

		if(info == null)
			return Response<ApplicationInfo>.Fail(_localizer["Settings cannot be null."]);
		if(string.IsNullOrWhiteSpace(info.ApplicationName))
			return Response<ApplicationInfo>.Fail(_localizer["The application name can't be empty"]);
		var settings = await LoadRawSettings();
		var data = settings.Data ?? new();
		data.ApplicationName = info.ApplicationName;
		data.Description = info.Description;
		await _context.SaveChangesAsync();
		return Response<ApplicationInfo>.Successful(info);
	}

	private async Task<Response<ApplicationSettings>> SaveRawSettings(ApplicationSettings settings)
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
		actualSettings.EncryptedPassword = _encryption.EncryptTextAsync(settings.EncryptedPassword ?? string.Empty, settings.ApiKey);
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
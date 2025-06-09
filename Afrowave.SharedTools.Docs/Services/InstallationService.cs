namespace Afrowave.SharedTools.Docs.Services;

/// <summary>
/// Provides functionality for managing the installation process of the application, including determining installation
/// eligibility and system state.
/// </summary>
/// <remarks>The <see cref="InstallationService"/> class is responsible for evaluating whether the application or
/// administrative components can be installed, as well as checking the current installation state. It interacts with
/// the underlying database context to retrieve and verify system settings and user data.</remarks>
/// <param name="logger"></param>
/// <param name="localizer"></param>
/// <param name="encryption"></param>
/// <param name="context"></param>
public class InstallationService(ILogger<InstallationService> logger,
	IStringLocalizer<InstallationService> localizer,
	IEncryptionService encryption,
	DocsDbContext context)
{
	private readonly ILogger<InstallationService> _logger = logger;
	private readonly IStringLocalizer<InstallationService> _localizer = localizer;
	private readonly IEncryptionService _encryption = encryption;
	private readonly DocsDbContext _context = context;

	public async Task<Response<InstallationResult>> InstallApplication(ApplicationInstall application)
	{
		InstallationResult result = new InstallationResult();
		if(await IsInstalledAsync())
		{
			return Response<InstallationResult>.Fail("Settings already exist");
		}
		// just simple checks
		if(application == null)
		{
			return Response<InstallationResult>.Fail("Missing data");
		}
		if(string.IsNullOrWhiteSpace(application.Email)
			|| string.IsNullOrWhiteSpace(application.SmtpHost))
		{
			return Response<InstallationResult>.Fail("Email and Host are required");
		}

		if(application.SmtpPort < 1 || application.SmtpPort > 65535)
		{
			return Response<InstallationResult>.Fail("SMTP Port must be between 1 and 65535");
		}

		if(!Validators.IsEmail(application.Email))
		{
			return Response<InstallationResult>.Fail("Email is not valid");
		}

		if(string.IsNullOrWhiteSpace(application.SenderEmail))
		{
			if(string.IsNullOrWhiteSpace(application.SmtpLogin))
			{
				return Response<InstallationResult>.Fail("Sender email is required");
			}
			application.SenderEmail = application.SmtpLogin;
		}
		if(application.UseAuthentication && string.IsNullOrWhiteSpace(application.SmtpLogin))
		{
			return Response<InstallationResult>.Fail("SMTP Login is required when authentication is used");
		}
		if(application.UseAuthentication && string.IsNullOrWhiteSpace(application.SmtpPassword))
		{
			return Response<InstallationResult>.Fail("SMTP Password is required when authentication is used");
		}
		string apiKey = _encryption.GenerateApplicationSecret();
		ApplicationSettings settings = new()
		{
			ApplicationName = application.ApplicationName ?? "Documentation",
			Description = application.Description,
			SmtpHost = application.SmtpHost,
			Port = application.SmtpPort,
			ApiKey = apiKey,
			SecureSocketOptions = application.SecureSocketOptions,
			Email = application.SenderEmail ?? application.SmtpLogin ?? string.Empty,
			SenderName = application.SenderName ?? application.SmtpLogin ?? string.Empty,
			UseAuthentication = application.UseAuthentication,
			Login = application.SmtpLogin ?? application.SenderEmail ?? string.Empty,
			EncryptedPassword = _encryption.EncryptTextAsync(application.SmtpPassword ?? string.Empty, apiKey)
		};
		try
		{
			await _context.ApplicationSettings.AddAsync(settings);
			await _context.SaveChangesAsync();
			result.ApplicationSettings = settings;
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "Failed to save application settings: {Message}", ex.Message);
			return Response<InstallationResult>.Fail("Failed to save application settings");
		}

		// all ready to save settings;

		_logger.LogInformation("Application installation settings prepared for: {ApplicationName}", application.ApplicationName);
	}

	/// <summary>
	/// Determines whether the application is currently installed.
	/// </summary>
	/// <remarks>This method evaluates the installation state of the application by checking specific conditions. It
	/// performs asynchronous operations to determine whether the application can be installed in administrative or
	/// application-specific contexts.</remarks>
	/// <returns><see langword="true"/> if the application is installed; otherwise, <see langword="false"/>.</returns>
	public async Task<bool> IsInstalledAsync()
	{
		return !(await CanInstallAdminAsync() && await CanInstallApplicationAsync());
	}

	private async Task<bool> CanInstallApplicationAsync()
	{
		// Check if the application is already installed
		var isInstalled = await _context.ApplicationSettings.AnyAsync();
		if(isInstalled)
		{
			_logger.LogInformation("Application is already installed.");
			return false;
		}
		_logger.LogInformation("Application can be installed.");
		return true;
	}

	private async Task<bool> CanInstallAdminAsync()
	{
		// Check if the admin user is already created
		var hasAdmin = await _context.Admins.AnyAsync();
		if(hasAdmin)
		{
			_logger.LogInformation("Admin user already exists.");
			return false;
		}
		_logger.LogInformation("Admin user can be created.");
		return true;
	}

	// Add methods for installation service functionality here
}
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
	DocsDbContext context) : IInstallationService
{
	private readonly ILogger<InstallationService> _logger = logger;
	private readonly IStringLocalizer<InstallationService> _localizer = localizer;
	private readonly IEncryptionService _encryption = encryption;
	private readonly DocsDbContext _context = context;

	/// <summary>
	/// Installs the application by validating the provided configuration, saving application settings,  and creating an
	/// admin user.
	/// </summary>
	/// <remarks>This method performs the following steps: <list type="bullet"> <item>Validates the provided
	/// <paramref name="application"/> object, ensuring all required fields are populated and valid.</item> <item>Checks if
	/// the application is already installed and prevents duplicate installations.</item> <item>Saves the application
	/// settings to the database, including encryption of sensitive data.</item> <item>Creates an admin user with the
	/// provided email and optional display name.</item> </list> If any validation or database operation fails, the method
	/// returns a failure response with an appropriate error message.</remarks>
	/// <param name="application">The <see cref="ApplicationInstall"/> object containing the configuration details required for the installation.</param>
	/// <returns>A <see cref="Response{T}"/> containing an <see cref="InstallationResult"/> object if the installation is
	/// successful,  or an error message if the installation fails.</returns>
	public async Task<Response<InstallationResult>> InstallApplication(ApplicationInstall application)
	{
		InstallationResult result = new();
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
			EncryptedPassword = _encryption.EncryptTextAsync(application.SmtpPassword ?? string.Empty, apiKey),
			SuccessfullyTested = true,
			IsActive = true
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

		// all ready to save admin;
		Admin admin = new()
		{
			Email = application.Email,
			DisplayName = application.DisplayName ?? "Admin",
			IsActive = true,
			Role = Role.Owner,
			Bearer = _encryption.GenerateApplicationSecret()
		};
		try
		{
			await _context.Admins.AddAsync(admin);
			await _context.SaveChangesAsync();
			result.Admin = admin;
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "Failed to save admin user: {Message}", ex.Message);
			return Response<InstallationResult>.Fail("Failed to save admin user");
		}
		_logger.LogInformation("Application installation settings prepared for: {ApplicationName}", application.ApplicationName);
		return Response<InstallationResult>.Successful(result, _localizer["Application installation settings prepared successfully."]);
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
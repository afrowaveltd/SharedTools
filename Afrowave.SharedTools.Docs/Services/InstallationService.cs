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
		if(await IsInstalledAsync())
		{
			return Response<InstallationResult>.Fail("Settings already exist");
		}
		// just simple checks
		if(application == null)
		{
			return Response<InstallationResult>.Fail("Missing data");
		}
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
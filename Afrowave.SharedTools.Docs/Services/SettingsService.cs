namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Provides methods for managing application settings stored in the database.
	/// </summary>
	/// <remarks>This service allows for checking the installation status, saving, loading, and verifying the
	/// existence of application settings. It interacts with the database to persist and retrieve settings, ensuring
	/// consistency and validation of the data.</remarks>
	/// <param name="context">
	/// The database context for accessing settings.
	/// </param>
	/// <param name="logger">
	/// The logger for logging service-related activities.
	/// </param>
	public class SettingsService(DocsDbContext context, ILogger<SettingsService> logger) : ISettingsService
	{
		private readonly DocsDbContext _context = context;
		private readonly ILogger<SettingsService> _logger = logger;

		/// <summary>
		/// Determines whether any document settings are currently installed in the context.
		/// </summary>
		/// <remarks>This method asynchronously checks the underlying data source to determine if any document
		/// settings exist.</remarks>
		/// <returns><see langword="true"/> if document settings are installed; otherwise, <see langword="false"/>.</returns>
		public async Task<bool> IsInstalledAsync()
			=> await _context.ApplicationSettings.AnyAsync();

		/// <summary>
		/// Saves the provided settings to the database asynchronously.
		/// </summary>
		/// <remarks>This method performs the following actions: <list type="bullet"> <item> If no settings exist in
		/// the database, it creates a new record using the provided <paramref name="settings"/>.  The <see
		/// cref="DocsSettings.Name"/> property must be provided in this case. </item> <item> If settings already exist, it
		/// updates the existing record with the values from the provided <paramref name="settings"/>  if the application name
		/// matches the stored settings. </item> <item> If the application name in the provided <paramref name="settings"/>
		/// does not match the stored settings,  the operation fails with an appropriate error message. </item>
		/// </list></remarks>
		/// <param name="settings">The <see cref="DocsSettings"/> object containing the settings to be saved.  The <see cref="DocsSettings.Name"/>
		/// property must not be null when creating new settings.</param>
		/// <returns>A <see cref="Response{T}"/> object containing the saved <see cref="DocsSettings"/> instance if the operation is
		/// successful.  If the operation fails, the response will include an error message describing the reason for failure.</returns>
		public async Task<Response<ApplicationSettings>> SaveSettingsAsync(ApplicationSettings settings)
		{
			if(settings == null)
			{
				return Response<ApplicationSettings>.Fail("Null can't be empty");
			}
			ApplicationSettings? actualSettings = await _context.ApplicationSettings.FirstOrDefaultAsync();
			if(actualSettings == null)
			{
				// no settings found in the database
				if(settings.ApplicationName == null)
				{
					return Response<ApplicationSettings>.Fail("Name is required");
				}
				ApplicationSettings newDocs = new()
				{
					ApplicationName = settings.ApplicationName,
					Description = settings.Description,
				};
				await _context.AddAsync(newDocs);
				await _context.SaveChangesAsync();
				return Response<ApplicationSettings>.Successful(newDocs, "New record added");
			}
			if(actualSettings.Id != settings.Id)
			{
				return Response<ApplicationSettings>.Fail("Stored settings are for different application name");
			}
			else
			{
				actualSettings.Description = settings.Description;

				await _context.SaveChangesAsync();
				return Response<ApplicationSettings>.Successful(actualSettings, "Data updated successfuly");
			}
		}

		/// <summary>
		/// Asynchronously loads the application settings from the database.
		/// </summary>
		/// <remarks>If no settings are found in the database, the method returns a failure response.</remarks>
		/// <returns>A <see cref="Response{T}"/> object containing the loaded <see cref="DocsSettings"/> if successful,  or a failure
		/// response with an appropriate message if no settings are found.</returns>
		public async Task<Response<ApplicationSettings>> LoadSettingsAsync()
		{
			ApplicationSettings? settings = await _context.ApplicationSettings.FirstOrDefaultAsync();
			if(settings == null)
			{
				return Response<ApplicationSettings>.Fail("No settings found in the database");
			}
			return Response<ApplicationSettings>.Successful(settings, "Settings loaded successfully");
		}

		/// <summary>
		/// Determines whether any settings exist in the data store.
		/// </summary>
		/// <remarks>This method asynchronously checks for the presence of settings in the underlying data
		/// store.</remarks>
		/// <returns><see langword="true"/> if at least one settings record exists in the data store; otherwise, <see
		/// langword="false"/>.</returns>
		public async Task<bool> SettingsExists() => await _context.ApplicationSettings.AnyAsync();
	}
}
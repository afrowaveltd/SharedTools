namespace Afrowave.SharedTools.Docs.Services
{
	public class SettingsService(DocsDbContext context, ILogger<SettingsService> logger)
	{
		private readonly DocsDbContext _context = context;
		private readonly ILogger<SettingsService> _logger = logger;

		public async Task<bool> IsInstalledAsync()
			=> await _context.DocsSettings.AnyAsync();

		public async Task<Response<DocsSettings>> SaveSettingsAsync(DocsSettings settings)
		{
			if(settings == null)
			{
				return Response<DocsSettings>.Fail("Null can't be empty");
			}
			DocsSettings? actualSettings = await _context.DocsSettings.FirstOrDefaultAsync();
			if(actualSettings == null)
			{
				// no settings found in the database
				if(settings.Name == null)
				{
					return Response<DocsSettings>.Fail("Name is required");
				}
				DocsSettings newDocs = new DocsSettings()
				{
					Name = settings.Name,
					MdAbout = settings.MdAbout,
					MdAboutLink = settings.MdAboutLink,
				};
				await _context.AddAsync(newDocs);
				await _context.SaveChangesAsync();
				return Response<DocsSettings>.Successful(newDocs, "New record added");
			}
			if(actualSettings.Name != settings.Name)
			{
				return Response<DocsSettings>.Fail("Stored settings are for different application name");
			}
			else
			{
				actualSettings.MdAbout = settings.MdAbout;
				actualSettings.MdAboutLink = settings.MdAboutLink;
				await _context.SaveChangesAsync();
				return Response<DocsSettings>.Successful(actualSettings, "Data updated successfuly");
			}
		}

		public async Task<Response<DocsSettings>> LoadSettingsAsync()
		{
		}
	}
}
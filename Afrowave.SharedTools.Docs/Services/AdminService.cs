namespace Afrowave.SharedTools.Docs.Services
{
	public class AdminService(ILogger<AdminService> logger,
		DocsDbContext context,
		IStringLocalizer<AdminService> localizer)
	{
		private readonly ILogger<AdminService> _logger = logger;
		private readonly DocsDbContext _context = context;
		private readonly IStringLocalizer<AdminService> _localizer = localizer;
	}
}
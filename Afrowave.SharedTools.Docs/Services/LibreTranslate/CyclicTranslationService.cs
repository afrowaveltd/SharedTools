using Afrowave.SharedTools.Docs.Hubs;

namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

public class CyclicTranslationService(ILibreFileService fileService,
	DocsDbContext context,
	IHubContext<OpenHub> openHub,
	IHubContext<RealtimeHub> realtimeHub,
	ILibreTranslateService translateService,
	ILogger<CyclicTranslationService> logger) : ICyclicTranslationService
{
	private readonly DocsDbContext _context = context;
	private readonly IHubContext<OpenHub> _openHub = openHub;
	private readonly IHubContext<RealtimeHub> _realtimeHub = realtimeHub;
	private readonly ILibreFileService _fileService = fileService;
	private readonly ILibreTranslateService _translateService = translateService;
	private readonly ILogger<CyclicTranslationService> _logger = logger;

	public async Task RunCycleAsync()
	{
		// Server status

		HostedServiceStatus.Clear();
		HostedServiceStatus.Status = WorkerStatus.Checks;
		Console.WriteLine("Cycle starts");
		await _openHub.Clients.All.SendAsync("CycleStarted");

		// A - JSON Dictionary translation

		// B - MD Files translation

		// C - Old DB dataCleaning
	}
}
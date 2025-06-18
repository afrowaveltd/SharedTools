namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

public class CyclicTranslationService(ILibreFileService fileService,
	ILibreTranslateService translateService,
	DocsDbContext context,
	ILogger<CyclicTranslationService> logger) : ICyclicTranslationService
{
	private readonly ILibreFileService _fileService = fileService;
	private readonly ILibreTranslateService _translateService = translateService;
	private readonly DocsDbContext _context = context;
	private readonly ILogger<CyclicTranslationService> _logger = logger;

	public async Task RunCycleAsync()
	{
		// A - JSON Dictionary translation

		// B - MD Files translation
	}
}
using Afrowave.SharedTools.Docs.Hubs;

namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

public class TranslatorHostedService(ILogger<TranslatorHostedService> logger,
	IServiceProvider serviceProvider, IHubContext<OpenHub> openHub) : IHostedService, IDisposable
{
	private readonly ILogger<TranslatorHostedService> _logger = logger;
	private readonly IServiceProvider _serviceProvider = serviceProvider;
	private readonly IHubContext<OpenHub> _openHub = openHub;
	private Timer? _timer;
	private volatile bool _isRunning = false;

	Task IHostedService.StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Translator Hosted Service is starting.");
		Console.WriteLine("Hello");
		return Task.CompletedTask;
	}

	Task IHostedService.StopAsync(CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
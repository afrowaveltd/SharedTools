using Afrowave.SharedTools.Docs.Hubs;

namespace Afrowave.SharedTools.Docs.Services.LibreTranslate;

/// <summary>
/// Represents a hosted service responsible for managing translation-related operations  and interacting with connected
/// clients through SignalR hubs.
/// </summary>
/// <remarks>This service implements the <see cref="IHostedService"/> interface, allowing it to be  started and
/// stopped by the host. It also implements <see cref="IDisposable"/> to release  any unmanaged resources when the
/// service is no longer needed.</remarks>
/// <param name="logger"></param>
/// <param name="serviceProvider"></param>
/// <param name="openHub"></param>
public class TranslatorHostedService(ILogger<TranslatorHostedService> logger,
	IServiceProvider serviceProvider, IHubContext<OpenHub> openHub) : IHostedService, IDisposable
{
	private readonly ILogger<TranslatorHostedService> _logger = logger;
	private readonly IServiceProvider _serviceProvider = serviceProvider;
	private readonly IHubContext<OpenHub> _openHub = openHub;
	private Timer? _timer;
	private volatile bool _isRunning = false;

	/// <summary>
	/// Starts the hosted service asynchronously.
	/// </summary>
	/// <param name="cancellationToken">A token that can be used to signal the operation should be canceled.</param>
	/// <returns>A completed <see cref="Task"/> that represents the asynchronous start operation.</returns>
	Task IHostedService.StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Translator Hosted Service is starting.");
		_timer = new Timer(async _ => await DoWorkAsync(), null, TimeSpan.FromMinutes(5), Timeout.InfiniteTimeSpan);

		return Task.CompletedTask;
	}

	private async Task DoWorkAsync()
	{
		if(_isRunning)
		{
			_logger.LogInformation("Previous cycle of the worker is still running / skipping");
			return;
		}
		_isRunning = true;
		try
		{
			using IServiceScope scope = _serviceProvider.CreateScope();
			ICyclicTranslationService cyclicService = scope.ServiceProvider.GetRequiredService<ICyclicTranslationService>();
			await cyclicService.RunCycleAsync();
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "An error occurred while running translations");
		}
		finally
		{
			_isRunning = false;
			_ = (_timer?.Change(TimeSpan.FromMinutes(30), Timeout.InfiniteTimeSpan));
		}
	}

	/// <summary>
	/// Stops the hosted service asynchronously.
	/// </summary>
	/// <remarks>This method is called by the host to perform any necessary cleanup or shutdown logic  when the
	/// application is stopping. Implementations should ensure that the operation  completes promptly to avoid delaying the
	/// application shutdown process.</remarks>
	/// <param name="cancellationToken">A token that can be used to signal the operation should be canceled.</param>
	/// <returns>A task that represents the asynchronous stop operation.</returns>
	/// <exception cref="NotImplementedException"></exception>
	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("UiTranslatorHostedService is stopping.");
		_ = (_timer?.Change(Timeout.Infinite, 0));
		return Task.CompletedTask;
	}

	/// <summary>
	/// Releases all resources used by the current instance of the class.
	/// </summary>
	/// <remarks>Call this method when you are finished using the instance. This method leaves the instance in an
	/// unusable state. After calling <see cref="Dispose"/>, you must release all references to the instance  so the
	/// garbage collector can reclaim the memory that the instance was occupying.</remarks>
	/// <exception cref="NotImplementedException"></exception>
	public void Dispose() => _timer?.Dispose();
}
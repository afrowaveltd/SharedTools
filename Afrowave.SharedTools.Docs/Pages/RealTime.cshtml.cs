using Afrowave.SharedTools.Docs.Hubs;

namespace Afrowave.SharedTools.Docs.Pages
{
	/// <summary>
	/// Represents the model for a real-time page in an ASP.NET Core Razor Pages application.
	/// </summary>
	/// <remarks>This class is used to handle the logic for the associated Razor Page when a GET request is
	/// made.</remarks>
	public class RealTimeModel(IStringLocalizer<RealTimeModel> localizer,
		IHubContext<RealtimeHub> hub,
		ILogger<RealTimeModel> logger,
		IConfiguration configuration) : PageModel
	{
		private IStringLocalizer<RealTimeModel> _localizer = localizer;
		private IHubContext<RealtimeHub> _hub = hub;
		private ILogger<RealTimeModel> _logger = logger;
		private IConfiguration _configuration = configuration;

		/// <summary>
		/// Handles GET requests for the page.
		/// </summary>
		/// <remarks>This method is invoked when the page is accessed via an HTTP GET request. Override this
		/// method to implement custom logic for handling GET requests.</remarks>
		public async Task OnGetAsync()
		{
		}
	}
}
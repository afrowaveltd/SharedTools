using System.Diagnostics;

namespace Afrowave.SharedTools.Docs.Pages
{
	/// <summary>
	/// Represents the error information for a page request, including details about the request ID.
	/// </summary>
	/// <remarks>This model is used to display error details in a Razor Page. It includes functionality to determine
	/// whether the request ID should be shown and initializes the request ID during a GET request.</remarks>
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	[IgnoreAntiforgeryToken]
	public class ErrorModel : PageModel
	{
		/// <summary>
		/// Gets or sets the unique identifier for the current request.
		/// </summary>
		public string? RequestId { get; set; }

		/// <summary>
		/// Gets a value indicating whether the request ID should be shown.
		/// </summary>
		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

		/// <summary>
		/// Handles GET requests and initializes the request identifier.
		/// </summary>
		/// <remarks>This method sets the <see cref="RequestId"/> property to the current activity's ID if available;
		/// otherwise, it uses the HTTP context's trace identifier.</remarks>
		public void OnGet()
		{
			RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
		}
	}
}
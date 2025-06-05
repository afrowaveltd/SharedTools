using Microsoft.Extensions.Localization;

namespace Afrowave.SharedTools.Docs.Pages
{
	/// <summary>
	/// Represents the model for the Index page in an ASP.NET Core Razor Pages application.
	/// </summary>
	/// <remarks>This class is used to handle the data and behavior for the Index page.  The <see cref="OnGet"/>
	/// method is invoked when a GET request is made to the page.</remarks>
	public class IndexModel(IStringLocalizer<IndexModel> t) : PageModel
	{
		private readonly IStringLocalizer<IndexModel> _t = t;

		public string Test = string.Empty;
		/// <summary>
		/// Handles GET requests for the page.
		/// </summary>
		/// <remarks>This method is invoked when the page is accessed via an HTTP GET request. Override this method to
		/// implement any logic needed to initialize the page's state or prepare data for rendering.</remarks>
		public void OnGet()
		{
			Test = _t["test"];
		}
	}
}
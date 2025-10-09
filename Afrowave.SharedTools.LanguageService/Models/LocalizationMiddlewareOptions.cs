using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afrowave.SharedTools.I18N.Models
{
	/// <summary>
	/// Configuration options for the <c>LocalizationMiddleware</c> that determines how the UI culture is resolved.
	/// </summary>
	/// <remarks>
	/// The middleware can resolve the culture code from multiple sources (query string, cookie, Accept-Language header)
	/// with a configurable precedence. You can also enable/disable individual sources using the boolean flags.
	/// </remarks>
	public sealed class LocalizationMiddlewareOptions
	{
		/// <summary>
		/// Cookie name that stores the language/culture code (e.g., "cs-CZ" or "en-US").
		/// Default value is <c>"lang"</c>.
		/// </summary>
		public string CookieName { get; set; } = "lang";

		/// <summary>
		/// Fallback culture code used when no source can resolve the culture (e.g., "en").
		/// </summary>
		public string DefaultCulture { get; set; } = "en";

		/// <summary>
		/// If true, the middleware attempts to resolve the culture from the query string.
		/// </summary>
		public bool LocalizeByQueryString { get; set; } = false;

		/// <summary>
		/// If true, the middleware attempts to resolve the culture from a cookie.
		/// </summary>
		public bool LocalizeByCookie { get; set; } = true;

		/// <summary>
		/// If true, the middleware attempts to resolve the culture from the Accept-Language HTTP header.
		/// </summary>
		public bool LocalizeByAcceptLanguageHeader { get; set; } = true;

		/// <summary>
		/// First source to check when resolving culture.
		/// </summary>
		public LocalizationSource FirstSource { get; set; } = LocalizationSource.Cookie;

		/// <summary>
		/// Second source to check when resolving culture.
		/// </summary>
		public LocalizationSource SecondSource { get; set; } = LocalizationSource.AcceptLanguageHeader;

		/// <summary>
		/// Third source to check when resolving culture.
		/// </summary>
		public LocalizationSource ThirdSource { get; set; } = LocalizationSource.QueryString;

		/// <summary>
		/// Gets the ordered list of resolution sources based on <see cref="FirstSource"/>, <see cref="SecondSource"/>, and <see cref="ThirdSource"/>,

		/// filtered by the corresponding <c>LocalizeBy*</c> flags and excluding <see cref="LocalizationSource.None"/>.
		/// </summary>
		/// <remarks>
		/// The resulting order is de-duplicated while preserving the original precedence.
		/// </remarks>
		public LocalizationSource[] OrderedSources
		{
			get
			{
				var candidates = new List<LocalizationSource> { FirstSource, SecondSource, ThirdSource };
				IEnumerable<LocalizationSource> filtered = candidates.Where(s => s != LocalizationSource.None);
				filtered = filtered.Where(s => s switch
				{
					LocalizationSource.QueryString => LocalizeByQueryString,
					LocalizationSource.Cookie => LocalizeByCookie,
					LocalizationSource.AcceptLanguageHeader => LocalizeByAcceptLanguageHeader,
					_ => false
				});
				return filtered.Distinct().ToArray();
			}
		}
	}

	/// <summary>
	/// Sources from which the culture can be resolved.
	/// </summary>
	public enum LocalizationSource
	{
		/// <summary>
		/// Resolve from the query string (e.g., <c>?lang=cs</c> or <c>?culture=en-US</c>).
		/// </summary>
		QueryString,

		/// <summary>
		/// Resolve from a cookie (see <see cref="LocalizationMiddlewareOptions.CookieName"/>).
		/// </summary>
		Cookie,

		/// <summary>
		/// Resolve from the Accept-Language HTTP header sent by the client.
		/// </summary>
		AcceptLanguageHeader,

		/// <summary>
		/// Do not use any additional source at this precedence slot.
		/// </summary>
		None
	}
}
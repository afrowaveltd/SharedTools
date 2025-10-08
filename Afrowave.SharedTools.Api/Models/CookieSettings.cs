using Afrowave.SharedTools.Api.Services;
using Microsoft.AspNetCore.Http;
using System;

namespace Afrowave.SharedTools.Api.Models
{
	/// <summary>
	/// Defines default policy and options used when creating and managing cookies.
	/// Intended to be consumed by services like <see cref="Afrowave.SharedTools.Api.Services.CookieService"/>.
	/// </summary>
	public sealed class CookieSettings
	{
		/// <summary>
		/// Domain for which the cookie is valid. Example: ".afrowave.ltd". Empty means host-only cookie.
		/// </summary>
		public string Domain { get; set; } = string.Empty;

		/// <summary>
		/// Number of days until expiration. Set to 0 to create a session cookie.
		/// </summary>
		public int ExpiryInDays { get; set; } = 30;

		/// <summary>
		/// Indicates whether the cookie is accessible only by the server (not by JavaScript).
		/// </summary>
		public bool HttpOnly { get; set; } = false;

		/// <summary>
		/// Whether the cookie is considered essential. Essential cookies may be set without user consent (e.g., session id).
		/// </summary>
		public bool IsEssential { get; set; } = true;

		/// <summary>
		/// Explicit cookie lifetime. Takes precedence over <see cref="ExpiryInDays"/> if specified.
		/// </summary>
		public TimeSpan? MaxAge { get; set; }

		/// <summary>
		/// Path for which the cookie is valid. Defaults to "/".
		/// </summary>
		public string Path { get; set; } = "/";

		/// <summary>
		/// SameSite behavior according to RFC6265bis.
		/// </summary>
		public SameSitePolicy SameSite { get; set; } = SameSitePolicy.Lax;

		/// <summary>
		/// Indicates whether the cookie should be sent only over HTTPS.
		/// </summary>
		public bool Secure { get; set; } = true;

		/// <summary>
		/// Determines when the Secure flag should be applied.
		/// </summary>
		public CookieSecurePolicy SecurePolicy { get; set; } = CookieSecurePolicy.SameAsRequest;

		/// <summary>
		/// Default SameSite value — enumerates all supported modes.
		/// </summary>
		public enum SameSitePolicy
		{
			/// <summary>No restrictions — the cookie is sent on cross-site requests.</summary>
			None = 0,

			/// <summary>Not sent on cross-site navigations; sent on top-level navigations and same-site requests.</summary>
			Lax = 1,

			/// <summary>Never sent on cross-site requests.</summary>
			Strict = 2
		}

		/// <summary>
		/// Creates an instance of <see cref="Microsoft.AspNetCore.Http.CookieOptions"/> based on the current settings.
		/// </summary>
		/// <param name="expiresOverride">Optional explicit expiration timestamp to apply.</param>
		/// <returns>A configured <see cref="Microsoft.AspNetCore.Http.CookieOptions"/> instance.</returns>
		public Microsoft.AspNetCore.Http.CookieOptions ToCookieOptions(DateTimeOffset? expiresOverride = null)
		{
			var options = new Microsoft.AspNetCore.Http.CookieOptions
			{
				Domain = Domain,
				Path = string.IsNullOrWhiteSpace(Path) ? "/" : Path,
				HttpOnly = HttpOnly,
				Secure = Secure,
				IsEssential = IsEssential
			};

			// Honor SecurePolicy (informational; typically enforced by middleware/policies)
			if(SecurePolicy == CookieSecurePolicy.Always)
				options.Secure = true;
			else if(SecurePolicy == CookieSecurePolicy.None)
				options.Secure = false;

			// Map SameSite
			options.SameSite = SameSite switch
			{
				SameSitePolicy.Strict => Microsoft.AspNetCore.Http.SameSiteMode.Strict,
				SameSitePolicy.Lax => Microsoft.AspNetCore.Http.SameSiteMode.Lax,
				_ => Microsoft.AspNetCore.Http.SameSiteMode.None,
			};

			// Expires / MaxAge
			if(expiresOverride.HasValue)
			{
				options.Expires = expiresOverride.Value;
			}
			else if(MaxAge.HasValue)
			{
				options.MaxAge = MaxAge.Value;
				options.Expires = DateTimeOffset.UtcNow.Add(MaxAge.Value);
			}
			else if(ExpiryInDays > 0)
			{
				options.Expires = DateTimeOffset.UtcNow.AddDays(ExpiryInDays);
			}

			// If SameSite=None, Secure must be true
			if(options.SameSite == SameSiteMode.None && !options.Secure)
				options.Secure = true;

			return options;
		}
	}
}
namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Provides methods for managing cookies, including retrieving, creating, updating, and removing cookies.
	/// </summary>
	/// <remarks>This interface defines a set of operations for working with cookies in a web application.  It
	/// includes methods for retrieving existing cookies, creating or updating cookies with optional expiration times,  and
	/// removing cookies. Additionally, it supports setting HTTP-only cookies for enhanced security.</remarks>
	public interface ICookieService
	{
		/// <summary>
		/// Retrieves the value of a cookie by its key.
		/// </summary>
		/// <param name="key">The key of the cookie to retrieve. Cannot be null or empty.</param>
		/// <returns>The value of the cookie associated with the specified key, or <see langword="null"/> if the cookie does not exist.</returns>
		string GetCookie(string key);

		/// <summary>
		/// Retrieves the value of an existing cookie with the specified key, or creates a new cookie with the given key and
		/// value.
		/// </summary>
		/// <remarks>If a cookie with the specified key already exists, its value is returned without modification. If
		/// no such cookie exists, a new cookie is created with the specified key, value, and expiration time.</remarks>
		/// <param name="key">The key identifying the cookie. Must not be null or empty.</param>
		/// <param name="value">The value to assign to the cookie if it does not already exist.</param>
		/// <param name="expireTime">The expiration time of the cookie in minutes. If set to 0, the cookie will be created as a session cookie.</param>
		/// <returns>The value of the existing cookie if found; otherwise, the value of the newly created cookie.</returns>
		string GetOrCreateCookie(string key, string value, int expireTime = 0);

		/// <summary>
		/// Removes all cookies from the current session.
		/// </summary>
		/// <remarks>This method clears all cookies stored in the current session, including those associated with any
		/// domains. Use this method to reset the session's cookie state completely. Note that this action cannot be
		/// undone.</remarks>
		void RemoveAllCookies();

		/// <summary>
		/// Removes the cookie associated with the specified key.
		/// </summary>
		/// <remarks>If the specified key does not exist, the method performs no action.</remarks>
		/// <param name="key">The key of the cookie to remove. Cannot be null or empty.</param>
		void RemoveCookie(string key);

		/// <summary>
		/// Sets a cookie with the specified key, value, and expiration time.
		/// </summary>
		/// <remarks>If <paramref name="expireTime"/> is set to 0, the cookie will be treated as a session cookie and
		/// will not persist after the browser is closed.</remarks>
		/// <param name="key">The name of the cookie. Cannot be null or empty.</param>
		/// <param name="value">The value to store in the cookie. Cannot be null.</param>
		/// <param name="expireTime">The expiration time of the cookie, in minutes. A value of 0 indicates that the cookie will expire at the end of
		/// the session.</param>
		void SetCookie(string key, string value, int expireTime = 0);

		/// <summary>
		/// Sets an HTTP-only cookie with the specified key, value, and expiration time.
		/// </summary>
		/// <remarks>HTTP-only cookies are inaccessible to client-side scripts, providing additional security by
		/// mitigating certain types of attacks, such as cross-site scripting (XSS).</remarks>
		/// <param name="key">The name of the cookie. Cannot be null or empty.</param>
		/// <param name="value">The value to store in the cookie. Cannot be null.</param>
		/// <param name="expireTime">The expiration time of the cookie in minutes. If set to 0, the cookie will be a session cookie and will expire
		/// when the browser is closed.</param>
		void SetHttpOnlyCookie(string key, string value, int expireTime = 0);
	}
}
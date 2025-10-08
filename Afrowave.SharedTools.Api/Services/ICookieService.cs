using Afrowave.SharedTools.Models.Results;
using System;

namespace Afrowave.SharedTools.Api.Services
{
	/// <summary>
	/// Provides high-level cookie operations for ASP.NET Core applications with support for named option profiles.
	/// </summary>
	/// <remarks>
	/// Implementations are expected to use cookie settings resolved from named profiles (e.g., via IOptionsMonitor) so that
	/// domain, path, security, and lifetime can be configured per use-case. Methods include convenience variants for
	/// long-lived cookies and JSON serialization helpers.
	/// </remarks>
	public interface ICookieService
	{
		/// <summary>
		/// Deletes the specified cookie using options derived from an optional profile (domain/path/samesite must match the writer).
		/// </summary>
		/// <param name="name">The cookie name to delete.</param>
		/// <param name="profileName">Optional profile name to build matching deletion options. If null, the default profile is used.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		Result Delete(string name, string profileName = null);

		/// <summary>
		/// Checks whether a cookie exists on the current request.
		/// </summary>
		/// <param name="name">The cookie name to look up.</param>
		/// <returns>True if the cookie is present; otherwise, false.</returns>
		bool Exists(string name);

		/// <summary>
		/// Gets the raw value of a cookie.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns>The cookie value if present; otherwise, null.</returns>
		string? Read(string name);

		/// <summary>
		/// Reads a JSON-serialized object from a cookie or creates the cookie with the specified default value and returns it.
		/// On deserialization failure, the provided <paramref name="defaultValue"/> is returned.
		/// </summary>
		/// <typeparam name="T">The expected object type.</typeparam>
		/// <param name="name">The cookie name.</param>
		/// <param name="defaultValue">The value to write and return when the cookie is missing.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <returns>
		/// The deserialized value when the cookie exists and contains valid JSON; otherwise the <paramref name="defaultValue"/>.
		/// </returns>
		T ReadObjectOrCreate<T>(string name, T defaultValue, string profileName = null);

		/// <summary>
		/// Returns the value of an existing cookie or creates it with the specified default value and returns that value.
		/// Uses a long-lived lifetime when creating the cookie.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="defaultValue">The value to write if the cookie does not exist.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <returns>The current cookie value if present; otherwise, the default value when successfully created.</returns>
		string ReadOrCreate(string name, string defaultValue, string profileName = null);

		/// <summary>
		/// Returns the value of an existing cookie or creates it with the specified default value and expiration, then returns that value.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="defaultValue">The value to write if the cookie does not exist.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <param name="expiresOverride">Explicit expiration timestamp used when creating the cookie.</param>
		/// <returns>The current cookie value if present; otherwise, the default value when successfully created.</returns>
		string ReadOrCreate(string name, string defaultValue, string profileName, DateTimeOffset? expiresOverride);

		/// <summary>
		/// Reads a cookie and returns a standardized response.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns>
		/// A successful <see cref="Response{T}"/> containing the cookie value when found;
		/// otherwise, a failed response with an error message.
		/// </returns>
		Response<string> ReadResponse(string name);

		/// <summary>
		/// Creates or overwrites a cookie using a long-lived lifetime by default.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="value">The cookie value. Null should be treated as an empty string.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		Result Update(string name, string value, string profileName = null);

		/// <summary>
		/// Creates or overwrites a cookie with a custom expiration.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="value">The cookie value. Null should be treated as an empty string.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <param name="expiresOverride">Explicit expiration timestamp.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		Result Update(string name, string value, string profileName, DateTimeOffset? expiresOverride);

		/// <summary>
		/// Serializes an object to JSON and stores it into the specified cookie.
		/// </summary>
		/// <typeparam name="T">The object type to serialize.</typeparam>
		/// <param name="name">The cookie name.</param>
		/// <param name="value">The object to serialize and store.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		Result UpdateObject<T>(string name, T value, string profileName = null);

		/// <summary>
		/// Writes a cookie only if it does not already exist, using a long-lived lifetime by default.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="value">The cookie value. Null should be treated as an empty string.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		Result Write(string name, string value, string profileName = null);

		/// <summary>
		/// Writes a cookie only if it does not already exist, with a custom expiration.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="value">The cookie value. Null should be treated as an empty string.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <param name="expiresOverride">Explicit expiration timestamp.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		Result Write(string name, string value, string profileName, DateTimeOffset? expiresOverride);
	}
}
using Afrowave.SharedTools.Api.Models;
using Afrowave.SharedTools.Models.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Afrowave.SharedTools.Api.Services
{
	/// <summary>
	/// Provides high-level cookie operations backed by named options profiles via <see cref="IOptionsMonitor{TOptions}"/>.
	/// Offers convenient methods to write, update, read, and read-or-create cookies, including variants with long-lived ("infinite") lifetime.
	/// </summary>
	/// <remarks>
	/// The service reads cookie configuration from <see cref="CookieSettings"/> using the current profile name (options monitor).
	/// Use different profiles to apply distinct cookie policies (domain, path, security) per scenario.
	/// </remarks>
	public sealed class CookieService : ICookieService
	{
		private static readonly TimeSpan InfiniteMaxAge = TimeSpan.FromDays(365 * 20); // ~20 years
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IOptionsMonitor<CookieSettings> _settings;
		private readonly JsonSerializerOptions _json;

		/// <summary>
		/// Initializes a new instance of the <see cref="CookieService"/> class.
		/// </summary>
		/// <param name="httpContextAccessor">Accessor used to obtain the current <see cref="HttpContext"/>.</param>
		/// <param name="settings">Options monitor providing named profiles of <see cref="CookieSettings"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="httpContextAccessor"/> or <paramref name="settings"/> is null.</exception>
		public CookieService(IHttpContextAccessor httpContextAccessor, IOptionsMonitor<CookieSettings> settings)
		{
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
			_settings = settings ?? throw new ArgumentNullException(nameof(settings));

			_json = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = false
			};
		}

		/// <summary>
		/// Builds cookie options for the specified profile, optionally overriding the expiration or forcing a long-lived lifetime.
		/// </summary>
		/// <param name="profileName">The name of the options profile to use. If null or empty, the current value is used.</param>
		/// <param name="expiresOverride">Optional explicit expiration timestamp.</param>
		/// <param name="infiniteLifetime">If true, converts the effective settings to use a very long <see cref="CookieSettings.MaxAge"/> (about 20 years).</param>
		/// <returns>A configured <see cref="CookieOptions"/> instance.</returns>
		private CookieOptions BuildOptions(string profileName, DateTimeOffset? expiresOverride, bool infiniteLifetime)
		{
			var cfg = string.IsNullOrWhiteSpace(profileName) ? _settings.CurrentValue : _settings.Get(profileName);

			CookieSettings effective = cfg;
			if(infiniteLifetime)
			{
				effective = new CookieSettings
				{
					Domain = cfg.Domain,
					Path = string.IsNullOrWhiteSpace(cfg.Path) ? "/" : cfg.Path,
					HttpOnly = cfg.HttpOnly,
					Secure = cfg.Secure,
					SameSite = cfg.SameSite,
					IsEssential = cfg.IsEssential,
					SecurePolicy = cfg.SecurePolicy,
					MaxAge = InfiniteMaxAge,
					ExpiryInDays = 0
				};
			}

			return effective.ToCookieOptions(expiresOverride);
		}

		/// <summary>
		/// Ensures an <see cref="HttpContext"/> is available and returns it.
		/// </summary>
		/// <param name="errorResult">Outputs a failure <see cref="Result"/> if the context is missing, or a success result otherwise.</param>
		/// <returns>The current <see cref="HttpContext"/>, or null if not available.</returns>
		private HttpContext RequireHttpContext(out Result errorResult)
		{
			var ctx = _httpContextAccessor.HttpContext;
			if(ctx == null)
			{
				errorResult = Result.Fail("HttpContext is not available.");
				return null;
			}
			errorResult = Result.Ok();
			return ctx;
		}

		/// <summary>
		/// Checks whether a cookie exists on the incoming request.
		/// </summary>
		/// <param name="name">The cookie name to look up.</param>
		/// <returns>True if the cookie is present; otherwise, false.</returns>
		public bool Exists(string name)
		{
			var ctx = _httpContextAccessor.HttpContext;
			return ctx != null && !string.IsNullOrWhiteSpace(name) && ctx.Request.Cookies.ContainsKey(name);
		}

		/// <summary>
		/// Deletes the specified cookie using options derived from the provided profile (domain/path/samesite must match the writer).
		/// </summary>
		/// <param name="name">The cookie name to delete.</param>
		/// <param name="profileName">Optional profile name to build matching deletion options.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		public Result Delete(string name, string profileName = null)
		{
			var ctx = RequireHttpContext(out var err);
			if(ctx == null) return err;
			if(string.IsNullOrWhiteSpace(name)) return Result.Fail("Cookie name is required.");

			var opts = BuildOptions(profileName, DateTimeOffset.UtcNow.AddYears(-10), infiniteLifetime: false);
			try
			{
				ctx.Response.Cookies.Delete(name, opts);
				return Result.Ok("Cookie deleted.");
			}
			catch(Exception ex)
			{
				return Result.Fail(ex.Message);
			}
		}

		/// <summary>
		/// Gets the raw value of a cookie.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns>The cookie value if present; otherwise, null.</returns>
		public string? Read(string name)
		{
			var ctx = _httpContextAccessor.HttpContext;
			if(ctx == null || string.IsNullOrWhiteSpace(name)) return null;
			string value;
			return ctx.Request.Cookies.TryGetValue(name, out value) ? value : null;
		}

		/// <summary>
		/// Reads a cookie and returns a standardized response.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns>
		/// A successful response containing the cookie value when found;
		/// otherwise, a failed response with an error message.
		/// </returns>
		public Response<string> ReadResponse(string name)
		{
			var value = Read(name);
			return value != null ? Response<string>.SuccessResponse(value, "OK") : Response<string>.Fail("Cookie not found.");
		}

		/// <summary>
		/// Writes a cookie only if it does not already exist. Uses a long-lived (approximately 20 years) lifetime by default.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="value">The cookie value. Null is treated as an empty string.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		public Result Write(string name, string value, string profileName = null)
			 => Write(name, value, profileName, expiresOverride: null, infiniteLifetime: true);

		/// <summary>
		/// Writes a cookie only if it does not already exist, with a custom expiration.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="value">The cookie value. Null is treated as an empty string.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <param name="expiresOverride">Explicit expiration timestamp.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		public Result Write(string name, string value, string profileName, DateTimeOffset? expiresOverride)
			 => Write(name, value, profileName, expiresOverride, infiniteLifetime: false);

		private Result Write(string name, string value, string profileName, DateTimeOffset? expiresOverride, bool infiniteLifetime)
		{
			if(string.IsNullOrWhiteSpace(name)) return Result.Fail("Cookie name is required.");

			var ctx = RequireHttpContext(out var err);
			if(ctx == null) return err;

			if(Exists(name))
				return Result.Ok("Cookie already exists – nothing written.");

			var opts = BuildOptions(profileName, expiresOverride, infiniteLifetime);
			try
			{
				ctx.Response.Cookies.Append(name, value ?? string.Empty, opts);
				return Result.Ok("Cookie written.");
			}
			catch(Exception ex)
			{
				return Result.Fail(ex.Message);
			}
		}

		/// <summary>
		/// Creates or overwrites a cookie. Uses a long-lived (approximately 20 years) lifetime by default.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="value">The cookie value. Null is treated as an empty string.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		public Result Update(string name, string value, string profileName = null)
			 => Update(name, value, profileName, expiresOverride: null, infiniteLifetime: true);

		/// <summary>
		/// Creates or overwrites a cookie with a custom expiration.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="value">The cookie value. Null is treated as an empty string.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <param name="expiresOverride">Explicit expiration timestamp.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		public Result Update(string name, string value, string profileName, DateTimeOffset? expiresOverride)
			 => Update(name, value, profileName, expiresOverride, infiniteLifetime: false);

		private Result Update(string name, string value, string profileName, DateTimeOffset? expiresOverride, bool infiniteLifetime)
		{
			if(string.IsNullOrWhiteSpace(name)) return Result.Fail("Cookie name is required.");

			var ctx = RequireHttpContext(out var err);
			if(ctx == null) return err;

			var opts = BuildOptions(profileName, expiresOverride, infiniteLifetime);
			try
			{
				// Append overwrites existing cookie with the same name.
				ctx.Response.Cookies.Append(name, value ?? string.Empty, opts);
				return Result.Ok("Cookie updated.");
			}
			catch(Exception ex)
			{
				return Result.Fail(ex.Message);
			}
		}

		/// <summary>
		/// Returns the value of an existing cookie or creates it with the specified default value and returns that value.
		/// Uses a long-lived (approximately 20 years) lifetime when creating the cookie.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="defaultValue">The value to write if the cookie does not exist.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <returns>The current cookie value if present; otherwise, the default value when successfully created; otherwise, null.</returns>
		public string ReadOrCreate(string name, string defaultValue, string profileName = null)
		{
			var current = Read(name);
			if(current != null) return current;

			var res = Write(name, defaultValue, profileName);
			if(!res.Success) return null;
			return defaultValue;
		}

		/// <summary>
		/// Returns the value of an existing cookie or creates it with the specified default value and expiration, then returns that value.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="defaultValue">The value to write if the cookie does not exist.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <param name="expiresOverride">Explicit expiration timestamp when creating the cookie.</param>
		/// <returns>The current cookie value if present; otherwise, the default value when successfully created; otherwise, null.</returns>
		public string ReadOrCreate(string name, string defaultValue, string profileName, DateTimeOffset? expiresOverride)
		{
			var current = Read(name);
			if(current != null) return current;

			var res = Write(name, defaultValue, profileName, expiresOverride);
			if(!res.Success) return null;
			return defaultValue;
		}

		/// <summary>
		/// Serializes an object to JSON and stores it into the specified cookie using <see cref="Update(string, string, string)"/>.
		/// </summary>
		/// <typeparam name="T">The object type to serialize.</typeparam>
		/// <param name="name">The cookie name.</param>
		/// <param name="value">The object to serialize and store.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <returns>A <see cref="Result"/> describing the outcome.</returns>
		public Result UpdateObject<T>(string name, T value, string profileName = null)
		{
			string serialized;
			try { serialized = JsonSerializer.Serialize(value, _json); }
			catch(Exception ex) { return Result.Fail("Serialization failed: " + ex.Message); }

			return Update(name, serialized, profileName);
		}

		/// <summary>
		/// Reads a JSON-serialized object from a cookie or creates the cookie with the specified default value and returns it.
		/// On deserialization failure, returns <paramref name="defaultValue"/>.
		/// </summary>
		/// <typeparam name="T">The expected object type.</typeparam>
		/// <param name="name">The cookie name.</param>
		/// <param name="defaultValue">The value to write and return when the cookie is missing.</param>
		/// <param name="profileName">Optional profile used to build cookie options.</param>
		/// <returns>
		/// The deserialized value when the cookie exists and contains valid JSON; otherwise,
		/// <paramref name="defaultValue"/> when the cookie was created or deserialization fails; otherwise, default of <typeparamref name="T"/> when context is missing.
		/// </returns>
		public T ReadObjectOrCreate<T>(string name, T defaultValue, string profileName = null)
		{
			var raw = ReadOrCreate(name, JsonSerializer.Serialize(defaultValue, _json), profileName);
			if(raw == null) return default(T);
			try { return JsonSerializer.Deserialize<T>(raw, _json); }
			catch { return defaultValue; }
		}
	}
}
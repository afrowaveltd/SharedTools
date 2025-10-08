// Afrowave.SharedTools.Api/Static/CookieHelper.cs
using Afrowave.SharedTools.Api.Models;
using Afrowave.SharedTools.Models.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;

namespace Afrowave.SharedTools.Api.Static
{
	/// <summary>
	/// Statický helper pro práci s cookies (bez DI).
	/// Nabízí Write / Update / Read / ReadOrCreate + Exists / Delete.
	/// Overloady s méně parametry používají "nekonečnou" platnost (~20 let).
	/// </summary>
	public static class CookieHelper
	{
		private static readonly TimeSpan InfiniteMaxAge = TimeSpan.FromDays(365 * 20);

		private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = false
		};

		// ---------------------------
		// Utilities
		// ---------------------------

		private static CookieOptions BuildOptions(CookieSettings settings, DateTimeOffset? expiresOverride, bool infiniteLifetime)
		{
			var cfg = settings ?? new CookieSettings();

			if(infiniteLifetime)
			{
				// klon s velmi dlouhou životností
				cfg = new CookieSettings
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

			return cfg.ToCookieOptions(expiresOverride);
		}

		// ---------------------------
		// Exists / Delete
		// ---------------------------

		public static bool Exists(HttpContext ctx, string name)
		{
			if(ctx == null || string.IsNullOrWhiteSpace(name)) return false;
			return ctx.Request.Cookies.ContainsKey(name);
		}

		public static Result Delete(HttpContext ctx, string name, CookieSettings settings = null)
		{
			if(ctx == null) return Result.Fail("HttpContext is not available.");
			if(string.IsNullOrWhiteSpace(name)) return Result.Fail("Cookie name is required.");

			var opts = BuildOptions(settings, DateTimeOffset.UtcNow.AddYears(-10), infiniteLifetime: false);
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

		// ---------------------------
		// Read
		// ---------------------------

		/// <summary>Vrátí hodnotu cookie, nebo null pokud neexistuje.</summary>
		public static string Read(HttpContext ctx, string name)
		{
			if(ctx == null || string.IsNullOrWhiteSpace(name)) return null;
			return ctx.Request.Cookies.TryGetValue(name, out var v) ? v : null;
		}

		// ---------------------------
		// Write (create only if not exists)
		// ---------------------------

		/// <summary>Zapíše cookie pouze pokud neexistuje. Nekonečná platnost (~20 let).</summary>
		public static Result Write(HttpContext ctx, string name, string value)
			 => Write(ctx, name, value, settings: null, expiresOverride: null, infiniteLifetime: true);

		/// <summary>Zapíše cookie pouze pokud neexistuje s konkrétní expirací.</summary>
		public static Result Write(HttpContext ctx, string name, string value, DateTimeOffset expires)
			 => Write(ctx, name, value, settings: null, expiresOverride: expires, infiniteLifetime: false);

		/// <summary>Zapíše cookie pouze pokud neexistuje s vlastními nastaveními a nekonečnou platností.</summary>
		public static Result Write(HttpContext ctx, string name, string value, CookieSettings settings)
			 => Write(ctx, name, value, settings, expiresOverride: null, infiniteLifetime: true);

		/// <summary>Zapíše cookie pouze pokud neexistuje s vlastními nastaveními a expirací.</summary>
		public static Result Write(HttpContext ctx, string name, string value, CookieSettings settings, DateTimeOffset? expiresOverride)
			 => Write(ctx, name, value, settings, expiresOverride, infiniteLifetime: false);

		private static Result Write(HttpContext ctx, string name, string value, CookieSettings settings, DateTimeOffset? expiresOverride, bool infiniteLifetime)
		{
			if(ctx == null) return Result.Fail("HttpContext is not available.");
			if(string.IsNullOrWhiteSpace(name)) return Result.Fail("Cookie name is required.");

			if(Exists(ctx, name))
				return Result.Ok("Cookie already exists – nothing written.");

			var opts = BuildOptions(settings, expiresOverride, infiniteLifetime);
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

		// ---------------------------
		// Update (create or overwrite)
		// ---------------------------

		/// <summary>Vytvoří nebo přepíše cookie. Nekonečná platnost (~20 let).</summary>
		public static Result Update(HttpContext ctx, string name, string value)
			 => Update(ctx, name, value, settings: null, expiresOverride: null, infiniteLifetime: true);

		/// <summary>Vytvoří nebo přepíše cookie s konkrétní expirací.</summary>
		public static Result Update(HttpContext ctx, string name, string value, DateTimeOffset expires)
			 => Update(ctx, name, value, settings: null, expiresOverride: expires, infiniteLifetime: false);

		/// <summary>Vytvoří nebo přepíše cookie s vlastními nastaveními a nekonečnou platností.</summary>
		public static Result Update(HttpContext ctx, string name, string value, CookieSettings settings)
			 => Update(ctx, name, value, settings, expiresOverride: null, infiniteLifetime: true);

		/// <summary>Vytvoří nebo přepíše cookie s vlastními nastaveními a expirací.</summary>
		public static Result Update(HttpContext ctx, string name, string value, CookieSettings settings, DateTimeOffset? expiresOverride)
			 => Update(ctx, name, value, settings, expiresOverride, infiniteLifetime: false);

		private static Result Update(HttpContext ctx, string name, string value, CookieSettings settings, DateTimeOffset? expiresOverride, bool infiniteLifetime)
		{
			if(ctx == null) return Result.Fail("HttpContext is not available.");
			if(string.IsNullOrWhiteSpace(name)) return Result.Fail("Cookie name is required.");

			var opts = BuildOptions(settings, expiresOverride, infiniteLifetime);
			try
			{
				ctx.Response.Cookies.Append(name, value ?? string.Empty, opts); // Append přepíše existující
				return Result.Ok("Cookie updated.");
			}
			catch(Exception ex)
			{
				return Result.Fail(ex.Message);
			}
		}

		// ---------------------------
		// ReadOrCreate
		// ---------------------------

		/// <summary>
		/// Vrátí hodnotu cookie; pokud neexistuje, vytvoří ji s danou hodnotou a nekonečnou platností.
		/// </summary>
		public static string ReadOrCreate(HttpContext ctx, string name, string defaultValue)
		{
			var current = Read(ctx, name);
			if(current != null) return current;

			var res = Write(ctx, name, defaultValue);
			return res.Success ? defaultValue : null;
		}

		/// <summary>
		/// Vrátí hodnotu cookie; pokud neexistuje, vytvoří ji s danou hodnotou a zadanou expirací.
		/// </summary>
		public static string ReadOrCreate(HttpContext ctx, string name, string defaultValue, DateTimeOffset expires)
		{
			var current = Read(ctx, name);
			if(current != null) return current;

			var res = Write(ctx, name, defaultValue, expires);
			return res.Success ? defaultValue : null;
		}

		/// <summary>
		/// Vrátí hodnotu cookie; pokud neexistuje, vytvoří ji s danou hodnotou a nekonečnou platností dle zadaných nastavení.
		/// </summary>
		public static string ReadOrCreate(HttpContext ctx, string name, string defaultValue, CookieSettings settings)
		{
			var current = Read(ctx, name);
			if(current != null) return current;

			var res = Write(ctx, name, defaultValue, settings);
			return res.Success ? defaultValue : null;
		}

		/// <summary>
		/// Vrátí hodnotu cookie; pokud neexistuje, vytvoří ji s danou hodnotou, nastaveními a expirací.
		/// </summary>
		public static string ReadOrCreate(HttpContext ctx, string name, string defaultValue, CookieSettings settings, DateTimeOffset expires)
		{
			var current = Read(ctx, name);
			if(current != null) return current;

			var res = Write(ctx, name, defaultValue, settings, expires);
			return res.Success ? defaultValue : null;
		}

		// ---------------------------
		// (volitelné) JSON helpery
		// ---------------------------

		public static Result UpdateObject<T>(HttpContext ctx, string name, T value, CookieSettings settings = null)
		{
			string serialized;
			try { serialized = JsonSerializer.Serialize(value, _json); }
			catch(Exception ex) { return Result.Fail("Serialization failed: " + ex.Message); }

			return Update(ctx, name, serialized, settings);
		}

		public static T ReadObjectOrCreate<T>(HttpContext ctx, string name, T defaultValue, CookieSettings settings = null)
		{
			var raw = ReadOrCreate(ctx, name, JsonSerializer.Serialize(defaultValue, _json), settings);
			if(raw == null) return default(T);
			try { return JsonSerializer.Deserialize<T>(raw, _json); }
			catch { return defaultValue; }
		}
	}
}
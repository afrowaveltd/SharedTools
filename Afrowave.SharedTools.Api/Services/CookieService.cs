using Afrowave.SharedTools.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Afrowave.SharedTools.Api.Services
{
	public class CookieService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IOptionsMonitor<CookieSettings> _cookie;
		private readonly Microsoft.AspNetCore.Http.CookieOptions _cookieOptions;

		public CookieService(IOptionsMonitor<CookieSettings> cookie, IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			_cookie = cookie;
			_cookieOptions = _cookie.CurrentValue.ToCookieOptions();
		}
	}
}
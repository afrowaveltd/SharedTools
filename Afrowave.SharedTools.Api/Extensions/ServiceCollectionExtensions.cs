using Afrowave.SharedTools.Api.Models;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Afrowave.SharedTools.Api.Services;

namespace Afrowave.SharedTools.Api.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCookieService(
			this IServiceCollection services,
			Action<CookieSettings> configure)
		{
			if(services == null) throw new ArgumentNullException(nameof(services));

			services.AddOptions<CookieSettings>()
				.Configure(configure);

			services.AddSingleton<CookieService>();
			services.AddHttpContextAccessor();
			return services;
		}
	}
}
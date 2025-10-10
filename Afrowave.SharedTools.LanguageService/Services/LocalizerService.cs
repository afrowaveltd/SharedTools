using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.I18N.Services
{
	public class LocalizerService : IStringLocalizer
	{
		private readonly IDistributedCache _cache;
		private readonly IOptions<LocalizationOptions> _options;

		public LocalizerService(IDistributedCache cache, IOptions<LocalizationOptions> options)
		{
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		LocalizedString IStringLocalizer.this[string name] => throw new NotImplementedException();

		LocalizedString IStringLocalizer.this[string name, params object[] arguments] => throw new NotImplementedException();

		IEnumerable<LocalizedString> IStringLocalizer.GetAllStrings(bool includeParentCultures)
		{
			throw new NotImplementedException();
		}
	}
}
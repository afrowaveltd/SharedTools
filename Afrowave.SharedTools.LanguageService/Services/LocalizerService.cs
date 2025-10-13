using Afrowave.SharedTools.I18N.DataStorages.JsonFlat;
using Afrowave.SharedTools.I18N.DataStorages.JsonFlat.Models;
using Afrowave.SharedTools.I18N.DataStorages.JsonFlat.Services;
using Afrowave.SharedTools.I18N.Interfaces;
using Afrowave.SharedTools.I18N.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Afrowave.SharedTools.I18N.Services
{
	public class LocalizerService : IStringLocalizer
	{
		private readonly IDistributedCache _cache;
		private readonly LocalizerOptions _options;
		private readonly ICapabilities _capabilities;
		private List<TranslationProvider> _translationProviders = new List<TranslationProvider> { TranslationProvider.None };
		private DictionaryFormat _supportedFormat = DictionaryFormat.JSON_FLAT;
		private bool _storeMissingKeys = false;
		private readonly IDataStorage _dataStorage;
		private readonly IOptions<IDataStorage> _dataStorageOptions;

		public LocalizerService(IDistributedCache cache, IOptions<LocalizerOptions> options, IOptions<IDataStorage> dataStorageOptions, ICapabilities capabilities)
		{
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_options = options.Value ?? new LocalizerOptions();
			_dataStorageOptions = dataStorageOptions;
			if(_options.TranslationProviders != null && _options.TranslationProviders.Count > 0)
			{
				_translationProviders = _options.TranslationProviders;
			}
			_supportedFormat = _options.SupportedFormat;
			_storeMissingKeys = _options.StoreMissingKeys;
			_capabilities = capabilities ?? throw new ArgumentNullException(nameof(capabilities));
			var capabilitiesLocal = _capabilities.GetCapabilities();
			switch(_capabilities.GetCapabilities().DictionaryFormat)
			{
				case DictionaryFormat.JSON_FLAT:
					if(_supportedFormat != DictionaryFormat.JSON_FLAT)
					{
						throw new NotSupportedException($"The configured dictionary format '{_supportedFormat}' is not supported by the current data storage provider which supports '{capabilitiesLocal.DictionaryFormat}'.");
					}
					_dataStorage = new FlatJsonDataStorage((IOptions<JsonFlatDataStorageOptions>)_dataStorageOptions, _capabilities);
					break;

				case DictionaryFormat.JSON_NESTED:
					if(_supportedFormat != DictionaryFormat.JSON_NESTED)
					{
						throw new NotSupportedException($"The configured dictionary format '{_supportedFormat}' is not supported by the current data storage provider which supports '{capabilitiesLocal.DictionaryFormat}'.");
					}
					_dataStorage = new FlatJsonDataStorage((IOptions<JsonFlatDataStorageOptions>)_dataStorageOptions, _capabilities);
					break;
			}
		}

		LocalizedString IStringLocalizer.this[string name]
		{
			get
			{
				var value = GetString(name);
				return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
			}
		}

		public LocalizedString this[string name, params object[] arguments]
		{
			get
			{
				LocalizedString actualValue = this[name];
				return !actualValue.ResourceNotFound
					  ? new LocalizedString(name, string.Format(actualValue.Value, arguments), false)
					  : actualValue;
			}
		}

		IEnumerable<LocalizedString> IStringLocalizer.GetAllStrings(bool includeParentCultures)
		{
			throw new NotImplementedException();
		}

		private string GetString(string name)
		{
			return string.Empty;
		}
	}
}
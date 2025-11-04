using Afrowave.SharedTools.I18N.Interfaces;
using Afrowave.SharedTools.I18N.Models;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.I18N.DataStorages.JsonFlatFtp
{
	public class JsonFlatFtpDataStorage : IDataStorage
	{
		public event IDataStorage.DictionaryChangedEventHandler DictionaryChanged;

		public Task<Result> AddTranslation(string languageCode, string key, string value)
		{
			throw new NotImplementedException();
		}

		public Task<Result> DeleteDictionaryAsync(string languageCode)
		{
			throw new NotImplementedException();
		}

		public Task<bool> DictionaryExistsAsync(string languageCode)
		{
			throw new NotImplementedException();
		}

		public DataStorageCapabilities GetCapabilities()
		{
			throw new NotImplementedException();
		}

		public Task<Response<string>> GetTranslation(string languageCode, string key)
		{
			throw new NotImplementedException();
		}

		public Task<bool> IsReadOnlyAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Response<List<string>>> ListAvailableLanguagesAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Response<Dictionary<string, string>>> LoadDictionaryAsync(string languageCode)
		{
			throw new NotImplementedException();
		}

		public Task<Result> RemoveTranslation(string lanugageCode, string key)
		{
			throw new NotImplementedException();
		}

		public Task<Result> SaveDictionaryAsync(string languageCode, Dictionary<string, string> translations)
		{
			throw new NotImplementedException();
		}

		public Task<Result> UpdateTranslation(string languageCode, string key, string value)
		{
			throw new NotImplementedException();
		}
	}
}
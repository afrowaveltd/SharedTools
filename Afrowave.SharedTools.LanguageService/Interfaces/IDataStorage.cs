using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.I18N.Interfaces
{
	public interface IDataStorage
	{
		public Task<Result> SaveDictionaryAsync(string languageCode, Dictionary<string, string> translations);
		public Task<Response<Dictionary<string, string>>> LoadDictionaryAsync(string languageCode);
		public Task<Response<List<string>>> ListAvailableLanguagesAsync();
		public Task<Result> DeleteDictionaryAsync(string languageCode);
		public Task<bool> DictionaryExistsAsync(string languageCode);
		public Task<bool> IsReadOnlyAsync();
		public Task<Response<string>> GetTranslation (string languageCode, string key);

   }
}

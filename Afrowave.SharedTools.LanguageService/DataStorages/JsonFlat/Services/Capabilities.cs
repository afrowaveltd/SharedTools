using Afrowave.SharedTools.I18N.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Afrowave.SharedTools.I18N.DataStorages.JsonFlat.Services
{
	/// <summary>
	/// Provides access to the set of supported data storage capabilities for the current context.
	/// </summary>
	/// <remarks>Use this class to query which operations are available, such as reading, writing, or deleting data,
	/// and to determine the supported dictionary format. The capabilities returned may be used to conditionally enable or
	/// disable features in client code.</remarks>
	public class Capabilities : ICapabilities
	{
		/// <summary>
		/// Retrieves the set of capabilities supported by the data storage provider.
		/// </summary>
		/// <remarks>Use this method to determine which operations are available before attempting data storage
		/// actions. The returned capabilities reflect the current configuration and may affect which features can be used
		/// safely.</remarks>
		/// <returns>A <see cref="DataStorageCapabilities"/> object describing the supported operations, such as reading, writing,
		/// deleting, and listing languages.</returns>
		public DataStorageCapabilities GetCapabilities()
		{
			return new DataStorageCapabilities
			{
				CanRead = true,
				CanCreate = true,
				CanUpdate = true,
				CanDelete = true,
				CanListLanguages = true,
				CanCheckExistence = true,
				CanReportDictionaryChanges = true,
				IsReadOnly = false,
				DictionaryFormat = DictionaryFormat.JSON_FLAT
			};
		}
	}
}
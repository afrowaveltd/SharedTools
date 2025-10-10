using System;
using System.Collections.Generic;
using System.Text;

namespace Afrowave.SharedTools.I18N.Models
{
	/// <summary>
	/// Represents the capabilities of a data storage provider for language resources.
	/// </summary>
	public class DataStorageCapabilities
	{
		/// <summary>
		/// Indicates whether the storage can read data.
		/// </summary>
		public bool CanRead { get; set; } = true;

		/// <summary>
		/// Gets or sets a value indicating whether new items can be created.
		/// </summary>
		public bool CanCreate { get; set; } = true;

		/// <summary>
		/// Gets or sets a value indicating whether updates are permitted.
		/// </summary>
		public bool CanUpdate { get; set; } = true;

		/// <summary>
		/// Indicates whether the storage can delete data.
		/// </summary>
		public bool CanDelete { get; set; } = true;

		/// <summary>
		/// Indicates whether the storage can list available languages.
		/// </summary>
		public bool CanListLanguages { get; set; } = true;

		/// <summary>
		/// Indicates whether the storage can check if a resource exists.
		/// </summary>
		public bool CanCheckExistence { get; set; } = true;

		/// <summary>
		/// Indicates whether the storage can report changes to dictionaries.
		/// </summary>
		public bool CanReportDictionaryChanges { get; set; } = false;

		/// <summary>
		/// Indicates whether the storage is read-only.
		/// </summary>
		public bool IsReadOnly { get; set; } = false;

		/// <summary>
		/// Gets or sets the format used to serialize dictionaries in the output.
		/// </summary>
		public DictionaryFormat DictionaryFormat { get; set; } = DictionaryFormat.JSON_FLAT;
	}
}
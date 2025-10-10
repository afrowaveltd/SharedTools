using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Afrowave.SharedTools.I18N.DataStorages.JsonFlat.Models
{
	/// <summary>
	/// Provides configuration options for initializing and managing a flat JSON-based data storage, including locale file
	/// location and creation behavior.
	/// </summary>
	/// <remarks>Use this class to specify settings such as the path to locale files and whether the storage should
	/// be created automatically if it does not exist. This type is intended to be used when configuring components that
	/// rely on flat JSON files for data persistence.</remarks>
	public sealed class JsonFlatDataStorageOptions
	{
		/// <summary>
		/// Gets or sets the file system path to the directory containing locale resources.
		/// </summary>
		public string LocalesPath { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets a value indicating whether the resource should be created if it does not already exist.
		/// </summary>
		/// <remarks>Set this property to <see langword="true"/> to automatically create the resource when performing
		/// operations that require its existence. If set to <see langword="false"/>, an exception may be thrown if the
		/// resource is missing.</remarks>
		public bool CreateIfNotExists { get; set; } = true;

		/// <summary>
		/// Initializes a new instance of the JsonFlatDataStorageOptions class with default settings.
		/// </summary>
		/// <remarks>The default constructor sets the LocalesPath property to the 'Locales' directory located at the
		/// application's base path. This ensures that locale data is loaded from a consistent location relative to the
		/// application's root directory.</remarks>
		public JsonFlatDataStorageOptions()
		{
			LocalesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory
				[..AppDomain.CurrentDomain.BaseDirectory
				.IndexOf("bin")], "Locales");
		}
	}
}
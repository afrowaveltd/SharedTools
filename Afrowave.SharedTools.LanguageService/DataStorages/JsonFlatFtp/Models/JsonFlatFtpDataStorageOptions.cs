using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Afrowave.SharedTools.Ftp.Options;

namespace Afrowave.SharedTools.I18N.DataStorages.JsonFlatFtp.Models
{
	/// <summary>
	/// Represents configuration options for storing data as flat JSON files on an FTP server.
	/// </summary>
	/// <remarks>Use this class to specify FTP connection settings and storage behavior when working with JSON-based
	/// data storage over FTP. All properties must be set appropriately before use. This class is not
	/// thread-safe.</remarks>
	public sealed class JsonFlatFtpDataStorageOptions
	{
		/// <summary>
		/// Gets or sets the FTP configuration options used for connecting to an FTP server.
		/// </summary>
		public FtpOptions FtpOptions { get; set; } = new FtpOptions();

		/// <summary>
		/// Gets or sets a value indicating whether the resource should be created if it does not already exist.
		/// </summary>
		public bool CreateIfNotExists { get; set; } = false;

		/// <summary>
		/// Gets or sets the default language code used for localization or content display.
		/// </summary>
		public string DefaultLanguage { get; set; } = "en";
	}
}
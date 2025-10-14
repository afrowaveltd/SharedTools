using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Afrowave.SharedTools.Ftp.Options
{
	/// <summary>
	/// Represents configuration settings for establishing and managing FTP/FTPS connections.
	/// </summary>
	/// <remarks>
	/// These options control connection details (host, port, base path, credentials), behavior (passive mode,
	/// SSL, keep-alive, transfer mode), timeouts, and retry policy. Use this type to configure an FTP client or service.
	/// </remarks>
	public sealed class FtpOptions
	{
		/// <summary>
		/// FTP server host name or IP address.
		/// </summary>
		public string Host { get; set; } = string.Empty;

		/// <summary>
		/// FTP server port number. Default is 21.
		/// </summary>
		public int Port { get; set; } = 21;

		/// <summary>
		/// Base path (prefix) used for all remote paths. Defaults to "/".
		/// </summary>
		public string BasePath { get; set; } = "/";

		/// <summary>
		/// Credentials used to authenticate against the FTP server.
		/// </summary>
		public NetworkCredential Credentials { get; set; } = new NetworkCredential();

		/// <summary>
		/// Enables explicit FTPS when set to true. When false, plain FTP is used.
		/// </summary>
		public bool EnableSsl { get; set; } = false;

		/// <summary>
		/// Uses passive mode data connections when true. Recommended for most scenarios.
		/// </summary>
		public bool UsePassive { get; set; } = true;

		/// <summary>
		/// Keeps the control connection alive between requests when true.
		/// </summary>
		public bool KeepAlive { get; set; } = false;

		/// <summary>
		/// Transfers data in binary mode when true; otherwise text mode.
		/// </summary>
		public bool UseBinary { get; set; } = true;

		/// <summary>
		/// Connection timeout in milliseconds applied to request/response operations. Default is 15000 ms.
		/// </summary>
		public int ConnectTimeoutMs { get; set; } = 15000;

		/// <summary>
		/// Read and write timeout in milliseconds for data transfers. Default is 30000 ms.
		/// </summary>
		public int ReadWriteTimeoutMs { get; set; } = 30000;

		/// <summary>
		/// Retry policy configuration applied to transient failures during FTP operations.
		/// </summary>
		public RetryPolicyOptions Retry { get; set; } = new RetryPolicyOptions();

		/// <summary>
		/// Builds an absolute FTP/FTPS URI by combining the configured host, port, base path and the provided relative path.
		/// </summary>
		/// <param name="path">Relative path to append to <see cref="BasePath"/>. Leading slashes are ignored.</param>
		/// <returns>An absolute <see cref="Uri"/> pointing to the remote resource.</returns>
		public Uri BuildUri(string path)
		{
			var p = (BasePath ?? "/").TrimEnd('/') + "/" + (path ?? string.Empty).TrimStart('/');
			var scheme = EnableSsl ? "ftps" : "ftp";
			return new Uri($"{scheme}://{Host}:{Port}{p}");
		}
	}
}

using Afrowave.SharedTools.Ftp.Options;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Afrowave.SharedTools.Ftp.Services; // reuse internal logic

namespace Afrowave.SharedTools.Ftp.Static
{
	/// <summary>
	/// Static convenience API for executing FTP/FTPS operations without setting up dependency injection.
	/// </summary>
	/// <remarks>
	/// Each method creates an internal <see cref="FtpService"/> instance using the supplied <see cref="FtpOptions"/>, performs the requested operation, and returns the result. Use this helper for simple, one-off scenarios.
	/// For applications with DI, prefer injecting and reusing <see cref="IFtpService"/> instead of this helper.
	/// </remarks>
	public static class FtpClientHelper
	{
		/// <summary>
		/// Creates a lightweight shim wrapping <see cref="FtpService"/> constructed from the provided options.
		/// </summary>
		/// <param name="opt">FTP configuration options used to construct the underlying service.</param>
		/// <returns>A shim exposing async FTP methods.</returns>
		private static IFtpShim Create(FtpOptions opt) => new IFtpShim(new FtpService(opt));

		/// <summary>
		/// Lists items in the specified remote directory.
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="remotePath">Remote directory path to list.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A response with item names contained in the directory.</returns>
		public static Task<Response<IReadOnlyList<string>>> ListAsync(FtpOptions opt, string remotePath, CancellationToken ct = default)
			 => Create(opt).ListAsync(remotePath, ct);

		/// <summary>
		/// Downloads a remote file as a byte array.
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="remotePath">Remote file path to download.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A response containing the downloaded bytes.</returns>
		public static Task<Response<byte[]>> DownloadBytesAsync(FtpOptions opt, string remotePath, CancellationToken ct = default)
			 => Create(opt).DownloadBytesAsync(remotePath, ct);

		/// <summary>
		/// Downloads a remote file and saves it to a local file path.
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="remotePath">Remote file path to download.</param>
		/// <param name="localFilePath">Destination local file path.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		public static Task<Result> DownloadToFileAsync(FtpOptions opt, string remotePath, string localFilePath, CancellationToken ct = default)
			 => Create(opt).DownloadToFileAsync(remotePath, localFilePath, ct);

		/// <summary>
		/// Uploads the specified bytes to a remote path.
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="remotePath">Destination remote file path.</param>
		/// <param name="data">Content to upload.</param>
		/// <param name="overwrite">When false, the upload fails if the target exists.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		public static Task<Result> UploadBytesAsync(FtpOptions opt, string remotePath, byte[] data, bool overwrite = true, CancellationToken ct = default)
			 => Create(opt).UploadBytesAsync(remotePath, data, overwrite, ct);

		/// <summary>
		/// Uploads a local file to a remote path.
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="localFilePath">Source local file path.</param>
		/// <param name="remotePath">Destination remote file path.</param>
		/// <param name="overwrite">When false, the upload fails if the target exists.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		public static Task<Result> UploadFileAsync(FtpOptions opt, string localFilePath, string remotePath, bool overwrite = true, CancellationToken ct = default)
			 => Create(opt).UploadFileAsync(localFilePath, remotePath, overwrite, ct);

		/// <summary>
		/// Deletes a file at the specified remote path.
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="remotePath">Remote file path to delete.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		public static Task<Result> DeleteFileAsync(FtpOptions opt, string remotePath, CancellationToken ct = default)
			 => Create(opt).DeleteFileAsync(remotePath, ct);

		/// <summary>
		/// Creates a directory at the specified remote path.
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="remotePath">Remote directory path to create.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		public static Task<Result> CreateDirectoryAsync(FtpOptions opt, string remotePath, CancellationToken ct = default)
			 => Create(opt).CreateDirectoryAsync(remotePath, ct);

		/// <summary>
		/// Deletes a directory at the specified remote path.
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="remotePath">Remote directory path to remove.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		public static Task<Result> DeleteDirectoryAsync(FtpOptions opt, string remotePath, CancellationToken ct = default)
			 => Create(opt).DeleteDirectoryAsync(remotePath, ct);

		/// <summary>
		/// Retrieves the size of the remote file (in bytes).
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="remotePath">Remote file path.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A response with the file size in bytes.</returns>
		public static Task<Response<long>> GetFileSizeAsync(FtpOptions opt, string remotePath, CancellationToken ct = default)
			 => Create(opt).GetFileSizeAsync(remotePath, ct);

		/// <summary>
		/// Retrieves the last modification time of the remote file.
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="remotePath">Remote file path.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A response containing the last modified <see cref="DateTime"/>.</returns>
		public static Task<Response<DateTime>> GetModifiedTimeAsync(FtpOptions opt, string remotePath, CancellationToken ct = default)
			 => Create(opt).GetModifiedTimeAsync(remotePath, ct);

		/// <summary>
		/// Checks whether a file or directory exists at the specified remote path.
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="remotePath">Remote path to check.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A response indicating whether the remote item exists.</returns>
		public static Task<Response<bool>> ExistsAsync(FtpOptions opt, string remotePath, CancellationToken ct = default)
			 => Create(opt).ExistsAsync(remotePath, ct);

		/// <summary>
		/// Renames a remote file or directory.
		/// </summary>
		/// <param name="opt">FTP configuration options.</param>
		/// <param name="remotePath">Current remote path.</param>
		/// <param name="newName">New name for the item.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		public static Task<Result> RenameAsync(FtpOptions opt, string remotePath, string newName, CancellationToken ct = default)
			 => Create(opt).RenameAsync(remotePath, newName, ct);

		/// <summary>
		/// Private shim proxy around <see cref="FtpService"/> to keep the helper succinct.
		/// </summary>
		private sealed class IFtpShim
		{
			private readonly FtpService _inner;

			/// <summary>
			/// Initializes the shim with an underlying <see cref="FtpService"/> instance.
			/// </summary>
			/// <param name="inner">The service to delegate calls to.</param>
			public IFtpShim(FtpService inner)
			{ _inner = inner; }

			public Task<Response<IReadOnlyList<string>>> ListAsync(string path, CancellationToken ct) => _inner.ListAsync(path, ct);
			public Task<Response<byte[]>> DownloadBytesAsync(string path, CancellationToken ct) => _inner.DownloadBytesAsync(path, ct);
			public Task<Result> DownloadToFileAsync(string path, string file, CancellationToken ct) => _inner.DownloadToFileAsync(path, file, ct);
			public Task<Result> UploadBytesAsync(string path, byte[] data, bool overwrite, CancellationToken ct) => _inner.UploadBytesAsync(path, data, overwrite, ct);
			public Task<Result> UploadFileAsync(string local, string remote, bool overwrite, CancellationToken ct) => _inner.UploadFileAsync(local, remote, overwrite, ct);
			public Task<Result> DeleteFileAsync(string path, CancellationToken ct) => _inner.DeleteFileAsync(path, ct);
			public Task<Result> CreateDirectoryAsync(string path, CancellationToken ct) => _inner.CreateDirectoryAsync(path, ct);
			public Task<Result> DeleteDirectoryAsync(string path, CancellationToken ct) => _inner.DeleteDirectoryAsync(path, ct);
			public Task<Response<long>> GetFileSizeAsync(string path, CancellationToken ct) => _inner.GetFileSizeAsync(path, ct);
			public Task<Response<DateTime>> GetModifiedTimeAsync(string path, CancellationToken ct) => _inner.GetModifiedTimeAsync(path, ct);
			public Task<Response<bool>> ExistsAsync(string path, CancellationToken ct) => _inner.ExistsAsync(path, ct);
			public Task<Result> RenameAsync(string path, string newName, CancellationToken ct) => _inner.RenameAsync(path, newName, ct);
		}
	}
}
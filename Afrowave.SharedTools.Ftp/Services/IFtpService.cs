using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Ftp.Services
{
	/// <summary>
	/// Defines a contract for basic FTP/FTPS operations such as listing, uploading, downloading,
	/// deleting, renaming files, and directory management.
	/// </summary>
	public interface IFtpService
	{
		/// <summary>
		/// Lists items in the specified remote directory.
		/// </summary>
		/// <param name="remotePath">Remote directory path to list.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A response with the list of item names in the remote directory.</returns>
		Task<Response<IReadOnlyList<string>>> ListAsync(string remotePath, CancellationToken ct = default);

		/// <summary>
		/// Downloads a remote file as a byte array.
		/// </summary>
		/// <param name="remotePath">Remote file path to download.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A response containing the downloaded bytes.</returns>
		Task<Response<byte[]>> DownloadBytesAsync(string remotePath, CancellationToken ct = default);

		/// <summary>
		/// Downloads a remote file and saves it to a local file path.
		/// </summary>
		/// <param name="remotePath">Remote file path to download.</param>
		/// <param name="localFilePath">Local file path to write the downloaded content to.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		Task<Result> DownloadToFileAsync(string remotePath, string localFilePath, CancellationToken ct = default);

		/// <summary>
		/// Uploads the specified bytes to a remote path.
		/// </summary>
		/// <param name="remotePath">Destination remote file path.</param>
		/// <param name="data">Content to upload.</param>
		/// <param name="overwrite">When false, the upload fails if the target exists.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		Task<Result> UploadBytesAsync(string remotePath, byte[] data, bool overwrite = true, CancellationToken ct = default);

		/// <summary>
		/// Uploads a local file to a remote path.
		/// </summary>
		/// <param name="localFilePath">Source local file path.</param>
		/// <param name="remotePath">Destination remote file path.</param>
		/// <param name="overwrite">When false, the upload fails if the target exists.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		Task<Result> UploadFileAsync(string localFilePath, string remotePath, bool overwrite = true, CancellationToken ct = default);

		/// <summary>
		/// Deletes a file at the specified remote path.
		/// </summary>
		/// <param name="remotePath">Remote file path to delete.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		Task<Result> DeleteFileAsync(string remotePath, CancellationToken ct = default);

		/// <summary>
		/// Creates a directory at the specified remote path.
		/// </summary>
		/// <param name="remotePath">Remote directory path to create.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		Task<Result> CreateDirectoryAsync(string remotePath, CancellationToken ct = default);

		/// <summary>
		/// Deletes the directory at the specified remote path.
		/// </summary>
		/// <param name="remotePath">Remote directory path to remove.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		Task<Result> DeleteDirectoryAsync(string remotePath, CancellationToken ct = default);

		/// <summary>
		/// Retrieves the size of the remote file in bytes.
		/// </summary>
		/// <param name="remotePath">Remote file path.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A response with the file size in bytes.</returns>
		Task<Response<long>> GetFileSizeAsync(string remotePath, CancellationToken ct = default);

		/// <summary>
		/// Retrieves the last modification time of the remote file.
		/// </summary>
		/// <param name="remotePath">Remote file path.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A response containing the last modified <see cref="DateTime"/>.</returns>
		Task<Response<DateTime>> GetModifiedTimeAsync(string remotePath, CancellationToken ct = default);

		/// <summary>
		/// Checks whether a file or directory exists at the specified remote path.
		/// </summary>
		/// <param name="remotePath">Remote path to check.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A response indicating whether the remote item exists.</returns>
		Task<Response<bool>> ExistsAsync(string remotePath, CancellationToken ct = default);

		/// <summary>
		/// Renames a remote file or directory.
		/// </summary>
		/// <param name="remotePath">Current remote path.</param>
		/// <param name="newName">New name for the item.</param>
		/// <param name="ct">Optional cancellation token.</param>
		/// <returns>A result indicating success or failure.</returns>
		Task<Result> RenameAsync(string remotePath, string newName, CancellationToken ct = default);
	}
}
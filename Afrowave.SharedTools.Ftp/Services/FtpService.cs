using Afrowave.SharedTools.Ftp.Options;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Ftp.Services
{
	/// <summary>
	/// Provides an FTP/FTPS client with common file and directory operations.
	/// </summary>
	/// <remarks>
	/// This service wraps <see cref="FtpWebRequest"/> to perform listing, upload, download, deletion,
	/// renaming, and metadata queries with a retry policy defined by <see cref="FtpOptions.Retry"/>.
	/// </remarks>
	public sealed class FtpService : IFtpService
	{
		private readonly FtpOptions _opt;
		private readonly Random _rnd = new Random();

		/// <summary>
		/// Initializes a new instance of the <see cref="FtpService"/> class with the provided options.
		/// </summary>
		/// <param name="options">FTP configuration options. <see cref="FtpOptions.Host"/> must be set.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown when <see cref="FtpOptions.Host"/> is not provided.</exception>
		public FtpService(FtpOptions options)
		{
			_opt = options ?? throw new ArgumentNullException(nameof(options));
			if(string.IsNullOrWhiteSpace(_opt.Host))
				throw new ArgumentException("FtpOptions.Host is required.");
		}

		// ------------ Public API ------------

		/// <inheritdoc/>
		public Task<Response<IReadOnlyList<string>>> ListAsync(string remotePath, CancellationToken ct = default)
			 => ExecWithRetry(async () =>
			 {
				 var req = CreateRequest(remotePath, WebRequestMethods.Ftp.ListDirectory);
				 using(var resp = (FtpWebResponse)await GetResponseAsync(req, ct).ConfigureAwait(false))
				 using(var stream = resp.GetResponseStream())
				 using(var reader = new StreamReader(stream, Encoding.UTF8))
				 {
					 var all = await reader.ReadToEndAsync().ConfigureAwait(false);
					 var lines = all.Replace("\r", "").Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
					 return Response<IReadOnlyList<string>>.SuccessResponse(lines, "OK");
				 }
			 });

		/// <inheritdoc/>
		public Task<Response<byte[]>> DownloadBytesAsync(string remotePath, CancellationToken ct = default)
			 => ExecWithRetry(async () =>
			 {
				 var req = CreateRequest(remotePath, WebRequestMethods.Ftp.DownloadFile);
				 using(var resp = (FtpWebResponse)await GetResponseAsync(req, ct).ConfigureAwait(false))
				 using(var stream = resp.GetResponseStream())
				 using(var ms = new MemoryStream())
				 {
					 await stream.CopyToAsync(ms).ConfigureAwait(false);
					 return Response<byte[]>.SuccessResponse(ms.ToArray(), "OK");
				 }
			 });

		/// <inheritdoc/>
		public Task<Result> DownloadToFileAsync(string remotePath, string localFilePath, CancellationToken ct = default)
			 => ExecWithRetry(async () =>
			 {
				 var req = CreateRequest(remotePath, WebRequestMethods.Ftp.DownloadFile);
				 using(var resp = (FtpWebResponse)await GetResponseAsync(req, ct).ConfigureAwait(false))
				 using(var stream = resp.GetResponseStream())
				 using(var fs = File.Create(localFilePath))
				 {
					 await stream.CopyToAsync(fs).ConfigureAwait(false);
					 return Result.Ok("Downloaded.");
				 }
			 });

		/// <inheritdoc/>
		public Task<Result> UploadBytesAsync(string remotePath, byte[] data, bool overwrite = true, CancellationToken ct = default)
			 => ExecWithRetry(async () =>
			 {
				 if(!overwrite)
				 {
					 var ex = await ExistsAsync(remotePath, ct).ConfigureAwait(false);
					 if(ex.Success && ex.Data) return Result.Fail("File already exists.");
				 }

				 var req = CreateRequest(remotePath, WebRequestMethods.Ftp.UploadFile);
				 req.ContentLength = data?.LongLength ?? 0L;

				 using(var rs = await GetRequestStreamAsync(req, ct).ConfigureAwait(false))
				 {
					 if(data != null && data.Length > 0)
						 await rs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
				 }

				 using(var resp = (FtpWebResponse)await GetResponseAsync(req, ct).ConfigureAwait(false)) { }
				 return Result.Ok("Uploaded.");
			 });

		/// <inheritdoc/>
		public Task<Result> UploadFileAsync(string localFilePath, string remotePath, bool overwrite = true, CancellationToken ct = default)
			 => ExecWithRetry(async () =>
			 {
				 if(!File.Exists(localFilePath)) return Result.Fail("Local file not found.");

				 if(!overwrite)
				 {
					 var ex = await ExistsAsync(remotePath, ct).ConfigureAwait(false);
					 if(ex.Success && ex.Data) return Result.Fail("File already exists.");
				 }

				 var bytes = await File.ReadAllBytesAsync(localFilePath, ct).ConfigureAwait(false);
				 return await UploadBytesAsync(remotePath, bytes, true, ct).ConfigureAwait(false);
			 });

		/// <inheritdoc/>
		public Task<Result> DeleteFileAsync(string remotePath, CancellationToken ct = default)
			 => ExecWithRetry(async () =>
			 {
				 var req = CreateRequest(remotePath, WebRequestMethods.Ftp.DeleteFile);
				 using(var resp = (FtpWebResponse)await GetResponseAsync(req, ct).ConfigureAwait(false)) { }
				 return Result.Ok("Deleted.");
			 });

		/// <inheritdoc/>
		public Task<Result> CreateDirectoryAsync(string remotePath, CancellationToken ct = default)
			 => ExecWithRetry(async () =>
			 {
				 var req = CreateRequest(remotePath, WebRequestMethods.Ftp.MakeDirectory);
				 using(var resp = (FtpWebResponse)await GetResponseAsync(req, ct).ConfigureAwait(false)) { }
				 return Result.Ok("Directory created.");
			 });

		/// <inheritdoc/>
		public Task<Result> DeleteDirectoryAsync(string remotePath, CancellationToken ct = default)
			 => ExecWithRetry(async () =>
			 {
				 var req = CreateRequest(remotePath, WebRequestMethods.Ftp.RemoveDirectory);
				 using(var resp = (FtpWebResponse)await GetResponseAsync(req, ct).ConfigureAwait(false)) { }
				 return Result.Ok("Directory removed.");
			 });

		/// <inheritdoc/>
		public Task<Response<long>> GetFileSizeAsync(string remotePath, CancellationToken ct = default)
			 => ExecWithRetry(async () =>
			 {
				 var req = CreateRequest(remotePath, WebRequestMethods.Ftp.GetFileSize);
				 using(var resp = (FtpWebResponse)await GetResponseAsync(req, ct).ConfigureAwait(false))
				 {
					 return Response<long>.SuccessResponse(req.ContentLength >= 0 ? req.ContentLength : 0, "OK");
				 }
			 });

		/// <inheritdoc/>
		public Task<Response<DateTime>> GetModifiedTimeAsync(string remotePath, CancellationToken ct = default)
			 => ExecWithRetry(async () =>
			 {
				 var req = CreateRequest(remotePath, WebRequestMethods.Ftp.GetDateTimestamp);
				 using(var resp = (FtpWebResponse)await GetResponseAsync(req, ct).ConfigureAwait(false))
				 {
					 return Response<DateTime>.SuccessResponse(resp.LastModified, "OK");
				 }
			 });

		/// <inheritdoc/>
		public async Task<Response<bool>> ExistsAsync(string remotePath, CancellationToken ct = default)
		{
			var size = TryRequest(() => GetFileSizeAsync(remotePath, ct));
			if(size.Item1) return Response<bool>.SuccessResponse(true, "OK");

			var time = TryRequest(() => GetModifiedTimeAsync(remotePath, ct));
			if(time.Item1) return Response<bool>.SuccessResponse(true, "OK");

			var parent = GetParentPath(remotePath);
			var name = GetName(remotePath);
			var lst = TryRequest(() => ListAsync(parent, ct));
			if(lst.Item1 && lst.Item2 != null)
			{
				foreach(var item in lst.Item2)
					if(string.Equals(item.Trim(), name, StringComparison.Ordinal))
						return Response<bool>.SuccessResponse(true, "OK");
			}

			return Response<bool>.SuccessResponse(false, "OK");
		}

		/// <inheritdoc/>
		public Task<Result> RenameAsync(string remotePath, string newName, CancellationToken ct = default)
			 => ExecWithRetry(async () =>
			 {
				 var req = CreateRequest(remotePath, WebRequestMethods.Ftp.Rename);
				 req.RenameTo = newName;
				 using(var resp = (FtpWebResponse)await GetResponseAsync(req, ct).ConfigureAwait(false)) { }
				 return Result.Ok("Renamed.");
			 });

		// ------------ Internals ------------

		private FtpWebRequest CreateRequest(string path, string method)
		{
			var uri = _opt.BuildUri(path);
			var req = (FtpWebRequest)WebRequest.Create(uri);
			req.Method = method;
			req.Credentials = _opt.Credentials;
			req.EnableSsl = _opt.EnableSsl;
			req.UsePassive = _opt.UsePassive;
			req.KeepAlive = _opt.KeepAlive;
			req.UseBinary = _opt.UseBinary;
			req.Timeout = _opt.ConnectTimeoutMs;        // ms
			req.ReadWriteTimeout = _opt.ReadWriteTimeoutMs;
			return req;
		}

		private static Task<Stream> GetRequestStreamAsync(FtpWebRequest req, CancellationToken ct)
		{
			// FtpWebRequest nemá moderní ct-aware async v .NET Standard — zabalíme do Task.Run
			return Task.Run<Stream>(() => req.GetRequestStream(), ct);
		}

		private static Task<WebResponse> GetResponseAsync(FtpWebRequest req, CancellationToken ct)
		{
			return Task.Run<WebResponse>(() => req.GetResponse(), ct);
		}

		private async Task<T> ExecWithRetry<T>(Func<Task<T>> action)
		{
			int retries = _opt.Retry.MaxRetries;
			var delay = _opt.Retry.BaseDelay;

			for(int attempt = 0; attempt <= retries; attempt++)
			{
				try
				{
					return await action().ConfigureAwait(false);
				}
				catch(WebException ex)
				{
					if(attempt == retries) return FailFromException<T>(ex);
				}
				catch(Exception ex)
				{
					if(attempt == retries) return FailFromException<T>(ex);
				}

				await Task.Delay(AddJitter(delay, _opt.Retry.Jitter)).ConfigureAwait(false);
				delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * _opt.Retry.BackoffFactor);
			}

			return default(T);
		}

		private Tuple<bool, T, string> TryRequest<T>(Func<Task<Response<T>>> call)
		{
			try
			{
				var t = call().GetAwaiter().GetResult();
				return new Tuple<bool, T, string>(t.Success, t.Data, t.Message);
			}
			catch(WebException ex)
			{
				return new Tuple<bool, T, string>(false, default(T), ex.Message);
			}
			catch(Exception ex)
			{
				return new Tuple<bool, T, string>(false, default(T), ex.Message);
			}
		}

		private static T FailFromException<T>(Exception ex)
		{
			if(typeof(T) == typeof(Result))
				return (T)(object)Result.Fail(ex.Message);

			var rt = typeof(T);
			if(rt.IsGenericType && rt.GetGenericTypeDefinition().FullName == "Afrowave.SharedTools.Models.Results.Response`1")
			{
				var fail = rt.GetMethod("Fail", new[] { typeof(string) });
				return (T)fail.Invoke(null, new object[] { ex.Message });
			}
			return default(T);
		}

		private static TimeSpan AddJitter(TimeSpan baseDelay, bool jitter)
		{
			if(!jitter) return baseDelay;
			var r = new Random();
			var factor = r.NextDouble() * 0.4 + 0.8;
			return TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * factor);
		}

		private static string GetParentPath(string path)
		{
			path = string.IsNullOrEmpty(path) ? "/" : path.Replace("\\", "/");
			var idx = path.LastIndexOf('/');
			if(idx <= 0) return "/";
			return path.Substring(0, idx);
		}

		private static string GetName(string path)
		{
			path = (path ?? "").Replace("\\", "/");
			var idx = path.LastIndexOf('/');
			return idx >= 0 ? path.Substring(idx + 1) : path;
		}
	}
}
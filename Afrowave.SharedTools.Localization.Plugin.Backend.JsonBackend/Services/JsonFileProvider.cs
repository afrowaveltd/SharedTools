using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Services
{
	/// <summary>
	/// Provides asynchronous file operations for reading and writing translation files.
	/// </summary>
	public sealed class JsonFileProvider
	{
		private readonly ILogger<JsonFileProvider> _logger;

		public JsonFileProvider(ILogger<JsonFileProvider> logger)
		{
			_logger = logger;
		}

		public async Task<string> ReadFileAsync(string filePath)
		{
			if(!File.Exists(filePath))
			{
				_logger.LogWarning("Translation file not found: {FilePath}", filePath);
				return string.Empty;
			}

			_logger.LogInformation("Reading translation file: {FilePath}", filePath);
			using var reader = new StreamReader(filePath, Encoding.UTF8);
			return await reader.ReadToEndAsync();
		}

		public async Task WriteFileAsync(string filePath, string content)
		{
			var dir = Path.GetDirectoryName(filePath);
			if(!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir!);
				_logger.LogInformation("Created directory: {Directory}", dir);
			}

			_logger.LogInformation("Writing translation file: {FilePath}", filePath);
			using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
			await writer.WriteAsync(content);
		}

		public Task<bool> FileExistsAsync(string filePath)
			 => Task.FromResult(File.Exists(filePath));

		public Task<string[]> GetFilesAsync(string folderPath, string searchPattern)
		{
			if(!Directory.Exists(folderPath))
			{
				_logger.LogWarning("Translation folder not found: {Folder}", folderPath);
				return Task.FromResult(new string[0]);
			}

			var files = Directory.GetFiles(folderPath, searchPattern);
			_logger.LogInformation("Found {Count} files matching pattern '{Pattern}' in {Folder}", files.Length, searchPattern, folderPath);
			return Task.FromResult(files);
		}
	}
}
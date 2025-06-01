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

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonFileProvider"/> class.
		/// </summary>
		/// <param name="logger">The logger used to record diagnostic information and errors related to the operations of the <see
		/// cref="JsonFileProvider"/>.</param>
		public JsonFileProvider(ILogger<JsonFileProvider> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Asynchronously reads the contents of a file at the specified path.
		/// </summary>
		/// <remarks>If the specified file does not exist, the method logs a warning and returns an empty string.
		/// Otherwise, the method logs an informational message and reads the file using UTF-8 encoding.</remarks>
		/// <param name="filePath">The path to the file to be read. Must be a valid file path.</param>
		/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.  The result contains the contents of the
		/// file as a string, or an empty string if the file does not exist.</returns>
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

		/// <summary>
		/// Asynchronously writes the specified content to a file at the given file path.
		/// </summary>
		/// <remarks>If the directory specified in <paramref name="filePath"/> does not exist, it will be created
		/// automatically. The file will be overwritten if it already exists. The content is written using UTF-8
		/// encoding.</remarks>
		/// <param name="filePath">The full path of the file to write to. If the directory does not exist, it will be created.</param>
		/// <param name="content">The content to write to the file.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
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

		/// <summary>
		/// Determines whether the specified file exists.
		/// </summary>
		/// <remarks>This method performs a synchronous check for file existence and wraps the result in a completed
		/// task. It is suitable for use in asynchronous workflows but does not perform actual asynchronous I/O.</remarks>
		/// <param name="filePath">The full path of the file to check. This parameter cannot be null or empty.</param>
		/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.  The result is <see langword="true"/> if
		/// the file exists; otherwise, <see langword="false"/>.</returns>
		public Task<bool> FileExistsAsync(string filePath)
			 => Task.FromResult(File.Exists(filePath));

		/// <summary>
		/// Asynchronously retrieves the file paths in the specified folder that match the given search pattern.
		/// </summary>
		/// <remarks>This method does not perform recursive searches; only files in the specified folder are included.
		/// Ensure that <paramref name="folderPath"/> points to an existing directory to avoid receiving an empty
		/// result.</remarks>
		/// <param name="folderPath">The path to the folder where the files will be searched. Must be a valid directory path.</param>
		/// <param name="searchPattern">The search pattern to match file names, such as "*.txt" for text files or "*.*" for all files.</param>
		/// <returns>A task that represents the asynchronous operation. The result contains an array of file paths matching the search
		/// pattern. If the specified folder does not exist, the result will be an empty array.</returns>
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
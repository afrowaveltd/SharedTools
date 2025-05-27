using Afrowave.SharedTools.Models.Localization;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Helpers
{
	/// <summary>
	/// Provides utility methods for locating, validating and repairing localization backend definitions.
	/// </summary>
	public static class LocalizationBackendHelper
	{
		private const string BackendIdFileName = ".afrowave-backend-id.txt";
		private const string CapabilitiesFileName = "capabilities.json";

		/// <summary>
		/// Finds the folder of a backend by scanning directories recursively from a given root.
		/// </summary>
		/// <param name="backendName">The internal backend name (e.g. JsonLocalizationBackend).</param>
		/// <param name="packageName">The NuGet package name (e.g. Afrowave.JsonLocalizationBackend).</param>
		/// <param name="searchRoot">Optional starting directory. If null, uses AppContext.BaseDirectory.</param>
		/// <returns>Absolute path to the backend folder or null if not found.</returns>
		public static async Task<string?> FindBackendFolderAsync(string backendName, string packageName, string? searchRoot = null)
		{
			var root = searchRoot ?? AppContext.BaseDirectory;

			foreach(var dir in Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories))
			{
				var idPath = Path.Combine(dir, BackendIdFileName);
				if(!File.Exists(idPath))
					continue;

				var lines = await File.ReadAllLinesAsync(idPath);
				if(lines.Length < 2)
					continue;

				var foundBackendName = lines[0].Trim();
				var foundPackageName = lines[1].Trim();

				if(foundBackendName == backendName && foundPackageName == packageName)
					return dir;
			}

			return null;
		}

		/// <summary>
		/// Writes or repairs the capabilities.json file for a given backend.
		/// </summary>
		/// <param name="backendName">The internal backend name.</param>
		/// <param name="packageName">The NuGet package name.</param>
		/// <param name="capabilities">The capabilities definition to write.</param>
		/// <param name="searchRoot">Optional root directory to start scanning for the backend.</param>
		public static async Task FixBackendCapabilitiesAsync(
			 string backendName,
			 string packageName,
			 LocalizationBackendCapabilities capabilities,
			 string? searchRoot = null)
		{
			var backendFolder = await FindBackendFolderAsync(backendName, packageName, searchRoot);

			if(backendFolder is null)
				throw new InvalidOperationException($"Could not find backend folder for {backendName} ({packageName}).");

			var idFilePath = Path.Combine(backendFolder, BackendIdFileName);
			var idLines = await File.ReadAllLinesAsync(idFilePath);

			if(idLines.Length < 2 ||
				 idLines[0].Trim() != backendName ||
				 idLines[1].Trim() != packageName)
			{
				throw new InvalidDataException("Backend identity file is invalid or does not match expected values.");
			}

			var jsonPath = Path.Combine(backendFolder, CapabilitiesFileName);
			var json = JsonSerializer.Serialize(capabilities, new JsonSerializerOptions { WriteIndented = true });

			await File.WriteAllTextAsync(jsonPath, json);
		}
	}
}
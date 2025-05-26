using System;
using System.IO;

namespace Afrowave.SharedTools.Localization.Helpers
{
	/// <summary>
	/// Provides utility methods for working with file paths related to the core library's "Jsons" folder.
	/// </summary>
	/// <remarks>This class includes methods to retrieve the path to the "Jsons" folder and to resolve file paths
	/// within it. The "Jsons" folder is always located relative to the core library's assembly.</remarks>
	public static class PathHelper
	{
		/// <summary>
		/// Gets the full path to the Jsons folder, always relative to the core library.
		/// </summary>
		public static string GetJsonsFolder()
		{
			var libraryDir = Path.GetDirectoryName(typeof(PathHelper).Assembly.Location)
				 ?? AppContext.BaseDirectory;
			return Path.Combine(libraryDir, "Jsons");
		}

		/// <summary>
		/// Resolves a file inside /Jsons, with optional override.
		/// </summary>
		public static string ResolveJsonsFile(string fileName, string? overridePath = null)
		{
			if(!string.IsNullOrWhiteSpace(overridePath))
			{
				// Absolute or relative to current process
				return Path.GetFullPath(overridePath);
			}
			return Path.Combine(GetJsonsFolder(), fileName);
		}
	}
}
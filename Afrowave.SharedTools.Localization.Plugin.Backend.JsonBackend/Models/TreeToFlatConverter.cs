using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Models
{
	/// <summary>
	/// Utility class to flatten a structured translation tree into flat dictionary format.
	/// </summary>
	public static class TreeToFlatConverter
	{
		public static Dictionary<string, string> Convert(TranslationTree tree)
		{
			var result = new Dictionary<string, string>();
			Recurse(tree, "", result);
			return result;
		}

		private static void Recurse(TranslationTree node, string prefix, Dictionary<string, string> dict)
		{
			if(node.Value != null)
			{
				dict[prefix] = node.Value;
			}

			foreach(var kv in node.Children)
			{
				var newPrefix = string.IsNullOrEmpty(prefix) ? kv.Key : $"{prefix}.{kv.Key}";
				Recurse(kv.Value, newPrefix, dict);
			}
		}
	}
}
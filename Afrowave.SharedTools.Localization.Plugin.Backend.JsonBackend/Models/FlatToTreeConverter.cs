using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Models
{
	/// <summary>
	/// Utility class to convert flat translation keys into a structured translation tree.
	/// </summary>
	public static class FlatToTreeConverter
	{
		public static TranslationTree Convert(Dictionary<string, string> flatDict)
		{
			var root = new TranslationTree();

			foreach(var kv in flatDict)
			{
				var parts = kv.Key.Split('.');
				var current = root;

				foreach(var part in parts)
				{
					if(!current.Children.ContainsKey(part))
					{
						current.Children[part] = new TranslationTree();
					}

					current = current.Children[part];
				}

				current.Value = kv.Value;
			}

			return root;
		}
	}
}
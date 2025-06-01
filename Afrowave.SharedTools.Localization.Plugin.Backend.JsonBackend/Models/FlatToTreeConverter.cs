using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Models
{
	/// <summary>
	/// Utility class to convert flat translation keys into a structured translation tree.
	/// </summary>
	public static class FlatToTreeConverter
	{
		/// <summary>
		/// Converts a flat dictionary of key-value pairs into a hierarchical <see cref="TranslationTree"/> structure.
		/// </summary>
		/// <remarks>This method processes each key in the input dictionary by splitting it into segments using the
		/// period ('.')  character. Each segment corresponds to a node in the resulting tree, and the value associated with
		/// the key  is assigned to the leaf node.</remarks>
		/// <param name="flatDict">A dictionary where keys represent dot-separated paths and values represent the corresponding data. Each key
		/// defines a path in the tree, with segments separated by periods.</param>
		/// <returns>A <see cref="TranslationTree"/> object representing the hierarchical structure derived from the keys and values in
		/// the input dictionary.</returns>
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
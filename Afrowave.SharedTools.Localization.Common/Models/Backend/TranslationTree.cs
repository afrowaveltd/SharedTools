using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Models
{
	/// <summary>
	/// Represents a structured (tree-like) JSON translation format used internally by the JSON backend.
	/// </summary>
	public sealed class TranslationTree
	{
		/// <summary>
		/// Optional translation value for the current node.
		/// </summary>
		public string? Value { get; set; }

		/// <summary>
		/// Child nodes representing nested translation keys.
		/// </summary>
		public Dictionary<string, TranslationTree> Children { get; set; } = new Dictionary<string, TranslationTree>();
	}
}
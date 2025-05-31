using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Models
{
	/// <summary>
	/// Represents the result of parsing JSON – either flat or structured.
	/// </summary>
	public sealed class ReadJsonResult
	{
		public bool IsValid { get; private set; }

		public string ErrorMessage { get; private set; } = string.Empty;

		public Dictionary<string, string>? Flat { get; private set; }

		public TranslationTree? Tree { get; private set; }

		public static ReadJsonResult FromFlat(Dictionary<string, string> flat)
			 => new ReadJsonResult { IsValid = true, Flat = flat };

		public static ReadJsonResult FromStructured(TranslationTree tree)
			 => new ReadJsonResult { IsValid = true, Tree = tree };

		public static ReadJsonResult Invalid(string error)
			 => new ReadJsonResult { IsValid = false, ErrorMessage = error };
	}
}
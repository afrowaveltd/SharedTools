using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Models
{
	/// <summary>
	/// Represents the result of parsing JSON – either flat or structured.
	/// </summary>
	public sealed class ReadJsonResult
	{
		/// <summary>
		/// Gets a value indicating whether the current state is valid.
		/// </summary>
		public bool IsValid { get; private set; }

		/// <summary>
		/// Gets the error message associated with the current operation.
		/// </summary>
		public string ErrorMessage { get; private set; } = string.Empty;

		/// <summary>
		/// Gets a dictionary containing flattened key-value pairs.
		/// </summary>
		public Dictionary<string, string>? Flat { get; private set; }

		/// <summary>
		/// Gets the translation tree used for managing and organizing translations.
		/// </summary>
		public TranslationTree? Tree { get; private set; }

		/// <summary>
		/// Creates a new instance of <see cref="ReadJsonResult"/> using the provided flat dictionary representation.
		/// </summary>
		/// <param name="flat">A dictionary containing key-value pairs representing the flat JSON structure. Keys are expected to be strings, and
		/// values are their corresponding string representations.</param>
		/// <returns>A <see cref="ReadJsonResult"/> instance with <see cref="ReadJsonResult.IsValid"/> set to <see langword="true"/>
		/// and <see cref="ReadJsonResult.Flat"/> populated with the provided dictionary.</returns>
		public static ReadJsonResult FromFlat(Dictionary<string, string> flat)
			 => new ReadJsonResult { IsValid = true, Flat = flat };

		/// <summary>
		/// Creates a <see cref="ReadJsonResult"/> instance from the specified structured translation tree.
		/// </summary>
		/// <param name="tree">The structured translation tree to be used for creating the result. Cannot be null.</param>
		/// <returns>A <see cref="ReadJsonResult"/> object with <see cref="ReadJsonResult.IsValid"/> set to <see langword="true"/>  and
		/// <see cref="ReadJsonResult.Tree"/> initialized to the provided <paramref name="tree"/>.</returns>
		public static ReadJsonResult FromStructured(TranslationTree tree)
			 => new ReadJsonResult { IsValid = true, Tree = tree };

		/// <summary>
		/// Creates a result indicating that the JSON read operation is invalid.
		/// </summary>
		/// <param name="error">The error message describing the reason for the invalid result. Cannot be null or empty.</param>
		/// <returns>A <see cref="ReadJsonResult"/> instance with <see cref="ReadJsonResult.IsValid"/> set to <see langword="false"/>
		/// and <see cref="ReadJsonResult.ErrorMessage"/> set to the specified error message.</returns>
		public static ReadJsonResult Invalid(string error)
			 => new ReadJsonResult { IsValid = false, ErrorMessage = error };
	}
}
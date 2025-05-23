using Afrowave.SharedTools.Models.Enums;

namespace Afrowave.SharedTools.Text.DI
{
	/// <summary>
	/// Interface for DI-friendly text helper service.
	/// </summary>
	public interface ITextHelperService
	{
		/// <summary>
		/// Capitalizes the first character of the input string.
		/// </summary>
		/// <param name="input">The input string to capitalize.</param>
		/// <returns>The input string with the first character capitalized.</returns>
		string CapitalizeFirst(string input);

		/// <summary>
		/// Removes diacritical marks from the input string.
		/// </summary>
		/// <param name="input">The input string to process.</param>
		/// <returns>The input string without diacritics.</returns>
		string RemoveDiacritics(string input);

		/// <summary>
		/// Converts new line characters in the input string to the specified style.
		/// </summary>
		/// <param name="input">The input string to process.</param>
		/// <param name="style">The target new line style.</param>
		/// <returns>The input string with converted new lines.</returns>
		string ConvertNewLines(string input, TextNewLineStyle style);

		/// <summary>
		/// Truncates the input string to the specified maximum length.
		/// </summary>
		/// <param name="input">The input string to truncate.</param>
		/// <param name="maxLength">The maximum allowed length of the string.</param>
		/// <returns>The truncated string.</returns>
		string Truncate(string input, int maxLength);

		/// <summary>
		/// Converts the input string into a URL-friendly slug.
		/// </summary>
		/// <param name="input">The input string to slugify.</param>
		/// <returns>The slugified string.</returns>
		string Slugify(string input);
	}
}

using Afrowave.SharedTools.Models.Enums;
using System.Globalization;
using System.Text;

namespace Afrowave.SharedTools.Text.Static
{
	/// <summary>
	/// Provides static utility methods for common text operations.
	/// </summary>
	public static class TextHelper
	{
		/// <summary>
		/// Capitalizes the first character of the string.
		/// </summary>
		public static string CapitalizeFirst(string input)
		{
			if(string.IsNullOrWhiteSpace(input)) return input;
			return char.ToUpper(input[0]) + input[1..];
		}

		/// <summary>
		/// Removes diacritic marks from letters (e.g., č → c).
		/// </summary>
		public static string RemoveDiacritics(string input)
		{
			if(input == null) return string.Empty;
			var normalized = input.Normalize(NormalizationForm.FormD);
			var sb = new StringBuilder();

			foreach(var ch in normalized)
			{
				if(CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
					sb.Append(ch);
			}

			return sb.ToString().Normalize(NormalizationForm.FormC);
		}

		/// <summary>
		/// Converts newline characters to a standardized format.
		/// </summary>
		public static string ConvertNewLines(string input, TextNewLineStyle style)
		{
			if(input == null) return string.Empty;
			var normalized = input.Replace("\r\n", "\n").Replace("\r", "\n");
			string target = style switch
			{
				TextNewLineStyle.Windows => "\r\n",
				TextNewLineStyle.Mac => "\r",
				_ => "\n",
			};
			return normalized.Replace("\n", target);
		}

		/// <summary>
		/// Truncates the string to a maximum length and adds ellipsis if needed.
		/// </summary>
		public static string Truncate(string input, int maxLength)
		{
			if(string.IsNullOrEmpty(input) || maxLength <= 0) return string.Empty;
			return input.Length <= maxLength ? input : input[..maxLength] + "…";
		}

		/// <summary>
		/// Converts string to a URL-safe slug.
		/// </summary>
		public static string Slugify(string input)
		{
			if(input == null) return string.Empty;
			var cleaned = RemoveDiacritics(input).ToLowerInvariant();
			var sb = new StringBuilder();
			foreach(char c in cleaned)
			{
				if(char.IsLetterOrDigit(c)) sb.Append(c);
				else if(char.IsWhiteSpace(c)) sb.Append('-');
			}
			return sb.ToString().Trim('-');
		}

		/// <summary>
		/// Converts a slug back to a readable phrase.
		/// </summary>
		public static string Unslugify(string input)
		{
			if(input == null) return string.Empty;
			return input.Replace('-', ' ');
		}
	}

	/// <summary>
	/// Enum for newline conversion options.
	/// </summary>
}
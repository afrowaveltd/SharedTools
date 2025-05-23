using Afrowave.SharedTools.Models.Enums;
using System.Globalization;
using System.Text;

namespace Afrowave.SharedTools.Text.DI
{
	/// <summary>
	/// Implementation of ITextHelperService providing instance methods for common text operations.
	/// </summary>
	public class TextHelperService : ITextHelperService
	{
		public string CapitalizeFirst(string input)
		{
			if(string.IsNullOrWhiteSpace(input)) return input;
			return char.ToUpper(input[0]) + input[1..];
		}

		public string RemoveDiacritics(string input)
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

		public string ConvertNewLines(string input, TextNewLineStyle style)
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

		public string Truncate(string input, int maxLength)
		{
			if(string.IsNullOrEmpty(input) || maxLength <= 0) return string.Empty;
			return input.Length <= maxLength ? input : input[..maxLength] + "…";
		}

		public string Slugify(string input)
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
	}
}
using System.Text;
using System.Text.RegularExpressions;

namespace Afrowave.SharedTools.Text.Static.Conversions
{
	/// <summary>
	/// Converts plain text into a basic HTML representation.
	/// </summary>
	public static class PlainToHtmlConverter
	{
		/// <summary>
		/// Converts plain text into a basic HTML representation.
		/// </summary>
		/// <param name="input">The plain text input.</param>
		/// <param name="minify">Whether to minify the resulting HTML output.</param>
		public static string Convert(string input, bool minify = false)
		{
			if(string.IsNullOrEmpty(input)) return string.Empty;

			var sb = new StringBuilder();
			sb.Append("<p>");

			var lines = input.Split('\n');
			bool inUl = false;
			bool inOl = false;

			foreach(var rawLine in lines)
			{
				var line = rawLine.TrimEnd('\r');

				if(string.IsNullOrWhiteSpace(line))
				{
					sb.Append("</p><p>");
					continue;
				}

				// Replace starting spaces with &nbsp;
				var indent = new StringBuilder();
				foreach(var c in rawLine)
				{
					if(c == ' ') indent.Append("&nbsp;");
					else break;
				}

				// Handle UL
				if(Regex.IsMatch(line, "^[-•] "))
				{
					if(!inUl) { sb.Append("<ul>"); inUl = true; }
					sb.Append("<li>" + indent + EscapeHtml(line[2..]) + "</li>");
					continue;
				}
				else if(inUl) { sb.Append("</ul>"); inUl = false; }

				// Handle OL
				if(Regex.IsMatch(line, "^\\d+[\\.)] "))
				{
					if(!inOl) { sb.Append("<ol>"); inOl = true; }
					var content = Regex.Replace(line, "^\\d+[\\.)] ", "");
					sb.Append("<li>" + indent + EscapeHtml(content) + "</li>");
					continue;
				}
				else if(inOl) { sb.Append("</ol>"); inOl = false; }

				// Escape, then insert nbsp for prepositions, then insert links
				var formatted = EscapeHtml(line);
				formatted = ReplacePrepositions(formatted);
				formatted = ConvertLinksAndEmails(formatted);

				sb.Append(indent + formatted + "<br>");
			}

			if(inUl) sb.Append("</ul>");
			if(inOl) sb.Append("</ol>");

			sb.Append("</p>");
			var html = sb.ToString();
			return minify ? MinifyHtml(html) : html;
		}

		private static string EscapeHtml(string input)
		{
			return input.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
		}

		private static string ConvertLinksAndEmails(string input)
		{
			input = Regex.Replace(input,
				@"\b([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})\b",
				"<a href=\"mailto:$1\">$1</a>");

			input = Regex.Replace(input,
				@"\b(http[s]?://[^\s<]+)\b",
				"<a href=\"$1\">$1</a>");

			return input;
		}

		private static string ReplacePrepositions(string input)
		{
			return Regex.Replace(input, @"\b([aiksvzo])\s", "$1&nbsp;");
		}

		private static string MinifyHtml(string html)
		{
			return Regex.Replace(html, @">\s+<", "><").Replace("\n", string.Empty).Replace("\r", string.Empty);
		}
	}
}
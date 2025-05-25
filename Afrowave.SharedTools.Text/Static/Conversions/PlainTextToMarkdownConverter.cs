using System;
using System.Linq;
using System.Text;

namespace Afrowave.SharedTools.Text.Static.Conversions
{
	public static class PlainTextToMarkdownConverter
	{
		/// <summary>
		/// Converts tab-delimited text tables and other common plain text patterns to Markdown.
		/// </summary>
		public static string Convert(string input)
		{
			if(string.IsNullOrWhiteSpace(input))
				return string.Empty;

			var lines = input.Replace("\r\n", "\n").Split('\n');
			var sb = new StringBuilder();
			bool inTable = false;
			int tableColCount = 0;

			foreach(var line in lines)
			{
				// 1. Tab-delimited table detection
				if(line.Contains('\t'))
				{
					var cells = line.Split('\t', StringSplitOptions.None); // OPRAVA ZDE!
					if(!inTable)
					{
						// First table row: treat as header
						tableColCount = cells.Length;
						sb.Append("| " + string.Join(" | ", cells.Select(c => c.Trim())) + " |" + Environment.NewLine);
						sb.Append("| " + string.Join(" | ", Enumerable.Repeat("---", tableColCount)) + " |" + Environment.NewLine);
						inTable = true;
					}
					else
					{
						// Pad missing cells if line has fewer columns
						var padded = cells.Concat(Enumerable.Repeat("", tableColCount - cells.Length)).Take(tableColCount);
						sb.Append("| " + string.Join(" | ", padded.Select(c => c.Trim())) + " |" + Environment.NewLine);
					}
					continue;
				}
				else if(inTable && string.IsNullOrWhiteSpace(line))
				{
					// End table if empty line found
					inTable = false;
					tableColCount = 0;
				}

				// 2. Bullet lists
				if(line.TrimStart().StartsWith("- ") || line.TrimStart().StartsWith("* ") || line.TrimStart().StartsWith("• "))
				{
					sb.AppendLine("- " + line.TrimStart().Substring(2));
					continue;
				}

				// 3. Ordered lists
				if(System.Text.RegularExpressions.Regex.IsMatch(line, @"^\d+\.\s"))
				{
					sb.AppendLine(line.Trim());
					continue;
				}

				// 4. Headings (all-caps line as heading)
				if(!string.IsNullOrWhiteSpace(line) &&
					 line.ToUpperInvariant() == line.Trim() &&
					 line.Trim().Length > 3 &&
					 line.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
				{
					sb.AppendLine("# " + CapitalizeFirst(line.ToLowerInvariant()));
					continue;
				}

				// 5. Code block detection
				if(line.Trim() == "CODE:")
				{
					sb.AppendLine("```");
					continue;
				}
				if(line.Trim() == "ENDCODE")
				{
					sb.AppendLine("```");
					continue;
				}

				// 6. Quote
				if(line.TrimStart().StartsWith("> "))
				{
					sb.AppendLine(line.Trim());
					continue;
				}

				// Default: just append the line
				sb.AppendLine(line);
			}

			return sb.ToString().TrimEnd();
		}

		private static string CapitalizeFirst(string input)
		{
			if(string.IsNullOrEmpty(input)) return input;
			return char.ToUpper(input[0]) + input.Substring(1);
		}
	}
}
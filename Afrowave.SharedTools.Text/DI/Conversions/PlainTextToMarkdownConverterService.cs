using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Afrowave.SharedTools.Text.DI.Conversions
{
	public class PlainTextToMarkdownConverterService : IPlainTextToMarkdownConverterService
	{
		public string Convert(string text)
		{
			if(string.IsNullOrWhiteSpace(text))
				return string.Empty;

			var lines = text.Replace("\r\n", "\n").Split('\n');
			var sb = new StringBuilder();
			bool inTable = false;
			int tableColCount = 0;

			foreach(var rawLine in lines)
			{
				var line = rawLine.TrimEnd();

				// 1. Tab-delimited table detection
				if(line.Contains('\t'))
				{
					var cells = line.Split('\t', StringSplitOptions.None); // Zachová prázdné buňky!
					if(!inTable)
					{
						// První řádek tabulky (header)
						tableColCount = cells.Length;
						sb.Append("| " + string.Join(" | ", cells.Select(c => c.Trim())) + " |" + Environment.NewLine);
						sb.Append("| " + string.Join(" | ", Enumerable.Repeat("---", tableColCount)) + " |" + Environment.NewLine);
						inTable = true;
					}
					else
					{
						// Vždy doplň prázdné buňky do správného počtu sloupců
						var padded = cells.Concat(Enumerable.Repeat("", tableColCount - cells.Length)).Take(tableColCount);
						sb.Append("| " + string.Join(" | ", padded.Select(c => c.Trim())) + " |" + Environment.NewLine);
					}
					continue;
				}
				else if(inTable && string.IsNullOrWhiteSpace(line))
				{
					// Konec tabulky (prázdný řádek)
					inTable = false;
					tableColCount = 0;
				}

				// 2. Bullet lists
				if(Regex.IsMatch(line, @"^(\-|\*|•)\s+"))
				{
					sb.Append("- " + line.Substring(2).Trim());
					sb.AppendLine();
					continue;
				}
				// 3. Ordered lists
				if(Regex.IsMatch(line, @"^\d+\.\s+"))
				{
					sb.Append(line.Trim());
					sb.AppendLine();
					continue;
				}
				// 4. Quote
				if(line.StartsWith("> "))
				{
					sb.Append("> " + line.Substring(2).Trim());
					sb.AppendLine();
					continue;
				}
				// 5. Code block
				if(line.StartsWith("CODE:"))
				{
					sb.AppendLine("```");
					continue;
				}
				if(line.StartsWith("ENDCODE"))
				{
					sb.AppendLine("```");
					continue;
				}
				// 6. Headings (all caps)
				if(line.ToUpperInvariant() == line && line.Length > 0 && !Regex.IsMatch(line, @"[a-z]"))
				{
					sb.AppendLine("# " + CapitalizeFirst(line.ToLowerInvariant()));
					continue;
				}

				// 7. Default – obyčejný řádek
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
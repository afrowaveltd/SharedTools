using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Text.DI.Conversions
{
	/// <summary>
	/// Service: Converts Markdown to plain text (tab-delimited tables, etc.)
	/// </summary>
	public class MarkdownToPlainTextConverterService : IMarkdownToPlainTextConverterService
	{
		public string Convert(string markdown)
		{
			if(string.IsNullOrEmpty(markdown))
				return string.Empty;

			var output = markdown;

			// 1. Kódové bloky (```)
			output = Regex.Replace(output, @"```[\s\S]*?```", match =>
			{
				var content = match.Value.Replace("```", "").Trim();
				return "\nCODE:\n" + content + "\nENDCODE\n";
			});

			// 2. Inline kód
			output = Regex.Replace(output, @"`([^`]+)`", "$1");

			// 3. Nadpisy
			output = Regex.Replace(output, @"^#{1,6}\s*(.*)$", match =>
			{
				var text = match.Groups[1].Value.Trim();
				return text.ToUpperInvariant();
			}, RegexOptions.Multiline);

			// 4. Citace
			output = Regex.Replace(output, @"^>\s?(.*)$", match => "> " + match.Groups[1].Value, RegexOptions.Multiline);

			// 5. Tabulky – zarovnání pomocí tabelátorů
			output = Regex.Replace(output, @"((^\|.*\|\s*$\n?)+)", delegate (Match m)
			{
				var block = m.Value;
				var lines = block.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
				var tableRows = new List<string[]>();
				foreach(var line in lines)
				{
					if(Regex.IsMatch(line, @"^\|[-:\s|]+\|$"))
						continue; // skip separator line
					var cells = line.Trim('|').Split('|');
					for(int i = 0; i < cells.Length; i++)
						cells[i] = cells[i].Trim();
					tableRows.Add(cells);
				}

				var sb = new StringBuilder();
				foreach(var row in tableRows)
				{
					sb.AppendLine(string.Join("\t", row));
				}
				return sb.ToString();
			}, RegexOptions.Multiline);

			// 6. Odrážky, seznamy – zachovat (ponecháváme "- " a "1. ")
			// 7. Tučné/kurzíva/underline a další markdown tagy – odstranit
			output = Regex.Replace(output, @"(\*\*|\*|__|_|~~|`|~|\^)", "");

			// 8. Footnotes, odkazy, obrázky – odstranit tagy, ponechat jen popis/odkaz
			output = Regex.Replace(output, @"!\[(.*?)\]\((.*?)\)", "$1 ($2)");
			output = Regex.Replace(output, @"\[(.*?)\]\((.*?)\)", "$1 ($2)");
			output = Regex.Replace(output, @"\[\^.*?\]:.*", ""); // footnote definition
			output = Regex.Replace(output, @"\[\^.*?\]", "");     // footnote ref

			// 9. HTML tagy smazat
			output = Regex.Replace(output, @"<[^>]+>", "");

			// 10. Trimování nadbytečných mezer/prázdných řádků
			output = Regex.Replace(output, @"\n{3,}", "\n\n").Trim();

			return output;
		}

		/// <summary>
		/// Converts a Markdown string to plain text.
		/// </summary>
		/// <param name="input">Input text stream</param>
		/// <param name="output">Output text stream</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		public async Task ConvertStreamAsync(TextReader input, TextWriter output, CancellationToken cancellationToken = default)
		{
			var sb = new StringBuilder();
			char[] buffer = new char[4096];
			int read;
			while((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
			{
				sb.Append(buffer, 0, read);
				if(cancellationToken.IsCancellationRequested)
					break;
			}

			var converted = Convert(sb.ToString());
			await output.WriteAsync(converted);
		}
	}
}
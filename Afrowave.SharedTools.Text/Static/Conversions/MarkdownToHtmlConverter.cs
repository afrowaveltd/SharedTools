using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Text.Static.Conversions
{
	/// <summary>
	/// Converts Markdown to HTML, supporting edge-cases and modern GFM extensions.
	/// Allows injection of a class attribute for all tags.
	/// </summary>
	public static class MarkdownToHtmlConverter
	{
		public static string Convert(string markdown, string className = "", bool escapeMarkdown = false)
		{
			if(string.IsNullOrEmpty(markdown))
				return string.Empty;

			string classAttr = !string.IsNullOrWhiteSpace(className) ? " class=\"" + className + "\"" : "";

			// 1. Extract raw HTML blocks so they are not parsed further
			var htmlBlocks = new List<string>();
			markdown = Regex.Replace(markdown, @"(?<=^|\n)\s*(<([a-zA-Z][\w\d\-]*)\b[\s\S]*?>[\s\S]*?<\/\2>)",
				 delegate (Match m)
				 {
					 htmlBlocks.Add(m.Groups[1].Value);
					 return string.Format("[[[HTMLBLOCK-{0}]]]", htmlBlocks.Count - 1);
				 }, RegexOptions.Multiline);

			// 2. Extract code blocks (```...```)
			var codeBlocks = new List<string>();
			markdown = Regex.Replace(markdown, @"```(\w*)\n([\s\S]*?)```",
				 delegate (Match m)
				 {
					 var lang = m.Groups[1].Value;
					 var code = WebUtility.HtmlEncode(m.Groups[2].Value.Trim('\n', '\r'));
					 string repl = "<pre" + classAttr + "><code" + classAttr +
							  (string.IsNullOrWhiteSpace(lang) ? "" : " data-lang=\"" + lang + "\"") +
							  ">" + code + "</code></pre>";
					 codeBlocks.Add(repl);
					 return string.Format("[[[CODEBLOCK-{0}]]]", codeBlocks.Count - 1);
				 });

			// 3. Escape markdown if requested and return immediately
			if(escapeMarkdown)
			{
				markdown = Regex.Replace(markdown, @"([\\`\*_{}\[\]\(\)#\+\-\.!~^])", @"\$1");
				return "<p>" + markdown.Trim() + "</p>";
			}

			// 4. Footnote definitions (collect)
			var footnotes = new Dictionary<string, string>();
			markdown = Regex.Replace(markdown, @"^\[\^(\w+)\]:\s*(.*)$",
				 delegate (Match m)
				 {
					 footnotes[m.Groups[1].Value] = m.Groups[2].Value;
					 return "";
				 }, RegexOptions.Multiline);

			// 5. Footnote references (replace in text)
			markdown = Regex.Replace(markdown, @"\[\^(\w+)\]",
				 delegate (Match m)
				 {
					 var id = m.Groups[1].Value;
					 var footnoteId = "footnote-" + id;
					 return "<sup" + classAttr + "><a href=\"#" + footnoteId + "\" id=\"ref-" + footnoteId + "\">[" + id + "]</a></sup>";
				 });
			// Tables (GFM style, greedy match - chytne celou tabulku včetně všech řádků)
			markdown = Regex.Replace(markdown, @"(^\|.+\|\r?\n)+^\|[-:| ]+\|\r?\n((^\|.+\|\r?\n?)*)",
				delegate (Match m)
				{
					var fullMatch = m.Value;
					var lines = fullMatch.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

					if(lines.Length < 2)
						return fullMatch;

					// Header
					var headerLine = lines[0];
					var headerCells = Regex.Matches(headerLine, @"\|([^|]+)");
					var sb = new StringBuilder();
					sb.Append("<table" + classAttr + ">\n<tr" + classAttr + ">");
					foreach(Match cell in headerCells)
						sb.Append("<th" + classAttr + ">" + cell.Groups[1].Value.Trim() + "</th>");
					sb.Append("</tr>\n");

					// Data rows (skip header and separator)
					for(int i = 2; i < lines.Length; i++)
					{
						var row = lines[i];
						if(string.IsNullOrWhiteSpace(row)) continue;
						sb.Append("<tr" + classAttr + ">");
						foreach(Match cell in Regex.Matches(row, @"\|([^|]+)"))
							sb.Append("<td" + classAttr + ">" + cell.Groups[1].Value.Trim() + "</td>");
						sb.Append("</tr>\n");
					}
					sb.Append("</table>\n");
					return sb.ToString();
				}, RegexOptions.Multiline);

			// 6. Inline code (not inside code blocks)
			markdown = Regex.Replace(markdown, @"`([^`]+)`",
				 delegate (Match m)
				 {
					 var code = WebUtility.HtmlEncode(m.Groups[1].Value);
					 return "<code" + classAttr + ">" + code + "</code>";
				 });

			// 7. Task lists
			markdown = Regex.Replace(markdown, @"^(\s*[-\*\+]\s*)\[( |x|X)\]\s+(.*)$",
				 delegate (Match m)
				 {
					 var checkedAttr = m.Groups[2].Value.Trim().ToLower() == "x" ? " checked" : "";
					 return m.Groups[1].Value + "<input type=\"checkbox\"" + checkedAttr + " disabled" + classAttr + " /> " + m.Groups[3].Value;
				 }, RegexOptions.Multiline);

			// 8. Horizontal rule
			markdown = Regex.Replace(markdown, @"^\s*((?:---|\*\*\*|___)\s*)$", "<hr" + classAttr + " />", RegexOptions.Multiline);

			// 9. Headings
			markdown = Regex.Replace(markdown, @"^(#{1,6})\s*(.*?)$",
				 delegate (Match m)
				 {
					 int level = m.Groups[1].Value.Length;
					 return "<h" + level + classAttr + ">" + m.Groups[2].Value.Trim() + "</h" + level + ">";
				 }, RegexOptions.Multiline);

			// 10. Blockquotes (recursive)
			markdown = Regex.Replace(markdown, @"^((>\s?.*\n?)+)",
				 delegate (Match m)
				 {
					 var block = Regex.Replace(m.Groups[1].Value, @"^>\s?", "", RegexOptions.Multiline);
					 block = Convert(block, className, false); // recursive, bez escapování
					 return "<blockquote" + classAttr + ">" + block.Trim() + "</blockquote>";
				 }, RegexOptions.Multiline);

			// 11. Images
			markdown = Regex.Replace(markdown, @"!\[(.*?)\]\((.*?)\)",
				 delegate (Match m)
				 {
					 var alt = WebUtility.HtmlEncode(m.Groups[1].Value);
					 var src = WebUtility.HtmlEncode(m.Groups[2].Value);
					 return "<img alt=\"" + alt + "\" src=\"" + src + "\"" + classAttr + " />";
				 });

			// 12. Links
			markdown = Regex.Replace(markdown, @"\[(.*?)\]\((.*?)\)",
				 delegate (Match m)
				 {
					 var text = m.Groups[1].Value;
					 var href = WebUtility.HtmlEncode(m.Groups[2].Value);
					 return "<a href=\"" + href + "\"" + classAttr + ">" + text + "</a>";
				 });

			// 13. Bold (** or __)
			markdown = Regex.Replace(markdown, @"(\*\*|__)(.*?)\1",
				 delegate (Match m)
				 {
					 return "<strong" + classAttr + ">" + m.Groups[2].Value + "</strong>";
				 });

			// 14. Italic (* or _)
			markdown = Regex.Replace(markdown, @"(\*|_)(.*?)\1",
				 delegate (Match m)
				 {
					 return "<em" + classAttr + ">" + m.Groups[2].Value + "</em>";
				 });

			// 15. Superscript ^text^
			markdown = Regex.Replace(markdown, @"\^([^^\s][^ \^]*)\^",
				 delegate (Match m)
				 {
					 return "<sup" + classAttr + ">" + m.Groups[1].Value + "</sup>";
				 });

			// 16. Subscript ~text~
			markdown = Regex.Replace(markdown, @"~([^~\s][^ ~]*)~",
				 delegate (Match m)
				 {
					 return "<sub" + classAttr + ">" + m.Groups[1].Value + "</sub>";
				 });

			// Najdi všechny bloky za sebou jdoucích řádků začínajících "1. ", "2. " apod. (oddělené max jedním \n)
			markdown = Regex.Replace(markdown, @"(^|\n)(?:(?:\d+\.\s.*\n?)+)",
				 delegate (Match m)
				 {
					 // Vezmi všechny řádky, které začínají číslem tečky a mezerou
					 string listBlock = m.Value;
					 var lines = listBlock.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
					 var sb = new StringBuilder();
					 bool found = false;
					 foreach(var line in lines)
					 {
						 var itemMatch = Regex.Match(line, @"^\d+\.\s+(.*)");
						 if(itemMatch.Success)
						 {
							 if(!found)
							 {
								 sb.Append("<ol" + classAttr + ">\n");
								 found = true;
							 }
							 sb.Append("<li" + classAttr + ">" + itemMatch.Groups[1].Value + "</li>\n");
						 }
						 else
						 {
							 if(found)
							 {
								 sb.Append("</ol>\n");
								 found = false;
							 }
							 sb.Append(line + "\n");
						 }
					 }
					 if(found)
					 {
						 sb.Append("</ol>\n");
					 }
					 return sb.ToString();
				 }, RegexOptions.Multiline);

			// 19. Unordered lists
			// Aggregate all consecutive lines starting with -, * or + into one <ul>
			markdown = Regex.Replace(markdown, @"(^|\n)(?:(?:[\*\-\+]\s.*\n?)+)",
				 delegate (Match m)
				 {
					 string listBlock = m.Value;
					 var lines = listBlock.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
					 var sb = new StringBuilder();
					 bool found = false;
					 foreach(var line in lines)
					 {
						 var itemMatch = Regex.Match(line, @"^[\*\-\+]\s+(.*)");
						 if(itemMatch.Success)
						 {
							 if(!found)
							 {
								 sb.Append("<ul" + classAttr + ">\n");
								 found = true;
							 }
							 sb.Append("<li" + classAttr + ">" + itemMatch.Groups[1].Value + "</li>\n");
						 }
						 else
						 {
							 if(found)
							 {
								 sb.Append("</ul>\n");
								 found = false;
							 }
							 sb.Append(line + "\n");
						 }
					 }
					 if(found)
					 {
						 sb.Append("</ul>\n");
					 }
					 return sb.ToString();
				 }, RegexOptions.Multiline);

			// 20. Append footnotes at the end (if any)
			if(footnotes.Count > 0)
			{
				var sb = new StringBuilder();
				sb.Append("<section" + classAttr + " class=\"footnotes\">\n<hr" + classAttr + " />\n<ol" + classAttr + ">\n");
				foreach(var kvp in footnotes)
				{
					var footnoteId = "footnote-" + kvp.Key;
					sb.Append("<li id=\"" + footnoteId + "\"" + classAttr + ">" + kvp.Value + " <a href=\"#ref-" + footnoteId + "\">&#8617;</a></li>\n");
				}
				sb.Append("</ol>\n</section>\n");
				markdown += "\n" + sb.ToString();
			}

			// 21. Restore code blocks
			for(int i = 0; i < codeBlocks.Count; i++)
				markdown = markdown.Replace("[[[CODEBLOCK-" + i + "]]]", codeBlocks[i]);

			// 22. Restore raw HTML blocks
			for(int i = 0; i < htmlBlocks.Count; i++)
				markdown = markdown.Replace("[[[HTMLBLOCK-" + i + "]]]", htmlBlocks[i]);

			// 23. Paragraphs for "orphan" lines not inside block tags
			markdown = Regex.Replace(markdown,
				 @"^(?!<.*>)([^\n]+)$",
				 delegate (Match m)
				 {
					 return "<p" + classAttr + ">" + m.Groups[1].Value.Trim() + "</p>";
				 }, RegexOptions.Multiline);

			// Remove multiple blank lines
			markdown = Regex.Replace(markdown, @"(\n\s*){3,}", "\n\n");

			return markdown.Trim();
		}

		/// <summary>
		/// Asynchronously converts Markdown from a TextReader to HTML written into a TextWriter.
		/// </summary>
		public static async Task ConvertStreamAsync(TextReader input, TextWriter output, string className = "", bool escapeMarkdown = false, CancellationToken cancellationToken = default)
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

			var converted = Convert(sb.ToString(), className, escapeMarkdown);
			await output.WriteAsync(converted);
		}
	}
}
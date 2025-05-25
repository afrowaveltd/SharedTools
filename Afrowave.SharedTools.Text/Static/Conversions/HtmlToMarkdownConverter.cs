using Afrowave.SharedTools.Text.Models.Markdown;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Text.Static.Conversions
{
	/// <summary>
	/// Converts HTML to Markdown using dynamic tag mapping.
	/// </summary>
	public static class HtmlToMarkdownConverter
	{
		public static string Convert(string html, List<MarkdownTagMapping> mappings)
		{
			if(string.IsNullOrEmpty(html)) return string.Empty;
			var output = html;

			foreach(var map in mappings.Where(m => !m.IsSpecial))
			{
				if(map.RequiresClosingTag)
				{
					output = Regex.Replace(output,
						 $@"<\s*{map.HtmlTag}\b[^>]*>(.*?)<\s*/\s*{map.HtmlTag}\s*>",
						 m =>
						 {
							 var content = m.Groups[1].Value;
							 var prefix = map.IsPrefix ? map.Markdown : "";
							 var suffix = map.IsSuffix ? map.Markdown : "";
							 return $"{prefix}{content}{suffix}";
						 },
						 RegexOptions.IgnoreCase | RegexOptions.Singleline);
				}
				else
				{
					output = Regex.Replace(output,
						 $@"<\s*{map.HtmlTag}\b[^>]*/?>",
						 map.Markdown,
						 RegexOptions.IgnoreCase);
				}
			}

			// Blockquote: > text
			output = Regex.Replace(output,
				 @"<blockquote>(.*?)<\/blockquote>",
				 m => $"> {m.Groups[1].Value.Trim().Replace("\n", "\n> ")}",
				 RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Horizontal rule: --- (or ***)
			output = Regex.Replace(output,
				 @"<hr\s*\/?>",
				 "---",
				 RegexOptions.IgnoreCase);

			// Pre/code blocks: ```code```
			output = Regex.Replace(output,
				 @"<pre>(.*?)<\/pre>",
				 m =>
				 {
					 var content = m.Groups[1].Value.Trim('\n', '\r');
					 // Remove any <code> inside <pre>
					 content = Regex.Replace(content, @"^<code>(.*?)<\/code>$", "$1", RegexOptions.IgnoreCase | RegexOptions.Singleline);
					 return $"```\n{content}\n```";
				 },
				 RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Inline code: <code> (not inside <pre>)
			output = Regex.Replace(output,
				 @"<code>(.*?)<\/code>",
				 m => $"`{m.Groups[1].Value}`",
				 RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Superscript: ^text^ (GitHub style)
			output = Regex.Replace(output,
				 @"<sup>(.*?)<\/sup>",
				 m => $"^{m.Groups[1].Value}^",
				 RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Subscript: ~text~ (GitHub style)
			output = Regex.Replace(output,
				 @"<sub>(.*?)<\/sub>",
				 m => $"~{m.Groups[1].Value}~",
				 RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Custom <bold> tag support
			output = Regex.Replace(output,
				 @"<bold>(.*?)<\/bold>",
				 "**$1**",
				 RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Unordered list
			output = Regex.Replace(output,
				 @"<ul>(.*?)<\/ul>",
				 m => Regex.Replace(m.Groups[1].Value, @"<li>(.*?)<\/li>", "- $1", RegexOptions.Singleline),
				 RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Ordered list (with dynamic numbering)
			output = Regex.Replace(output,
				 @"<ol>(.*?)<\/ol>",
				 m =>
				 {
					 int index = 1;
					 return Regex.Replace(m.Groups[1].Value, @"<li>(.*?)<\/li>", _ => $"{index++}. {_.Groups[1].Value}", RegexOptions.Singleline);
				 },
				 RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Remove leftover <ol>, <ul>
			output = Regex.Replace(output, @"<\/?ol>|<\/?ul>", "", RegexOptions.IgnoreCase);

			// Tables (very basic)
			output = Regex.Replace(output,
				 @"<tr>(.*?)<\/tr>",
				 m => $"|{m.Groups[1].Value}|\n",
				 RegexOptions.IgnoreCase | RegexOptions.Singleline);
			output = Regex.Replace(output,
				 @"<t[hd]>(.*?)<\/t[hd]>",
				 " $1 ",
				 RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Links
			output = Regex.Replace(output,
				 @"<a\s+href=""(.*?)""[^>]*>(.*?)<\/a>",
				 "[$2]($1)",
				 RegexOptions.IgnoreCase);

			// Images
			output = Regex.Replace(output,
				 @"<img\s+[^>]*alt=""(.*?)""[^>]*src=""(.*?)""[^>]*/?>",
				 "![$1]($2)",
				 RegexOptions.IgnoreCase);

			// Strip <div>, <span>
			output = Regex.Replace(output, @"<\/?(div|span)[^>]*>", "", RegexOptions.IgnoreCase);

			// Remove all other HTML tags
			output = Regex.Replace(output, "<[^>]+>", "");

			return output.Trim();
		}

		public static async Task ConvertStreamAsync(TextReader input, TextWriter output, List<MarkdownTagMapping> mappings, CancellationToken cancellationToken = default)
		{
			var buffer = new char[4096];
			var sb = new StringBuilder();
			int read;
			while((read = await input.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
			{
				sb.Append(buffer, 0, read);
			}

			var converted = Convert(sb.ToString(), mappings);
			await output.WriteAsync(converted);
		}
	}
}
using System;
using System.Text;

namespace Afrowave.SharedTools.Text.Static.Conversions
{
	/// <summary>
	/// Converts HTML content into plain text, preserving logical structure.
	/// </summary>
	public static class HtmlToPlainTextConverter
	{
		/// <summary>
		/// Converts an HTML string into readable plain text.
		/// </summary>
		/// <param name="html">The HTML input.</param>
		/// <returns>Plain text string.</returns>
		public static string Convert(string html)
		{
			if(string.IsNullOrEmpty(html)) return string.Empty;

			var output = new StringBuilder();
			var tagBuffer = new StringBuilder();
			bool inTag = false;
			bool inBody = false;
			bool inIgnoreBlock = false;
			bool inAnchor = false;
			string anchorHref = string.Empty;
			string ignoreTag = string.Empty;
			int indentLevel = 0;
			bool insideOrderedList = false;
			int orderedIndex = 1;

			for(int i = 0; i < html.Length; i++)
			{
				char c = html[i];

				if(inTag)
				{
					if(c == '>')
					{
						inTag = false;
						string tag = tagBuffer.ToString().Trim().ToLower();
						tagBuffer.Clear();

						if(!inBody)
						{
							if(tag.StartsWith("body")) inBody = true;
							continue;
						}

						if(tag.StartsWith("script") || tag.StartsWith("style") || tag.StartsWith("head"))
						{
							inIgnoreBlock = true;
							ignoreTag = tag.Split(' ')[0];
							continue;
						}

						if(tag.StartsWith("a") && tag.Contains("href="))
						{
							inAnchor = true;
							int hrefIndex = tag.IndexOf("href=", StringComparison.OrdinalIgnoreCase);
							int startQuote = tag.IndexOf('"', hrefIndex);
							int endQuote = tag.IndexOf('"', startQuote + 1);
							if(startQuote > 0 && endQuote > startQuote)
							{
								anchorHref = tag.Substring(startQuote + 1, endQuote - startQuote - 1);
							}
							continue;
						}
						if(tag.StartsWith("li"))
						{
							output.Append('\n');
							output.Append(new string(' ', indentLevel * 2));
							if(insideOrderedList)
							{
								output.Append($"{orderedIndex++}) ");
							}
							else
							{
								output.Append("- ");
							}
							continue;
						}
						if(tag.StartsWith("ol"))
						{
							indentLevel++;
							insideOrderedList = true;
							orderedIndex = 1;
							continue;
						}
						if(tag.StartsWith("ul"))
						{
							indentLevel++;
							continue;
						}
						if(tag.StartsWith("/ol"))
						{
							indentLevel = Math.Max(0, indentLevel - 1);
							insideOrderedList = false;
							output.Append("\n");
							continue;
						}
						if(tag.StartsWith("/ul"))
						{
							indentLevel = Math.Max(0, indentLevel - 1);
							output.Append("\n");
							continue;
						}
						if(tag == "br" || tag == "br/")
						{
							output.Append('\n');
						}
						else if(tag == "/p")
						{
							output.Append("\n\n");
						}
						else if(tag.StartsWith("&nbsp;"))
						{
							output.Append(' ');
						}
						else if(tag.StartsWith("/a") && inAnchor)
						{
							output.Append($": {anchorHref}");
							inAnchor = false;
							anchorHref = string.Empty;
						}
					}
					else
					{
						tagBuffer.Append(c);
					}
					continue;
				}

				if(inIgnoreBlock)
				{
					if(c == '<' && html.Substring(i).StartsWith($"</{ignoreTag}"))
					{
						inIgnoreBlock = false;
					}
					continue;
				}

				if(c == '<')
				{
					inTag = true;
					continue;
				}

				if(inBody)
				{
					output.Append(c);
				}
			}

			return output.ToString().Trim();
		}
	}
}
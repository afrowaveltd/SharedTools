using Afrowave.SharedTools.Text.Static.Conversions;
using System.Text.RegularExpressions;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Text.Services.Static.Conversions
{
	public class MarkdownToHtmlConverterTests
	{
		[Theory]
		[InlineData("```csharp\nvar a = 5;\n```", "<pre><code data-lang=\"csharp\">var a = 5;</code></pre>")]
		[InlineData("```\nplain code\n```", "<pre><code>plain code</code></pre>")]
		public void Converts_CodeBlocks(string md, string expected)
		{
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Equal(expected, html);
		}

		[Theory]
		[InlineData("Text with `inline code` in it.", "<p>Text with <code>inline code</code> in it.</p>")]
		public void Converts_InlineCode(string md, string expected)
		{
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Equal(expected, html);
		}

		[Theory]
		[InlineData("> blockquote", "<blockquote><p>blockquote</p></blockquote>")]
		[InlineData("> level 1\n> > level 2", "<blockquote><p>level 1</p><blockquote><p>level 2</p></blockquote></blockquote>")]
		public void Converts_Blockquotes(string md, string expected)
		{
			var html = MarkdownToHtmlConverter.Convert(md);
			// Remove newlines for comparison robustness
			Assert.Equal(expected.Replace("\n", ""), html.Replace("\n", ""));
		}

		[Theory]
		[InlineData("---", "<hr />")]
		[InlineData("___", "<hr />")]
		public void Converts_HorizontalRule(string md, string expected)
		{
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Equal(expected, html);
		}

		[Theory]
		[InlineData("# Heading 1", "<h1>Heading 1</h1>")]
		[InlineData("## Heading 2", "<h2>Heading 2</h2>")]
		public void Converts_Headings(string md, string expected)
		{
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Equal(expected, html);
		}

		[Theory]
		[InlineData("- [ ] Task A", "<ul>\n<li><input type=\"checkbox\" disabled /> Task A</li>\n</ul>")]
		[InlineData("- [x] Done Task", "<ul>\n<li><input type=\"checkbox\" checked disabled /> Done Task</li>\n</ul>")]
		public void Converts_TaskList(string md, string expected)
		{
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Equal(expected.Replace("\r", ""), html.Replace("\r", ""));
		}

		[Theory]
		[InlineData("**bold** and *italic*", "<strong>bold</strong> and <em>italic</em>")]
		[InlineData("__bold__ and _italic_", "<strong>bold</strong> and <em>italic</em>")]
		public void Converts_BoldAndItalic(string md, string expected)
		{
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Equal(expected, html);
		}

		[Theory]
		[InlineData("^sup^ and ~sub~", "<sup>sup</sup> and <sub>sub</sub>")]
		public void Converts_SupAndSub(string md, string expected)
		{
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Equal(expected, html);
		}

		[Theory]
		[InlineData("[text](url)", "<a href=\"url\">text</a>")]
		[InlineData("![alt](img.jpg)", "<img alt=\"alt\" src=\"img.jpg\" />")]
		public void Converts_LinksAndImages(string md, string expected)
		{
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Equal(expected, html);
		}

		[Fact]
		public void Converts_OrderedList()
		{
			var md = "1. First\n2. Second\n3. Third";
			var expected = "<ol>\n<li>First</li>\n<li>Second</li>\n<li>Third</li>\n</ol>";
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Equal(expected.Replace("\r", ""), html.Replace("\r", ""));
		}

		[Fact]
		public void Converts_UnorderedList()
		{
			var md = "- Apple\n- Banana\n- Cherry";
			var expected = "<ul>\n<li>Apple</li>\n<li>Banana</li>\n<li>Cherry</li>\n</ul>";
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Equal(expected.Replace("\r", ""), html.Replace("\r", ""));
		}

		[Fact]
		public void Converts_Table()
		{
			var md = "|Name|Age|\n|---|---|\n|Tom|10|\n|Anna|9|\n";
			var expected =
@"<table>
<tr><th>Name</th><th>Age</th></tr>
<tr><td>Tom</td><td>10</td></tr>
<tr><td>Anna</td><td>9</td></tr>
</table>
";
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Equal(expected.Replace("\r", "").Replace("\n", ""), html.Replace("\r", "").Replace("\n", ""));
		}

		[Fact]
		public void Converts_EscapedMarkdown()
		{
			var md = @"\*\*not bold\*\* \_not italic\_";
			var html = MarkdownToHtmlConverter.Convert(md, "", true);
			Console.WriteLine("HTML výstup: " + html);
			foreach(var c in html) Console.Write((int)c + " "); // Zobrazí ASCII kódy
			Assert.True(html.Contains(@"\*")); // True pokud je alespoň jeden backslash a hvězdička
		}

		[Fact]
		public void Ignores_RawHtmlBlock()
		{
			var md = "<div>Custom HTML</div>\n\nParagraph text";
			var expected = "<div>Custom HTML</div>\n<p>Paragraph text</p>";
			var html = MarkdownToHtmlConverter.Convert(md);

			// Normalize newlines (any sequence of \n or \r to a single \n)
			string normalize(string s) => Regex.Replace(s, @"[\r\n]+", "\n").Trim();
			Assert.Equal(normalize(expected), normalize(html));
		}

		[Fact]
		public void Converts_Footnotes()
		{
			var md = "Some text with a footnote.[^1]\n\n[^1]: Footnote text here";
			var html = MarkdownToHtmlConverter.Convert(md);
			Assert.Contains("footnote-1", html); // Footnote anchor generated
			Assert.Contains("Footnote text here", html); // Footnote body present
		}

		[Theory]
		[InlineData("**bold** and *italic*", "<strong>bold</strong> and <em>italic</em>")]
		[InlineData("# Heading", "<h1>Heading</h1>")]
		[InlineData("```csharp\nvar a = 5;\n```", "<pre><code data-lang=\"csharp\">var a = 5;</code></pre>")]
		[InlineData("> quote", "<blockquote><p>quote</p></blockquote>")]
		[InlineData("- Item 1\n- Item 2", "<ul>\n<li>Item 1</li>\n<li>Item 2</li>\n</ul>")]
		public async Task Converts_Stream_Variants(string md, string expectedHtml)
		{
			using(var reader = new StringReader(md))
			using(var writer = new StringWriter())
			{
				await MarkdownToHtmlConverter.ConvertStreamAsync(reader, writer);
				var output = writer.ToString();
				// Odstraň \r kvůli Windows/Linux rozdílu v testu
				Assert.Contains(expectedHtml.Replace("\r", ""), output.Replace("\r", ""));
			}
		}

		[Fact]
		public async Task Converts_Stream_EscapedMarkdown()
		{
			string md = "**not bold** _not italic_";
			string expected = "<p>\\*\\*not bold\\*\\* \\_not italic\\_</p>";
			using(var reader = new StringReader(md))
			using(var writer = new StringWriter())
			{
				await MarkdownToHtmlConverter.ConvertStreamAsync(reader, writer, "", true);
				var output = writer.ToString();
				Assert.Equal(expected, output);
			}
		}

		[Fact]
		public async Task Converts_Stream_RespectsClass()
		{
			string md = "**bold**";
			string expected = "<strong class=\"test-class\">bold</strong>";
			using(var reader = new StringReader(md))
			using(var writer = new StringWriter())
			{
				await MarkdownToHtmlConverter.ConvertStreamAsync(reader, writer, "test-class");
				var output = writer.ToString();
				Assert.Contains(expected, output);
			}
		}

		[Fact]
		public async Task Converts_Stream_Footnotes()
		{
			string md = "Some text[^1]\n\n[^1]: This is a footnote";
			using(var reader = new StringReader(md))
			using(var writer = new StringWriter())
			{
				await MarkdownToHtmlConverter.ConvertStreamAsync(reader, writer);
				var output = writer.ToString();
				Assert.Contains("footnote-1", output);
				Assert.Contains("This is a footnote", output);
			}
		}

		[Fact]
		public async Task Converts_Stream_Table()
		{
			string md = "|Name|Age|\n|---|---|\n|Tom|10|\n|Anna|9|";
			string expected = "<table><tr><th>Name</th><th>Age</th></tr><tr><td>Tom</td><td>10</td></tr><tr><td>Anna</td><td>9</td></tr></table>";
			using(var reader = new StringReader(md))
			using(var writer = new StringWriter())
			{
				await MarkdownToHtmlConverter.ConvertStreamAsync(reader, writer);
				var output = writer.ToString();
				// Normalizace výstupu pro porovnání
				string norm(string s) => s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
				Assert.Contains(norm("<table><tr><th>Name</th><th>Age</th></tr>"), norm(output));
				Assert.Contains(norm("<tr><td>Tom</td><td>10</td></tr>"), norm(output));
				Assert.Contains(norm("<tr><td>Anna</td><td>9</td></tr>"), norm(output));
			}
		}
	}
}
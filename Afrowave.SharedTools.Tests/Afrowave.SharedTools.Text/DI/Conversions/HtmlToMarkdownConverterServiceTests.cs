using Afrowave.SharedTools.Text.DI.Conversions;
using Afrowave.SharedTools.Text.Models.Markdown;

namespace Afrowave.SharedTools.Text.Tests.DI.Conversions
{
	public class HtmlToMarkdownConverterServiceTests
	{
		private readonly HtmlToMarkdownConverterService _service = new();
		private readonly List<MarkdownTagMapping> _emptyMappings = new();

		[Theory]
		[InlineData("<blockquote>This is a quote</blockquote>", "> This is a quote")]
		[InlineData("<blockquote>Line1\nLine2</blockquote>", "> Line1\n> Line2")]
		public void Converts_Blockquote(string html, string expected)
		{
			var md = _service.Convert(html, _emptyMappings);
			Assert.Equal(expected, md);
		}

		[Theory]
		[InlineData("<hr>", "---")]
		[InlineData("<hr/>", "---")]
		public void Converts_HorizontalRule(string html, string expected)
		{
			var md = _service.Convert(html, _emptyMappings);
			Assert.Equal(expected, md);
		}

		[Theory]
		[InlineData("<pre>code here</pre>", "```\ncode here\n```")]
		[InlineData("<pre><code>abc</code></pre>", "```\nabc\n```")]
		public void Converts_PreBlock(string html, string expected)
		{
			var md = _service.Convert(html, _emptyMappings);
			Assert.Equal(expected, md);
		}

		[Theory]
		[InlineData("<code>abc</code>", "`abc`")]
		[InlineData("Text <code>x</code> more", "Text `x` more")]
		public void Converts_InlineCode(string html, string expected)
		{
			var md = _service.Convert(html, _emptyMappings);
			Assert.Equal(expected, md);
		}

		[Theory]
		[InlineData("<sup>2</sup>", "^2^")]
		[InlineData("E = mc<sup>2</sup>", "E = mc^2^")]
		public void Converts_Superscript(string html, string expected)
		{
			var md = _service.Convert(html, _emptyMappings);
			Assert.Equal(expected, md);
		}

		[Theory]
		[InlineData("<sub>2</sub>", "~2~")]
		[InlineData("H<sub>2</sub>O", "H~2~O")]
		public void Converts_Subscript(string html, string expected)
		{
			var md = _service.Convert(html, _emptyMappings);
			Assert.Equal(expected, md);
		}

		[Fact]
		public void Converts_OrderedList_WithDynamicNumbers()
		{
			var html = "<ol><li>One</li><li>Two</li><li>Three</li></ol>";
			var expected = "1. One2. Two3. Three";
			var md = _service.Convert(html, _emptyMappings);
			Assert.Equal(expected, md.Replace("\n", "").Replace("\r", ""));
		}
	}
}
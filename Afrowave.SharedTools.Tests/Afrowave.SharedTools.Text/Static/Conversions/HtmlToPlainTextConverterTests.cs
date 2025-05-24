using Afrowave.SharedTools.Text.Static.Conversions;
using System.Diagnostics;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Text.Static.Conversions
{
	public class HtmlToPlainTextConverterTests
	{
		[Fact]
		public void Convert_BasicParagraphAndBr_ShouldTransformCorrectly()
		{
			string html = "<html><body><p>Hello<br>World</p></body></html>";
			string expected = "Hello\nWorld";
			string result = HtmlToPlainTextConverter.Convert(html);
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Convert_AnchorTag_ShouldExtractLink()
		{
			string html = "<body><a href=\"https://example.com\">Click here</a></body>";
			string expected = "Click here: https://example.com";
			string result = HtmlToPlainTextConverter.Convert(html);
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Convert_ListStructure_ShouldFormatWithIndentation()
		{
			string html = "<body><ul><li>Item 1<ul><li>Subitem</li></ul></li><li>Item 2</li></ul></body>";
			string result = HtmlToPlainTextConverter.Convert(html);
			Assert.Contains("- Item 1", result);
			Assert.Contains("  - Subitem", result);
			Assert.Contains("- Item 2", result);
		}

		[Fact]
		public void Convert_OrderedList_ShouldAddNumbers()
		{
			string html = "<body><ol><li>First</li><li>Second</li></ol></body>";
			string result = HtmlToPlainTextConverter.Convert(html);
			Assert.Contains("1) First", result);
			Assert.Contains("2) Second", result);
		}

		[Fact]
		public void Convert_Performance_ShouldBeFast()
		{
			string html = "<body>" + string.Concat(Enumerable.Repeat("<p>Line<br></p>", 1000)) + "</body>";
			var sw = Stopwatch.StartNew();
			HtmlToPlainTextConverter.Convert(html);
			sw.Stop();
			Assert.True(sw.ElapsedMilliseconds < 50, $"Execution took too long: {sw.ElapsedMilliseconds} ms");
		}
	}
}
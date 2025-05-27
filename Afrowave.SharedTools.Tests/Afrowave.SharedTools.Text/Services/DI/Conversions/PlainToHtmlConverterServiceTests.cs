using Afrowave.SharedTools.Text.DI.Conversions;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Text.Services.DI.Conversions
{
	public class PlainToHtmlConverterServiceTests
	{
		private readonly IPlainToHtmlConverterService _service = new PlainToHtmlConverterService();

		[Fact]
		public void Convert_SimpleParagraph_ShouldWrapInParagraphTags()
		{
			var input = "Hello world!";
			var html = _service.Convert(input);
			Assert.Contains("<p>", html);
			Assert.Contains("Hello world!", html);
		}

		[Fact]
		public void Convert_EmptyInput_ShouldReturnEmptyString()
		{
			var html = _service.Convert("");
			Assert.Equal(string.Empty, html);
		}

		[Fact]
		public void Convert_ListDetection_ShouldWrapWithUlAndLi()
		{
			var input = "- item 1\n- item 2";
			var html = _service.Convert(input);
			Assert.Contains("<ul>", html);
			Assert.Contains("<li>item 1</li>", html);
			Assert.Contains("<li>item 2</li>", html);
		}

		[Fact]
		public void Convert_NumberedList_ShouldWrapWithOlAndLi()
		{
			var input = "1. First\n2. Second";
			var html = _service.Convert(input);
			Assert.Contains("<ol>", html);
			Assert.Contains("<li>First</li>", html);
			Assert.Contains("<li>Second</li>", html);
		}

		[Fact]
		public void Convert_EmailsAndUrls_ShouldCreateHyperlinks()
		{
			var input = "Contact: user@example.com and https://example.com";
			var html = _service.Convert(input);
			Assert.Contains("<a href=\"mailto:user@example.com\">user@example.com</a>", html);
			Assert.Contains("<a href=\"https://example.com\">https://example.com</a>", html);
		}

		[Fact]
		public void Convert_PreloadPrepositions_ShouldInsertNbsp()
		{
			var input = "a svět";
			var html = _service.Convert(input);
			Assert.Contains("a&nbsp;svět", html);
		}

		[Fact]
		public void Convert_Minify_ShouldRemoveLineBreaks()
		{
			var input = "Line1\nLine2";
			var html = _service.Convert(input, minify: true);
			Assert.DoesNotContain("\n", html);
			Assert.DoesNotContain("\r", html);
		}
	}
}
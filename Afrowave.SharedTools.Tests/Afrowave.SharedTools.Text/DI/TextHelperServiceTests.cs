using Afrowave.SharedTools.Models.Enums;
using Afrowave.SharedTools.Text.DI;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Text.DI
{
	public class TextHelperServiceTests
	{
		private readonly ITextHelperService _helper = new TextHelperService();

		[Theory]
		[InlineData("česká řeřicha", "ceska rericha")]
		[InlineData("Žluťoučký kůň", "Zlutoucky kun")]
		[InlineData(null, "")]
		public void RemoveDiacritics_ShouldRemoveCorrectly(string input, string expected)
		{
			var result = _helper.RemoveDiacritics(input);
			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData("test", "Test")]
		[InlineData("Test", "Test")]
		[InlineData("", "")]
		[InlineData(null, null)]
		public void CapitalizeFirst_ShouldCapitalizeFirstLetter(string input, string expected)
		{
			var result = _helper.CapitalizeFirst(input);
			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData("Line1\nLine2", TextNewLineStyle.Windows, "Line1\r\nLine2")]
		[InlineData("Line1\r\nLine2", TextNewLineStyle.Unix, "Line1\nLine2")]
		[InlineData("Line1\rLine2", TextNewLineStyle.Unix, "Line1\nLine2")]
		[InlineData(null, TextNewLineStyle.Unix, "")]
		public void ConvertNewLines_ShouldConvertCorrectly(string input, TextNewLineStyle style, string expected)
		{
			var result = _helper.ConvertNewLines(input, style);
			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData("Hello world", 5, "Hello…")]
		[InlineData("Hello", 10, "Hello")]
		[InlineData("", 5, "")]
		[InlineData(null, 5, "")]
		public void Truncate_ShouldRespectMaxLength(string input, int maxLength, string expected)
		{
			var result = _helper.Truncate(input, maxLength);
			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData("Dobrý den světe!", "dobry-den-svete")]
		[InlineData("Čau123 světe!", "cau123-svete")]
		[InlineData(null, "")]
		public void Slugify_ShouldGenerateUrlSafeSlug(string input, string expected)
		{
			var result = _helper.Slugify(input);
			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData("krasny-den", "krasny den")]
		[InlineData("multi--slug", "multi  slug")]
		[InlineData("", "")]
		[InlineData(null, "")]
		public void Unslugify_ShouldReplaceDashesWithSpaces(string input, string expected)
		{
			var result = _helper.Unslugify(input);
			Assert.Equal(expected, result);
		}
	}
}
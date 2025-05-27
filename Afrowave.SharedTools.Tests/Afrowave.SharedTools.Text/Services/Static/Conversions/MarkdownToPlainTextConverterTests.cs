using Afrowave.SharedTools.Text.Static.Conversions;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Text.Services.Static.Conversions
{
	public class MarkdownToPlainTextConverterTests
	{
		[Fact]
		public void Converts_Table_TabDelimited()
		{
			var md = "| Name  | Age |\n|-------|-----|\n| Tom   | 10  |\n| Anna  | 9   |";
			var expected =
	  @"Name	Age
Tom	10
Anna	9";

			var plain = MarkdownToPlainTextConverter.Convert(md);

			// Normalize řádky a trim
			string Normalize(string s) =>
				 s.Replace("\r", "").Trim();

			Assert.Equal(Normalize(expected), Normalize(plain));
		}

		[Fact]
		public async Task Converts_Stream_Table_TabDelimited()
		{
			var md = "| Name  | Age |\n|-------|-----|\n| Tom   | 10  |\n| Anna  | 9   |";
			var expected =
	  @"Name	Age
Tom	10
Anna	9";

			using(var reader = new StringReader(md))
			using(var writer = new StringWriter())
			{
				await MarkdownToPlainTextConverter.ConvertStreamAsync(reader, writer);
				string result = writer.ToString();
				string Normalize(string s) => s.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
				Assert.Equal(Normalize(expected), Normalize(result));
			}
		}
	}
}
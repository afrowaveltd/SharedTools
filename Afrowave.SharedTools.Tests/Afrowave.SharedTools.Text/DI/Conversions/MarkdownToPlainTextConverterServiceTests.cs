using Afrowave.SharedTools.Text.DI.Conversions;

namespace Afrowave.SharedTools.Text.Tests.DI.Conversions
{
	public class MarkdownToPlainTextConverterServiceTests
	{
		private readonly IMarkdownToPlainTextConverterService _service = new MarkdownToPlainTextConverterService();

		private string Normalize(string s) => s.Replace("\r\n", "\n").Replace("\r", "\n").Trim();

		[Fact]
		public void Converts_Table_TabDelimited()
		{
			var md = "| Name  | Age |\n|-------|-----|\n| Tom   | 10  |\n| Anna  | 9   |";
			var expected =
@"Name	Age
Tom	10
Anna	9";

			var plain = _service.Convert(md);
			Assert.Equal(Normalize(expected), Normalize(plain));
		}

		[Fact]
		public void Converts_Table_Complicated_TabDelimited()
		{
			var md = "| City      | Population | Notes         |\n|-----------|------------|---------------|\n| London    | 9000000    | Capital city  |\n| Paris     | 2100000    |               |\n| Prague    | 1300000    | Beautiful!    |";
			var expected =
	 "City\tPopulation\tNotes\n" +
	 "London\t9000000\tCapital city\n" +
	 "Paris\t2100000\t\n" +
	 "Prague\t1300000\tBeautiful!";

			var plain = _service.Convert(md);
			Assert.Equal(Normalize(expected), Normalize(plain));
		}

		[Fact]
		public void Converts_Heading_And_List()
		{
			var md = "# My Heading\n\n- Item one\n- Item two";
			var expected =
@"MY HEADING

- Item one
- Item two";

			var plain = _service.Convert(md);
			Assert.Equal(Normalize(expected), Normalize(plain));
		}

		[Fact]
		public void Converts_CodeBlock()
		{
			var md = "```csharp\nvar x = 1;\n```";
			var plain = _service.Convert(md);
			Assert.Contains("CODE:", plain);
			Assert.Contains("ENDCODE", plain);
			Assert.Contains("var x = 1;", plain);
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
				await _service.ConvertStreamAsync(reader, writer);
				string result = writer.ToString();
				Assert.Equal(Normalize(expected), Normalize(result));
			}
		}

		[Fact]
		public async Task Converts_Stream_Heading_And_List()
		{
			var md = "# My Heading\n\n- Item one\n- Item two";
			var expected =
@"MY HEADING

- Item one
- Item two";

			using(var reader = new StringReader(md))
			using(var writer = new StringWriter())
			{
				await _service.ConvertStreamAsync(reader, writer);
				string result = writer.ToString();
				Assert.Equal(Normalize(expected), Normalize(result));
			}
		}

		[Fact]
		public async Task Converts_Stream_CodeBlock()
		{
			var md = "```csharp\nvar x = 1;\n```";
			using(var reader = new StringReader(md))
			using(var writer = new StringWriter())
			{
				await _service.ConvertStreamAsync(reader, writer);
				string result = writer.ToString();
				Assert.Contains("CODE:", result);
				Assert.Contains("ENDCODE", result);
				Assert.Contains("var x = 1;", result);
			}
		}
	}
}
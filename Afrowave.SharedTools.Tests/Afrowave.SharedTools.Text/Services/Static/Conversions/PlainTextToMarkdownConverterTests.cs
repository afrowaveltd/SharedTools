using Afrowave.SharedTools.Text.Static.Conversions;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Text.Services.Static.Conversions
{
	public class PlainTextToMarkdownConverterTests
	{
		[Fact]
		public void Converts_TabDelimited_Table_To_Markdown()
		{
			var txt = "City\tPopulation\tNotes\nLondon\t9000000\tCapital city\nParis\t2100000\t\nPrague\t1300000\tBeautiful!";
			var md = PlainTextToMarkdownConverter.Convert(txt);

			Assert.Contains("| City | Population | Notes |", md);
			Assert.Contains("| --- | --- | --- |", md);
			Assert.Contains("| London | 9000000 | Capital city |", md);
			Assert.Contains("| Paris | 2100000 |  |", md);
			Assert.Contains("| Prague | 1300000 | Beautiful! |", md);
		}

		[Fact]
		public void Converts_Bullet_Lists()
		{
			var txt = "- First\n- Second\n* Third\n• Fourth";
			var md = PlainTextToMarkdownConverter.Convert(txt);

			// Všechny převede na Markdown odrážky "- "
			Assert.Contains("- First", md);
			Assert.Contains("- Second", md);
			Assert.Contains("- Third", md);
			Assert.Contains("- Fourth", md);
		}

		[Fact]
		public void Converts_Ordered_Lists()
		{
			var txt = "1. First\n2. Second\n10. Tenth";
			var md = PlainTextToMarkdownConverter.Convert(txt);

			Assert.Contains("1. First", md);
			Assert.Contains("2. Second", md);
			Assert.Contains("10. Tenth", md);
		}

		[Fact]
		public void Converts_Code_Blocks()
		{
			var txt = "Some intro\nCODE:\nvar x = 5;\nENDCODE\nSummary";
			var md = PlainTextToMarkdownConverter.Convert(txt);

			Assert.Contains("```", md); // Začátek a konec code blocku
			Assert.Contains("var x = 5;", md);
		}

		[Fact]
		public void Converts_Heading_From_AllCaps()
		{
			var txt = "MY MAIN HEADING\nSomething else";
			var md = PlainTextToMarkdownConverter.Convert(txt);

			// Heading jako # My main heading
			Assert.Contains("# My main heading", md);
		}

		[Fact]
		public void Converts_Quote()
		{
			var txt = "> This is a quote";
			var md = PlainTextToMarkdownConverter.Convert(txt);

			Assert.Contains("> This is a quote", md);
		}

		[Fact]
		public void Converts_Mixed_Content()
		{
			var txt = "MY REPORT\n\n- Item one\n- Item two\n\nCity\tPopulation\nPrague\t1300000\nBrno\t400000\n\nCODE:\nprint('Hello')\nENDCODE";
			var md = PlainTextToMarkdownConverter.Convert(txt);

			Assert.Contains("# My report", md);
			Assert.Contains("- Item one", md);
			Assert.Contains("| City | Population |", md);
			Assert.Contains("| Prague | 1300000 |", md);
			Assert.Contains("```", md);
			Assert.Contains("print('Hello')", md);
		}
	}
}
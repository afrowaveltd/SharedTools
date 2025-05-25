
using Afrowave.SharedTools.Text.Models.Markdown;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Text.DI.Conversions
{
	/// <summary>
	/// Interface for DI-based HTML to Markdown conversion.
	/// </summary>
	public interface IHtmlToMarkdownConverterService
	{
		string Convert(string html, List<MarkdownTagMapping> mappings);
		Task ConvertStreamAsync(TextReader input, TextWriter output, List<MarkdownTagMapping> mappings, CancellationToken cancellationToken = default);
	}
}

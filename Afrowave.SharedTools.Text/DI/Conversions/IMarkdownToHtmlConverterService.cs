using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Text.DI.Conversions
{
	/// <summary>
	/// Interface for DI-based Markdown to HTML conversion.
	/// </summary>
	public interface IMarkdownToHtmlConverterService
	{
		string Convert(string markdown, string className = "", bool escapeMarkdown = false);

		/// <summary>
		/// Converts markdown from a stream (TextReader) to HTML stream (TextWriter) asynchronously.
		/// </summary>
		Task ConvertStreamAsync(TextReader input, TextWriter output, string className = "", bool escapeMarkdown = false, CancellationToken cancellationToken = default);
	}
}
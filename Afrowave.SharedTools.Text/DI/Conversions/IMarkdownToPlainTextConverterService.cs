using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Text.DI.Conversions
{
	public interface IMarkdownToPlainTextConverterService
	{
		string Convert(string markdown);

		/// <summary>
		/// Asynchronously converts markdown from a stream (TextReader) to plain text stream (TextWriter).
		/// </summary>
		Task ConvertStreamAsync(TextReader input, TextWriter output, CancellationToken cancellationToken = default);
	}
}
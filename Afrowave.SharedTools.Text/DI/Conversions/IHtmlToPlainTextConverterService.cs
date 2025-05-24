namespace Afrowave.SharedTools.Text.DI.Conversions
{
	/// <summary>
	/// Interface for DI-based HTML to plain text conversion service.
	/// </summary>
	public interface IHtmlToPlainTextConverterService
	{
		string Convert(string html);
	}
}
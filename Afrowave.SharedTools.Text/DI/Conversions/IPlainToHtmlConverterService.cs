namespace Afrowave.SharedTools.Text.DI.Conversions
{
	/// <summary>
	/// Interface for DI-based plain text to HTML converter.
	/// </summary>
	public interface IPlainToHtmlConverterService
	{
		string Convert(string input, bool minify = false);
	}
}
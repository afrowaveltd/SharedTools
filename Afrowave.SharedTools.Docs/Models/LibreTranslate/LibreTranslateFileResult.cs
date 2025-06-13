namespace Afrowave.SharedTools.Docs.Models.LibreTranslate
{
	/// <summary>
	/// Represents the result of a file translation operation performed by LibreTranslate.
	/// </summary>
	/// <remarks>This class contains the URL of the translated file, which can be used to access the file after the
	/// translation process.</remarks>
	public class LibreTranslateFileResult
	{
		/// <summary>
		/// Gets or sets the URL of the translated file.
		/// </summary>
		public string TranslatedFileUrl { get; set; } = string.Empty;
	}
}
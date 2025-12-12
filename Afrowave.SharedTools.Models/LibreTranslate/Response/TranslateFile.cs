using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.Models.LibreTranslate.Response
{
   /// <summary>
   /// Represents the result of a file translation operation performed by LibreTranslate.
   /// </summary>
   /// <remarks>
   /// This class contains the URL of the translated file, which can be used to access the file after
   /// the translation process.
   /// </remarks>
   public class TranslateFile
   {
      /// <summary>
      /// Gets or sets the URL of the translated file.
      /// </summary>

      [JsonPropertyName("translatedFileUrl")]
      public string TranslatedFileUrl { get; set; } = string.Empty;
   }
}
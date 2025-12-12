using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.Models.Localization
{
   public class Country
   {
      [JsonPropertyName("name")]
      public string Name { get; set; } = string.Empty;

      [JsonPropertyName("dial_code")]
      public string Dial_code { get; set; } = string.Empty;

      [JsonPropertyName("emoji")]
      public string Emoji { get; set; } = string.Empty;

      [JsonPropertyName("code")]
      public string Code { get; set; } = string.Empty;
   }
}
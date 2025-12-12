namespace Afrowave.SharedTools.Text.Models.Markdown
{
   /// <summary>
   /// Represents a mapping between Markdown syntax and corresponding HTML tags.
   /// </summary>
   public class MarkdownTagMapping
   {
      public string Markdown { get; set; }
      public string HtmlTag { get; set; }
      public bool IsPrefix { get; set; }
      public bool IsSuffix { get; set; }
      public bool IsBlock { get; set; }
      public bool RequiresClosingTag { get; set; }
      public bool IsSpecial { get; set; } // for links, images, etc.
      public string Class { get; set; } = string.Empty;
      public string Description { get; set; } = string.Empty;
   }
}
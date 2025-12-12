namespace Afrowave.SharedTools.Models.Ui
{
   /// <summary>
   /// Generic menu item for UI navigation; value is used for logic, text for display (can be localized).
   /// </summary>
   public class MenuItem<T>
   {
      /// <summary>
      /// The value/identifier of the menu item. Always used in logic, never shown to the user.
      /// </summary>
      public T Value { get; set; }

      /// <summary>
      /// Display text (localized as needed).
      /// </summary>
      public string Text { get; set; }

      public MenuItem(T value, string text)
      {
         Value = value;
         Text = text;
      }

      public override string ToString() => Text;
   }
}
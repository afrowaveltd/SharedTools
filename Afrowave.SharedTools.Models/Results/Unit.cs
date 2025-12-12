namespace Afrowave.SharedTools.Models.Results
{
   /// <summary>
   /// Represents a "no data" value for generic responses.
   /// Useful when you want Response&lt;T&gt; but do not want to return any payload.
   /// </summary>
   public readonly struct Unit
   {
      public static readonly Unit Value = new Unit();
      public override string ToString() => "Unit";
   }
}
namespace Core.Markup.Code.Extents
{
   public sealed class TextExtent : Extent
   {
      public TextExtent(string text)
      {
         Text = text;
      }

      public string Text { get; }
   }
}
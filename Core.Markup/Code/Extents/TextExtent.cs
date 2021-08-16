namespace Core.Markup.Code.Extents
{
   public class TextExtent : Extent
   {
      public TextExtent(string text)
      {
         Text = text;
      }

      public string Text { get; }
   }
}
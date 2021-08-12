namespace Core.Markup.Objects
{
   public sealed class TextExtent : Element
   {
      public TextExtent(string text)
      {
         Text = text;
      }

      public string Text { get; }
   }
}
namespace Core.Markup.Objects
{
   public sealed class Format : Element
   {
      public Format(string fontName, float fontSize, bool bold, bool italic)
      {
         FontName = fontName;
         FontSize = fontSize;
         Bold = bold;
         Italic = italic;
      }

      public string FontName { get; }

      public float FontSize { get; }

      public bool Bold { get; }

      public bool Italic { get; }
   }
}
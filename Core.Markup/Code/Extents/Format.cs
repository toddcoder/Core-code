using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Code.Extents
{
   public sealed class Format : Extent
   {
      private Maybe<string> _fontName;
      private Maybe<float> _fontSize;
      private Maybe<bool> _bold;
      private Maybe<bool> _italic;

      public Format()
      {
         _fontName = nil;
         _fontSize = nil;
         _bold = nil;
         _italic = nil;
      }

      public Maybe<string> FontName
      {
         get => _fontName;
         set => _fontName = value;
      }

      public Maybe<float> FontSize
      {
         get => _fontSize;
         set => _fontSize = value;
      }

      public Maybe<bool> Bold
      {
         get => _bold;
         set => _bold = value;
      }

      public Maybe<bool> Italic
      {
         get => _italic;
         set => _italic = value;
      }
   }
}
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class FontData
{
   public FontData(FontDescriptor font)
   {
      Font = font;
      FontSize = nil;
      FontStyle = nil;
      ForegroundColor = nil;
      BackgroundColor = nil;
      Bookmark = nil;
      LocalHyperlink = nil;
      LocalHyperlinkTip = nil;
   }

   public FontDescriptor Font { get; }

   public Maybe<float> FontSize { get; set; }

   public Maybe<FontStyleFlag> FontStyle { get; set; }

   public Maybe<ColorDescriptor> ForegroundColor { get; set; }

   public Maybe<ColorDescriptor> BackgroundColor { get; set; }

   public Maybe<string> Bookmark { get; set; }

   public Maybe<string> LocalHyperlink { get; set; }

   public Maybe<string> LocalHyperlinkTip { get; set; }

   public void SetCharFormat(CharFormat charFormat)
   {
      charFormat.Font = Font;
      charFormat.FontSize = FontSize;
      if (FontStyle)
      {
         charFormat.FontStyle += FontStyle;
      }

      charFormat.ForegroundColor = ForegroundColor;
      charFormat.BackgroundColor = BackgroundColor;
      charFormat.Bookmark = Bookmark;
      charFormat.LocalHyperlink = LocalHyperlink;
      charFormat.LocalHyperlinkTip = LocalHyperlinkTip;
   }
}
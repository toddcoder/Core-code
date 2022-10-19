using Core.Strings;

namespace Core.Markup.Rtf;

public class Formatter
{
   public static Formatter operator +(Formatter formatter, Feature feature) => feature switch
   {
      Feature.Bold => formatter.Bold(),
      Feature.Italic => formatter.Italic(),
      Feature.Underline => formatter.Underline(),
      Feature.Bullet => formatter.Bullet(),
      _ => formatter
   };

   public static Formatter operator +(Formatter formatter, FontData fontData) => formatter.FontData(fontData);

   public static Formatter operator +(Formatter formatter, Alignment alignment) => formatter.Alignment(alignment);

   public static Formatter operator +(Formatter formatter, ForegroundColorDescriptor foregroundColor) => formatter.ForegroundColor(foregroundColor);

   public static Formatter operator +(Formatter formatter, BackgroundColorDescriptor backgroundColor) => formatter.BackgroundColor(backgroundColor);

   public static Formatter operator +(Formatter formatter, (string hyperlink, string hyperlinkTip) local)
   {
      return formatter.LocalHyperlink(local.hyperlink, local.hyperlinkTip);
   }

   public static Formatter operator +(Formatter formatter, FontDescriptor font) => formatter.Font(font);

   public static Formatter operator +(Formatter formatter, float fontSize) => formatter.FontSize(fontSize);

   protected Paragraph paragraph;
   protected CharFormat format;

   public Formatter(Paragraph paragraph, CharFormat format)
   {
      this.paragraph = paragraph;
      this.format = format;
   }

   public Formatter Italic(bool on = true)
   {
      if (on)
      {
         format.FontStyle += FontStyleFlag.Italic;
      }
      else
      {
         format.FontStyle -= FontStyleFlag.Italic;
      }

      return this;
   }

   public Formatter Bold(bool on = true)
   {
      if (on)
      {
         format.FontStyle += FontStyleFlag.Bold;
      }
      else
      {
         format.FontStyle -= FontStyleFlag.Bold;
      }

      return this;
   }

   public Formatter Underline(bool on = true)
   {
      if (on)
      {
         format.FontStyle += FontStyleFlag.Underline;
      }
      else
      {
         format.FontStyle -= FontStyleFlag.Underline;
      }

      return this;
   }

   public Formatter Bullet()
   {
      paragraph.Bullet = true;
      return this;
   }

   public Formatter FontData(FontData fontData)
   {
      format.FontData = fontData;
      return this;
   }

   public Formatter FontSize(float fontSize)
   {
      format.FontSize = fontSize;
      return this;
   }

   public Formatter Alignment(Alignment alignment)
   {
      paragraph.Alignment = alignment;
      return this;
   }

   public Formatter ForegroundColor(ColorDescriptor foregroundColor)
   {
      format.ForegroundColor = foregroundColor;
      return this;
   }

   public Formatter BackgroundColor(ColorDescriptor backgroundColor)
   {
      format.BackgroundColor = backgroundColor;
      return this;
   }

   public Formatter Font(FontDescriptor font)
   {
      format.Font = font;
      return this;
   }

   public Formatter FontStyle(FontStyleFlag fontStyleFlag)
   {
      format.FontStyle += fontStyleFlag;
      return this;
   }

   public Formatter LocalHyperlink(string localHyperlink, string localHyperlinkTip = "")
   {
      format.LocalHyperlink = localHyperlink;
      if (localHyperlinkTip.IsNotEmpty())
      {
         format.LocalHyperlinkTip = localHyperlinkTip;
      }

      return this;
   }

   public Formatter Bookmark(string bookmark)
   {
      format.Bookmark = bookmark;
      return this;
   }
}
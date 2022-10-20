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

   public static Formatter operator +(Formatter formatter, LocalHyperlink localHyperlink) => formatter.LocalHyperlink(localHyperlink);

   public static Formatter operator +(Formatter formatter, FontDescriptor font) => formatter.Font(font);

   public static Formatter operator +(Formatter formatter, float fontSize) => formatter.FontSize(fontSize);

   public static Formatter operator +(Formatter formatter, FirstLineIndent firstLineIndent) => formatter.FirstLineIndent(firstLineIndent.Amount);

   public static Paragraph operator +(Formatter formatter, Paragraph _) => formatter.Paragraph;

   protected Paragraph paragraph;
   protected CharFormat format;

   public Formatter(Paragraph paragraph, CharFormat format)
   {
      this.paragraph = paragraph;
      this.format = format;
   }

   public Paragraph Paragraph => paragraph;

   public CharFormat CharFormat => format;

   public virtual Formatter Italic(bool on = true)
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

   public virtual Formatter Bold(bool on = true)
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

   public virtual Formatter Underline(bool on = true)
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

   public virtual Formatter Bullet()
   {
      paragraph.Bullet = true;
      return this;
   }

   public virtual Formatter FontData(FontData fontData)
   {
      format.FontData = fontData;
      return this;
   }

   public virtual Formatter FontSize(float fontSize)
   {
      format.FontSize = fontSize;
      return this;
   }

   public virtual Formatter Alignment(Alignment alignment)
   {
      paragraph.Alignment = alignment;
      return this;
   }

   public virtual Formatter ForegroundColor(ColorDescriptor foregroundColor)
   {
      format.ForegroundColor = foregroundColor;
      return this;
   }

   public virtual Formatter BackgroundColor(ColorDescriptor backgroundColor)
   {
      format.BackgroundColor = backgroundColor;
      return this;
   }

   public virtual Formatter Font(FontDescriptor font)
   {
      format.Font = font;
      return this;
   }

   public virtual Formatter FontStyle(FontStyleFlag fontStyleFlag)
   {
      format.FontStyle += fontStyleFlag;
      return this;
   }

   public virtual Formatter LocalHyperlink(LocalHyperlink localHyperlink)
   {
      format.LocalHyperlink = localHyperlink.Link;
      format.LocalHyperlinkTip = localHyperlink.LinkTip;

      return this;
   }

   public virtual Formatter Bookmark(string bookmark)
   {
      format.Bookmark = bookmark;
      return this;
   }

   public virtual Formatter FirstLineIndent(float indentAmount)
   {
      paragraph.FirstLineIndent = indentAmount;
      return this;
   }
}
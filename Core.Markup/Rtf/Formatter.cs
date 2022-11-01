using Core.Monads;

namespace Core.Markup.Rtf;

public class Formatter
{
   public static Formatter Empty => new();

   public static Formatter operator |(Formatter formatter, Feature feature) => feature switch
   {
      Feature.Bold => formatter.Bold(),
      Feature.Italic => formatter.Italic(),
      Feature.Underline => formatter.Underline(),
      Feature.Bullet => formatter.Bullet(),
      Feature.NewPage => formatter.NewPage(),
      Feature.NewPageAfter => formatter.NewPageAfter(),
      _ => formatter
   };

   public static Formatter operator |(Formatter formatter, Style style) => formatter.Style(style);

   public static Formatter operator |(Formatter formatter, Alignment alignment) => formatter.Alignment(alignment);

   public static Formatter operator |(Formatter formatter, ForegroundColorDescriptor foregroundColor) => formatter.ForegroundColor(foregroundColor);

   public static Formatter operator |(Formatter formatter, BackgroundColorDescriptor backgroundColor) => formatter.BackgroundColor(backgroundColor);

   public static Formatter operator |(Formatter formatter, Hyperlink hyperlink) => formatter.Hyperlink(hyperlink);

   public static Formatter operator |(Formatter formatter, Bookmark bookmark) => formatter.Bookmark(bookmark.Name);

   public static Formatter operator |(Formatter formatter, FontDescriptor font) => formatter.Font(font);

   public static Formatter operator |(Formatter formatter, float fontSize) => formatter.FontSize(fontSize);

   public static Formatter operator |(Formatter formatter, FirstLineIndent firstLineIndent) => formatter.FirstLineIndent(firstLineIndent.Amount);

   public static Formatter operator |(Formatter formatter, (Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins)
   {
      return formatter.Margins(margins);
   }

   public static Formatter operator |(Formatter formatter, FieldType fieldType) => formatter.ControlWord(fieldType);

   public static Paragraph operator |(Formatter formatter, Paragraph _) => formatter.Paragraph;

   public static Formatter operator |(Formatter formatter, string text)
   {
      var paragraph = formatter.Paragraph;
      var begin = paragraph.Text.Length;
      var end = begin + text.Length - 1;
      _ = paragraph | text;

      return new PartialFormatter(paragraph, paragraph.CharFormat(begin, end, false), begin, end);
   }

   protected Paragraph paragraph;
   protected CharFormat format;

   public Formatter(Paragraph paragraph, CharFormat format)
   {
      this.paragraph = paragraph;
      this.format = format;
   }

   protected Formatter()
   {
      paragraph = Paragraph.Empty;
      format = new CharFormat();
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

   public virtual Formatter NewPage()
   {
      paragraph.StartNewPage = true;
      return this;
   }

   public virtual Formatter NewPageAfter()
   {
      paragraph.StartNewPageAfter = true;
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

   public virtual Formatter Hyperlink(Hyperlink hyperlink)
   {
      format.Hyperlink = hyperlink.Link;
      format.HyperlinkTip = hyperlink.LinkTip;

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

   public virtual Formatter Margins((Maybe<float> left, Maybe<float> top, Maybe<float> right, Maybe<float> bottom) margins)
   {
      if (margins.left)
      {
         paragraph.Margins[Direction.Left] = margins.left;
      }

      if (margins.top)
      {
         paragraph.Margins[Direction.Top] = margins.top;
      }

      if (margins.right)
      {
         paragraph.Margins[Direction.Right] = margins.right;
      }

      if (margins.bottom)
      {
         paragraph.Margins[Direction.Bottom] = margins.bottom;
      }

      return this;
   }

   public virtual Formatter ControlWord(FieldType fieldType)
   {
      paragraph.ControlWord(paragraph.Text.Length - 1, fieldType);
      return this;
   }

   public virtual Formatter Style(Style style)
   {
      style.Merge(this);
      return this;
   }
}
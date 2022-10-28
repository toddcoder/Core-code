using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class Style
{
   public static Style operator ~(Style style) => style.Copy();

   public static Style operator |(Style style, Feature feature) => feature switch
   {
      Feature.Bold => style.Bold(),
      Feature.Italic => style.Italic(),
      Feature.Underline => style.Underline(),
      Feature.Bullet => style.Bullet(),
      Feature.NewPage => style.NewPage(),
      Feature.NewPageAfter => style.NewPageAfter(),
      _ => style
   };

   public static Style operator |(Style style, Style otherStyle) => style.CopyFrom(otherStyle);

   public static Style operator |(Style style, Alignment alignment) => style.Alignment(alignment);

   public static Style operator |(Style style, ForegroundColorDescriptor foregroundColor) => style.ForegroundColor(foregroundColor);

   public static Style operator |(Style style, BackgroundColorDescriptor backgroundColor) => style.BackgroundColor(backgroundColor);

   public static Style operator |(Style style, Hyperlink hyperlink) => style.Hyperlink(hyperlink);

   public static Style operator |(Style style, FontDescriptor font) => style.Font(font);

   public static Style operator |(Style style, float fontSize) => style.FontSize(fontSize);

   public static Style operator |(Style style, FirstLineIndent firstLineIndent) => style.FirstLineIndent(firstLineIndent);

   public static Style operator |(Style style, (Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins)
   {
      return style.Margins(margins);
   }

   protected Set<Feature> features;
   protected Maybe<Alignment> _alignment;
   protected Maybe<ForegroundColorDescriptor> _foregroundColor;
   protected Maybe<BackgroundColorDescriptor> _backgroundColor;
   protected Maybe<Hyperlink> _hyperlink;
   protected Maybe<FontDescriptor> _font;
   protected Maybe<float> _fontSize;
   protected Maybe<FirstLineIndent> _firstLineIndent;
   protected (Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins;

   public Style()
   {
      features = new Set<Feature>();
      _alignment = nil;
      _foregroundColor = nil;
      _backgroundColor = nil;
      _hyperlink = nil;
      _font = nil;
      _fontSize = nil;
      _firstLineIndent = nil;
      margins = (nil, nil, nil, nil);
   }

   public void SetParagraph(Paragraph paragraph)
   {
      if (_alignment)
      {
         paragraph.Alignment = _alignment;
      }

      if (_firstLineIndent)
      {
         paragraph.FirstLineIndent = (~_firstLineIndent).Amount;
      }

      var (_left, _top, _right, _bottom) = margins;
      if (_left)
      {
         paragraph.Margins[Direction.Left] = _left;
      }

      if (_top)
      {
         paragraph.Margins[Direction.Top] = _top;
      }

      if (_right)
      {
         paragraph.Margins[Direction.Right] = _right;
      }

      if (_bottom)
      {
         paragraph.Margins[Direction.Bottom] = _bottom;
      }
   }

   public void SetCharFormat(CharFormat charFormat)
   {
      foreach (var feature in features)
      {
         switch (feature)
         {
            case Feature.Bold:
               charFormat.FontStyle += FontStyleFlag.Bold;
               break;
            case Feature.Italic:
               charFormat.FontStyle += FontStyleFlag.Italic;
               break;
            case Feature.Underline:
               charFormat.FontStyle += FontStyleFlag.Underline;
               break;
         }
      }

      if (_foregroundColor)
      {
         charFormat.ForegroundColor = _foregroundColor.CastAs<ColorDescriptor>();
      }

      if (_backgroundColor)
      {
         charFormat.BackgroundColor = _backgroundColor.CastAs<ColorDescriptor>();
      }

      if (_hyperlink)
      {
         charFormat.Hyperlink = _hyperlink.Map(l => l.Link);
         charFormat.HyperlinkTip = _hyperlink.Map(l => l.LinkTip);
      }

      if (_font)
      {
         charFormat.Font = _font;
      }

      if (_fontSize)
      {
         charFormat.FontSize = _fontSize;
      }
   }

   public Style Copy()
   {
      var newStyle = new Style
      {
         _alignment = _alignment,
         _foregroundColor = _foregroundColor,
         _backgroundColor = _backgroundColor,
         _hyperlink = _hyperlink,
         _font = _font,
         _fontSize = _fontSize,
         _firstLineIndent = _firstLineIndent,
         margins = margins
      };
      foreach (var feature in features)
      {
         newStyle.features.Add(feature);
      }

      return newStyle;
   }

   public virtual Style CopyFrom(Style otherStyle)
   {
      features.Clear();
      features.AddRange(otherStyle.features);
      _alignment = otherStyle._alignment;
      _foregroundColor = otherStyle._foregroundColor;
      _backgroundColor = otherStyle._backgroundColor;
      _hyperlink = otherStyle._hyperlink;
      _font = otherStyle._font;
      _fontSize = otherStyle._fontSize;
      _firstLineIndent = otherStyle._firstLineIndent;
      margins = otherStyle.margins;

      return this;
   }

   public virtual Style Italic(bool on = true)
   {
      if (on)
      {
         features.Add(Feature.Italic);
      }
      else
      {
         features.Remove(Feature.Italic);
      }

      return this;
   }

   public virtual Style Bold(bool on = true)
   {
      if (on)
      {
         features.Add(Feature.Bold);
      }
      else
      {
         features.Remove(Feature.Bold);
      }

      return this;
   }

   public virtual Style Underline(bool on = true)
   {
      if (on)
      {
         features.Add(Feature.Underline);
      }
      else
      {
         features.Remove(Feature.Underline);
      }

      return this;
   }

   public virtual Style Bullet()
   {
      features.Add(Feature.Bullet);
      return this;
   }

   public virtual Style NewPage()
   {
      features.Add(Feature.NewPage);
      return this;
   }

   public virtual Style NewPageAfter()
   {
      features.Add(Feature.NewPageAfter);
      return this;
   }

   public Style Alignment(Alignment alignment)
   {
      _alignment = alignment;
      return this;
   }

   public Style ForegroundColor(ForegroundColorDescriptor foregroundColor)
   {
      _foregroundColor = foregroundColor;
      return this;
   }

   public Style BackgroundColor(BackgroundColorDescriptor backgroundColor)
   {
      _backgroundColor = backgroundColor;
      return this;
   }

   public Style Hyperlink(Hyperlink hyperlink)
   {
      _hyperlink = hyperlink;
      return this;
   }

   public Style Font(FontDescriptor font)
   {
      _font = font;
      return this;
   }

   public Style FontSize(float fontSize)
   {
      _fontSize = fontSize;
      return this;
   }

   public Style FirstLineIndent(FirstLineIndent firstLineIndent)
   {
      _firstLineIndent = firstLineIndent;
      return this;
   }

   public Style Margins((Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins)
   {
      this.margins = margins;
      return this;
   }

   public Formatter Formatter(Paragraph paragraph) => Formatter(paragraph, paragraph.DefaultCharFormat);

   public Formatter Formatter(Paragraph paragraph, CharFormat format)
   {
      var formatter = new Formatter(paragraph, format);

      foreach (var feature in features)
      {
         _ = formatter | feature;
      }

      if (_alignment)
      {
         formatter.Alignment(_alignment);
      }

      if (_foregroundColor)
      {
         formatter.ForegroundColor(_foregroundColor);
      }

      if (_backgroundColor)
      {
         formatter.BackgroundColor(_backgroundColor);
      }

      if (_hyperlink)
      {
         formatter.Hyperlink(_hyperlink);
      }

      if (_font)
      {
         formatter.Font(_font);
      }

      if (_fontSize)
      {
         formatter.FontSize(_fontSize);
      }

      if (_firstLineIndent)
      {
         formatter.FirstLineIndent((~_firstLineIndent).Amount);
      }

      formatter.Margins(margins);

      return formatter;
   }

   public void Merge(Formatter formatter)
   {
      foreach (var feature in features)
      {
         _ = formatter | feature;
      }

      if (_alignment)
      {
         formatter.Alignment(_alignment);
      }

      if (_foregroundColor)
      {
         formatter.ForegroundColor(_foregroundColor);
      }

      if (_backgroundColor)
      {
         formatter.BackgroundColor(_backgroundColor);
      }

      if (_hyperlink)
      {
         formatter.Hyperlink(_hyperlink);
      }

      if (_font)
      {
         formatter.Font(_font);
      }

      if (_fontSize)
      {
         formatter.FontSize(_fontSize);
      }

      if (_firstLineIndent)
      {
         formatter.FirstLineIndent((~_firstLineIndent).Amount);
      }

      formatter.Margins(margins);
   }
}
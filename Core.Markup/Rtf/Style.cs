using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class Style
{
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

   public static Style operator |(Style style, FontData fontData) => style.FontData(fontData);

   public static Style operator |(Style style, Alignment alignment) => style.Alignment(alignment);

   public static Style operator |(Style style, ForegroundColorDescriptor foregroundColor) => style.ForegroundColor(foregroundColor);

   public static Style operator |(Style style, BackgroundColorDescriptor backgroundColor) => style.BackgroundColor(backgroundColor);

   public static Style operator |(Style style, LocalHyperlink localHyperlink) => style.LocalHyperlink(localHyperlink);

   public static Style operator |(Style style, FontDescriptor font) => style.Font(font);

   public static Style operator |(Style style, float fontSize) => style.FontSize(fontSize);

   public static Style operator |(Style style, FirstLineIndent firstLineIndent) => style.FirstLineIndent(firstLineIndent);

   public static Style operator |(Style style, (Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins)
   {
      return style.Margins(margins);
   }

   protected Set<Feature> features;
   protected Maybe<FontData> _fontData;
   protected Maybe<Alignment> _alignment;
   protected Maybe<ForegroundColorDescriptor> _foregroundColor;
   protected Maybe<BackgroundColorDescriptor> _backgroundColor;
   protected Maybe<LocalHyperlink> _localHyperlink;
   protected Maybe<FontDescriptor> _font;
   protected Maybe<float> _fontSize;
   protected Maybe<FirstLineIndent> _firstLineIndent;
   protected (Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins;

   public Style()
   {
      features = new Set<Feature>();
      _fontData = nil;
      _alignment = nil;
      _foregroundColor = nil;
      _backgroundColor = nil;
      _localHyperlink = nil;
      _font = nil;
      _fontSize = nil;
      _firstLineIndent = nil;
      margins = (nil, nil, nil, nil);
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

   public Style FontData(FontData fontData)
   {
      _fontData = fontData;
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

   public Style LocalHyperlink(LocalHyperlink localHyperlink)
   {
      _localHyperlink = localHyperlink;
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

      if (_fontData)
      {
         formatter.FontData(_fontData);
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

      if (_localHyperlink)
      {
         formatter.LocalHyperlink(_localHyperlink);
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

      if (_fontData)
      {
         formatter.FontData(_fontData);
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

      if (_localHyperlink)
      {
         formatter.LocalHyperlink(_localHyperlink);
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
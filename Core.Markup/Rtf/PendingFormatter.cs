﻿using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class PendingFormatter
{
   public static PendingFormatter operator |(PendingFormatter pendingFormatter, string columnText)
   {
      return pendingFormatter.Table.ColumnPendingFormatter(columnText);
   }

   public static PendingFormatter operator &(PendingFormatter formatter, Feature feature) => feature switch
   {
      Feature.Bold => formatter.Bold(),
      Feature.Italic => formatter.Italic(),
      Feature.Underline => formatter.Underline(),
      Feature.Bullet => formatter.Bullet(),
      Feature.NewPage => formatter.NewPage(),
      Feature.NewPageAfter => formatter.NewPageAfter(),
      _ => formatter
   };

   public static PendingFormatter operator &(PendingFormatter formatter, FontData fontData) => formatter.FontData(fontData);

   public static PendingFormatter operator &(PendingFormatter formatter, Alignment alignment) => formatter.Alignment(alignment);

   public static PendingFormatter operator &(PendingFormatter formatter, ForegroundColorDescriptor foregroundColor) => formatter.ForegroundColor(foregroundColor);

   public static PendingFormatter operator &(PendingFormatter formatter, BackgroundColorDescriptor backgroundColor) => formatter.BackgroundColor(backgroundColor);

   public static PendingFormatter operator &(PendingFormatter formatter, LocalHyperlink localHyperlink) => formatter.LocalHyperlink(localHyperlink);

   public static PendingFormatter operator &(PendingFormatter formatter, FontDescriptor font) => formatter.Font(font);

   public static PendingFormatter operator &(PendingFormatter formatter, float fontSize) => formatter.FontSize(fontSize);

   public static PendingFormatter operator &(PendingFormatter formatter, FirstLineIndent firstLineIndent) => formatter.FirstLineIndent(firstLineIndent);

   protected Table table;
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

   public PendingFormatter(Table table)
   {
      this.table = table;

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

   public Table Table => table;

   public virtual PendingFormatter Italic(bool on = true)
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

   public virtual PendingFormatter Bold(bool on = true)
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

   public virtual PendingFormatter Underline(bool on = true)
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

   public virtual PendingFormatter Bullet()
   {
      features.Add(Feature.Bullet);
      return this;
   }

   public virtual PendingFormatter NewPage()
   {
      features.Add(Feature.NewPage);
      return this;
   }

   public virtual PendingFormatter NewPageAfter()
   {
      features.Add(Feature.NewPageAfter);
      return this;
   }

   public PendingFormatter FontData(FontData fontData)
   {
      _fontData = fontData;
      return this;
   }

   public PendingFormatter Alignment(Alignment alignment)
   {
      _alignment = alignment;
      return this;
   }

   public PendingFormatter ForegroundColor(ForegroundColorDescriptor foregroundColor)
   {
      _foregroundColor = foregroundColor;
      return this;
   }

   public PendingFormatter BackgroundColor(BackgroundColorDescriptor backgroundColor)
   {
      _backgroundColor = backgroundColor;
      return this;
   }

   public PendingFormatter LocalHyperlink(LocalHyperlink localHyperlink)
   {
      _localHyperlink = localHyperlink;
      return this;
   }

   public PendingFormatter Font(FontDescriptor font)
   {
      _font = font;
      return this;
   }

   public PendingFormatter FontSize(float fontSize)
   {
      _fontSize = fontSize;
      return this;
   }

   public PendingFormatter FirstLineIndent(FirstLineIndent firstLineIndent)
   {
      _firstLineIndent = firstLineIndent;
      return this;
   }

   public PendingFormatter Margins((Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins)
   {
      this.margins = margins;
      return this;
   }

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
}
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Core.Markup.Rtf;

public class RtfTemplate
{
   public abstract class FormatData
   {
      protected FormatData(int begin, int end)
      {
         Begin = begin;
         End = end;
         Partial = true;
         Index = nil;
         Length = nil;
         Slice = nil;
      }

      public FormatData()
      {
         Begin = -1;
         End = -1;
         Partial = false;
         Index = nil;
         Length = nil;
         Slice = nil;
      }

      public int Begin { get; }

      public int End { get; }

      public bool Partial { get; }

      public Maybe<int> Index { get; set; }

      public Maybe<int> Length { get; set; }

      public Maybe<string> Slice { get; set; }

      public CharFormat GetCharFormat(Paragraph paragraph) => paragraph.CharFormat(Begin, End);

      public abstract void SetCharFormat(CharFormat charFormat);
   }

   public class FontData : FormatData
   {
      public static FormatData Create(FontDescriptor font, int begin, int end, bool partial)
      {
         return partial ? new FontData(font) : new FontData(font, begin, end);
      }

      public FontData(FontDescriptor font, int begin, int end) : base(begin, end)
      {
         Font = font;
      }

      public FontData(FontDescriptor font)
      {
         Font = font;
      }

      public FontDescriptor Font { get; }

      public override void SetCharFormat(CharFormat charFormat) => charFormat.Font = Font;
   }

   public class ForegroundColorData : FormatData
   {
      public static FormatData Create(ColorDescriptor color, int begin, int end, bool partial)
      {
         return partial ? new ForegroundColorData(color, begin, end) : new ForegroundColorData(color);
      }

      public ForegroundColorData(ColorDescriptor color, int begin, int end) : base(begin, end)
      {
         Color = color;
      }

      public ForegroundColorData(ColorDescriptor color)
      {
         Color = color;
      }

      public ColorDescriptor Color { get; }

      public override void SetCharFormat(CharFormat charFormat)
      {
         charFormat.ForegroundColor = Color;
      }
   }

   public class BackgroundColorData : FormatData
   {
      public static FormatData Create(ColorDescriptor color, int begin, int end, bool partial)
      {
         return partial ? new BackgroundColorData(color, begin, end) : new BackgroundColorData(color);
      }

      public BackgroundColorData(ColorDescriptor color, int begin, int end) : base(begin, end)
      {
         Color = color;
      }

      public BackgroundColorData(ColorDescriptor color)
      {
         Color = color;
      }

      public ColorDescriptor Color { get; }

      public override void SetCharFormat(CharFormat charFormat)
      {
         charFormat.BackgroundColor = Color;
      }
   }

   public class FontSizeData : FormatData
   {
      public static FormatData Create(float fontSize, int begin, int end, bool partial)
      {
         return partial ? new FontSizeData(fontSize) : new FontSizeData(fontSize, begin, end);
      }

      public FontSizeData(float fontSize, int begin, int end) : base(begin, end)
      {
         FontSize = fontSize;
      }

      public FontSizeData(float fontSize)
      {
         FontSize = fontSize;
      }

      public float FontSize { get; }

      public override void SetCharFormat(CharFormat charFormat) => charFormat.FontSize = FontSize;
   }

   public class FontStyleData : FormatData
   {
      public static FormatData Create(IEnumerable<FontStyleFlag> fontStyleFlags, int begin, int end, bool partial)
      {
         return partial ? new FontStyleData(fontStyleFlags) : new FontStyleData(fontStyleFlags, begin, end);
      }

      public FontStyleData(IEnumerable<FontStyleFlag> fontStyleFlags, int begin, int end) : base(begin, end)
      {
         FontStyleFlags = fontStyleFlags;
      }

      public FontStyleData(IEnumerable<FontStyleFlag> fontStyleFlags)
      {
         FontStyleFlags = fontStyleFlags;
      }

      public IEnumerable<FontStyleFlag> FontStyleFlags { get; }

      public override void SetCharFormat(CharFormat charFormat)
      {
         foreach (var fontStyleFlag in FontStyleFlags)
         {
            charFormat.FontStyle += fontStyleFlag;
         }
      }
   }

   public class LocalHyperlinkData : FormatData
   {
      public static FormatData Create(string localHyperlink, string localHyperlinkTip, int begin, int end, bool partial)
      {
         return partial ? new LocalHyperlinkData(localHyperlink, localHyperlinkTip)
            : new LocalHyperlinkData(localHyperlink, localHyperlinkTip, begin, end);
      }

      public LocalHyperlinkData(string localHyperlink, string localHyperlinkTip, int begin, int end) : base(begin, end)
      {
         LocalHyperlink = localHyperlink;
         LocalHyperlinkTip = localHyperlinkTip;
      }

      public LocalHyperlinkData(string localHyperlink, string localHyperlinkTip)
      {
         LocalHyperlink = localHyperlink;
         LocalHyperlinkTip = localHyperlinkTip;
      }

      public string LocalHyperlink { get; }

      public string LocalHyperlinkTip { get; }

      public override void SetCharFormat(CharFormat charFormat)
      {
         charFormat.LocalHyperlink = LocalHyperlink;
         charFormat.LocalHyperlinkTip = LocalHyperlinkTip;
      }
   }

   protected const string PATTERN_NAME = "['a-z_'] ['a-z0-9_-']*";
   protected const string PATTERN_NAMES = $"{PATTERN_NAME}/s*";

   protected Document document;
   protected StringHash<Descriptor> descriptors;
   protected Hash<string, Pattern> patterns;

   public RtfTemplate(PaperSize paperSize = PaperSize.Letter, PaperOrientation paperOrientation = PaperOrientation.Portrait, Lcid lcid = Lcid.English)
   {
      document = new Document(paperSize, paperOrientation, lcid);

      descriptors = new StringHash<Descriptor>(true);

      patterns = new Hash<string, Pattern>
      {
         ["create-font"] = $"^ '//font' /s+ /({PATTERN_NAME}) /s* '//' /s* /(.+) $; f",
         ["create-color"] = $"^ '//color' /s+ /({PATTERN_NAME}) /s* '//' /s* /(.+) $; f",
         ["font"] = $"'font' /s+ /({PATTERN_NAME}) /b",
         ["foreground"] = $"'fg-color' /s+ /({PATTERN_NAME}) /b; f",
         ["background"] = $"'bg-color' /s+ /({PATTERN_NAME}) /b; f",
         ["font-size"] = "'font-size' /s+ /(['0-9.-']+); f",
         ["style"] = $"'[font-style' /s+ /({PATTERN_NAMES}) /b",
         ["link"] = $"'[link' /s+ /({PATTERN_NAMES}) ']'",
         ["alignment"] = $"^ 'alignment' /s+ /({PATTERN_NAME}) /b; f"
      };
   }

   protected void createFont(MatchResult result)
   {
      var (fontName, specifier) = result;
      var fontDescriptor = document.Font(specifier);
      descriptors[fontName] = fontDescriptor;
   }

   protected void createColor(MatchResult result)
   {
      var (colorName, specifier) = result;
      var colorDescriptor = document.Color(specifier);
      descriptors[colorName] = colorDescriptor;
   }

   protected static (int begin, int end) getBeginEnd(MatchResult result, int groupIndex)
   {
      var begin = result.Index;
      var length = result.GetGroup(0, groupIndex).Length;
      var end = result.Index + length - 1;

      return (begin, end);
   }

   protected static FormatData setSlice(MatchResult result, int groupIndex, FormatData formatData)
   {
      if (!formatData.Partial)
      {
         var index = result.Index;
         var length = result.Length;
         var slice = result.GetGroup(0, groupIndex).Text;

         formatData.Index = index;
         formatData.Length = length;
         formatData.Slice = slice;
      }

      return formatData;
   }

   protected Maybe<FormatData> useFont(MatchResult result, bool partial)
   {
      var (begin, end) = getBeginEnd(result, 1);
      var name = result.FirstGroup;
      var _descriptor = descriptors.Of(name);
      if (_descriptor && ~_descriptor is FontDescriptor fontDescriptor)
      {
         var formatData = FontData.Create(fontDescriptor, begin, end, partial);
         return setSlice(result, 1, formatData);
      }
      else
      {
         return nil;
      }
   }

   protected Maybe<FormatData> useForegroundColor(MatchResult result, bool partial)
   {
      var (begin, end) = getBeginEnd(result, 1);
      var name = result.FirstGroup;
      var _descriptor = descriptors.Of(name);
      if (_descriptor && ~_descriptor is ColorDescriptor colorDescriptor)
      {
         var formatData = ForegroundColorData.Create(colorDescriptor, begin, end, partial);
         return setSlice(result, 1, formatData);
      }
      else
      {
         return nil;
      }
   }

   protected Maybe<FormatData> useBackgroundColor(MatchResult result, bool partial)
   {
      var (begin, end) = getBeginEnd(result, 1);
      var name = result.FirstGroup;
      var _descriptor = descriptors.Of(name);
      if (_descriptor && ~_descriptor is ColorDescriptor colorDescriptor)
      {
         var formatData = BackgroundColorData.Create(colorDescriptor, begin, end, partial);
         return setSlice(result, 1, formatData);
      }
      else
      {
         return nil;
      }
   }

   protected static Maybe<FormatData> setFontSize(MatchResult result, bool partial)
   {
      var (begin, end) = getBeginEnd(result, 1);
      var _fontSize = Maybe.Single(result.FirstGroup);

      return _fontSize.Map(fs =>
      {
         var formatData = FontSizeData.Create(fs, begin, end, partial);
         return setSlice(result, 1, formatData);
      });
   }

   protected static Maybe<FormatData> setStyle(MatchResult result, bool partial)
   {
      IEnumerable<FontStyleFlag> getFontStyleFlags()
      {
         var styles = result.FirstGroup.Unjoin("/s+; f");
         foreach (var style in styles)
         {
            Maybe<FontStyleFlag> _flag = style.ToLower() switch
            {
               "bold" => FontStyleFlag.Bold,
               "italic" => FontStyleFlag.Italic,
               "underline" => FontStyleFlag.Underline,
               "super" => FontStyleFlag.Super,
               "sub" => FontStyleFlag.Sub,
               "scaps" => FontStyleFlag.Scaps,
               "strike" => FontStyleFlag.Strike,
               _ => nil
            };
            if (_flag)
            {
               yield return _flag;
            }
         }
      }

      var (begin, end) = getBeginEnd(result, 1);
      var formatData = FontStyleData.Create(getFontStyleFlags(), begin, end, partial);
      return setSlice(result, 1, formatData);
   }

   protected static Maybe<FormatData> localHyperlink(MatchResult result, bool partial)
   {
      var (begin, end) = getBeginEnd(result, 1);
      var name = result.FirstGroup;
      var items = name.Unjoin("/s+; f");
      var hyperlink = string.Empty;
      var hyperlinkTip = string.Empty;
      switch (items.Length)
      {
         case 0:
            return nil;
         case 1:
            hyperlink = items[0];
            break;
         case > 1:
            hyperlink = items[0];
            hyperlinkTip = items[1];
            break;
      }

      var formatData = LocalHyperlinkData.Create(hyperlink, hyperlinkTip, begin, end, partial);
      return setSlice(result, 1, formatData);
   }

   protected static Maybe<Alignment> setAlignment(MatchResult result)
   {
      var alignment = result.FirstGroup;
      return alignment.ToLower() switch
      {
         "left" => Alignment.Left,
         "right" => Alignment.Right,
         "center" => Alignment.Center,
         "fully-justify" => Alignment.FullyJustify,
         "distributed" => Alignment.Distributed,
         _ => nil
      };
   }

   public Result<Document> Render(string template)
   {
      try
      {
         var source = new Source(template);

         while (source.More)
         {
            var _line = source.NextLine();
            if (!_line)
            {
               break;
            }

            var line = ~_line;
            var data = new List<FormatData>();
            Maybe<FormatData> _formatData = nil;
            Maybe<Alignment> _alignment = nil;
            Maybe<string> _paragraphText = nil;

            while (true)
            {
               var _tagged = patterns.Matches(line);
               if (_tagged)
               {
                  var (tag, result) = ~_tagged;
                  switch (tag)
                  {
                     case "create-font":
                        createFont(result);
                        break;
                     case "create-color":
                        createColor(result);
                        break;
                     case "use-font":
                        _formatData = useFont(result, false);
                        break;
                     case "use-font-partial":
                        _formatData = useFont(result, true);
                        break;
                     case "use-foreground":
                        _formatData = useForegroundColor(result, false);
                        break;
                     case "use-foreground-partial":
                        _formatData = useForegroundColor(result, true);
                        break;
                     case "use-background":
                        _formatData = useBackgroundColor(result, false);
                        break;
                     case "use-background-partial":
                        _formatData = useBackgroundColor(result, true);
                        break;
                     case "font-size":
                        _formatData = setFontSize(result, false);
                        break;
                     case "font-size-partial":
                        _formatData = setFontSize(result, true);
                        break;
                     case "style":
                        _formatData = setStyle(result, false);
                        break;
                     case "style-partial":
                        _formatData = setStyle(result, true);
                        break;
                     case "link":
                        _formatData = localHyperlink(result, false);
                        break;
                     case "link-partial":
                        _formatData = localHyperlink(result, true);
                        break;
                     case "alignment":
                        _alignment = setAlignment(result);
                        break;
                  }
               }
               else
               {
                  _paragraphText = line;
               }

               if (_formatData)
               {
                  data.Add(_formatData);
                  _formatData = nil;
               }
               else
               {
                  break;
               }
            }

            var paragraph = document.Paragraph();
            if (_paragraphText)
            {
               paragraph.Text = _paragraphText;
            }
            else
            {
               Slicer slicer = line;
               foreach (var formatData in data.Where(f => f.Partial))
               {
                  if (formatData.Slice && formatData.Index && formatData.Length)
                  {
                     slicer[formatData.Index, formatData.Length] = formatData.Slice;
                  }
               }

               paragraph.Text = slicer.ToString();

               foreach (var formatData in data)
               {
                  if (formatData.Partial)
                  {
                     var charFormat = formatData.GetCharFormat(paragraph);
                     formatData.SetCharFormat(charFormat);
                  }
                  else
                  {
                     formatData.SetCharFormat(paragraph.DefaultCharFormat);
                  }
               }

               if (_alignment)
               {
                  paragraph.Alignment = _alignment;
               }
            }
         }

         return document;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}
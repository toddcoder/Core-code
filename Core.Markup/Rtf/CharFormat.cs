using System.Text;
using Core.Assertions;
using Core.Collections;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf
{
   public class CharFormat
   {
      protected static Hash<FontStyleFlag, string> fontStyleMap;

      static CharFormat()
      {
         fontStyleMap = new Hash<FontStyleFlag, string>
         {
            [FontStyleFlag.Bold] = "b",
            [FontStyleFlag.Italic] = "i",
            [FontStyleFlag.Scaps] = "scaps",
            [FontStyleFlag.Strike] = "strike",
            [FontStyleFlag.Sub] = "sub",
            [FontStyleFlag.Super] = "super",
            [FontStyleFlag.Underline] = "ul"
         };
      }

      public static LateLazy<CharFormat> Lazy() => new(errorMessage: "DefaultCharFormat referenced before being set");

      protected Maybe<int> _begin;
      protected Maybe<int> _end;
      protected Maybe<FontDescriptor> _font;
      protected Maybe<FontDescriptor> _ansiFont;
      protected Maybe<float> _fontSize;
      protected FontStyle fontStyle;
      protected Maybe<ColorDescriptor> _backgroundColor;
      protected Maybe<ColorDescriptor> _foregroundColor;
      protected TwoInOneStyle twoInOneStyle;
      protected Maybe<string> _bookmark;
      protected Maybe<string> _localHyperlink;
      protected Maybe<string> _localHyperlinkTip;

      public CharFormat()
      {
         _begin = none<int>();
         _end = none<int>();
         _font = none<FontDescriptor>();
         _ansiFont = none<FontDescriptor>();
         _fontSize = none<float>();
         fontStyle = new FontStyle();
         _backgroundColor = none<ColorDescriptor>();
         _foregroundColor = none<ColorDescriptor>();
         twoInOneStyle = TwoInOneStyle.NotEnabled;
         _bookmark = none<string>();
         _localHyperlink = none<string>();
         _localHyperlinkTip = none<string>();
      }

      public CharFormat(int begin, int end, int textLength)
      {
         var message = $"Invalid range: ({begin}, {end})";

         begin.Must().BeLessThanOrEqual(end).OrThrow(message);
         begin.Must().BeGreaterThanOrEqual(0).OrThrow(message);
         end.Must().BeLessThan(textLength).OrThrow(message);
         end.Must().BeGreaterThanOrEqual(0).OrThrow(message);

         _begin = begin;
         _end = end;
         _font = none<FontDescriptor>();
         _ansiFont = none<FontDescriptor>();
         _fontSize = none<float>();
         fontStyle = new FontStyle();
         _backgroundColor = none<ColorDescriptor>();
         _foregroundColor = none<ColorDescriptor>();
         twoInOneStyle = TwoInOneStyle.NotEnabled;
         _bookmark = none<string>();
         _localHyperlink = none<string>();
         _localHyperlinkTip = none<string>();
      }

      public void CopyFrom(CharFormat sourceFormat)
      {
         if (_begin.IsNone && sourceFormat._begin.If(out var begin))
         {
            _begin = begin;
         }

         if (_end.IsNone && sourceFormat._end.If(out var end))
         {
            _end = end;
         }

         if (_font.IsNone && sourceFormat._font.If(out var font))
         {
            _font = new FontDescriptor(font.Value);
         }

         if (_ansiFont.IsNone && sourceFormat._ansiFont.If(out var ansiFont))
         {
            _ansiFont = new FontDescriptor(ansiFont.Value);
         }

         if (_fontSize.IsNone && sourceFormat._fontSize.If(out var fontSize))
         {
            _fontSize = fontSize;
         }

         if (fontStyle.IsEmpty && !sourceFormat.fontStyle.IsEmpty)
         {
            fontStyle = new FontStyle(sourceFormat.fontStyle);
         }

         if (_backgroundColor.IsNone && sourceFormat._backgroundColor.If(out var backgroundColor))
         {
            _backgroundColor = new ColorDescriptor(backgroundColor.Value);
         }

         if (_foregroundColor.IsNone && sourceFormat._foregroundColor.If(out var foregroundColor))
         {
            _foregroundColor = new ColorDescriptor(foregroundColor.Value);
         }
      }

      public Maybe<int> Begin => _begin;

      public Maybe<int> End => _end;

      public Maybe<string> Bookmark
      {
         get => _bookmark;
         set => _bookmark = value;
      }

      public Maybe<string> LocalHyperlink
      {
         get => _localHyperlink;
         set => _localHyperlink = value;
      }

      public Maybe<string> LocalHyperlinkTip
      {
         get => _localHyperlinkTip;
         set => _localHyperlinkTip = value;
      }

      public Maybe<FontDescriptor> Font
      {
         get => _font;
         set => _font = value;
      }

      public Maybe<FontDescriptor> AnsiFont
      {
         get => _ansiFont;
         set => _ansiFont = value;
      }

      public Maybe<float> FontSize
      {
         get => _fontSize;
         set => _fontSize = value;
      }

      public FontStyle FontStyle
      {
         get => fontStyle;
         set => fontStyle = value;
      }

      public Maybe<ColorDescriptor> ForegroundColor
      {
         get => _foregroundColor;
         set => _foregroundColor = value;
      }

      public Maybe<ColorDescriptor> BackgroundColor
      {
         get => _backgroundColor;
         set => _backgroundColor = value;
      }

      public TwoInOneStyle TwoInOneStyle
      {
         get => twoInOneStyle;
         set => twoInOneStyle = value;
      }

      public string RenderHead()
      {
         var result = new StringBuilder("{");

         if (_localHyperlink.If(out var localHyperlink))
         {
            result.Append(@"{\field{\*\fldinst HYPERLINK \\l ");
            result.Append($"\"{localHyperlink}\"");
            if (_localHyperlinkTip.If(out var localHyperlinkTip))
            {
               result.Append($" \\\\o \"{localHyperlinkTip}\"");
            }

            result.Append(@"}{\fldrslt{");
         }

         if (_font.If(out var font))
         {
            if (_ansiFont.If(out var ansiFont))
            {
               result.Append($@"\loch\af{ansiFont.Value}\hich\af{ansiFont.Value}\dbch\af{font.Value}");
            }
            else
            {
               result.Append($@"\f{font.Value}");
            }
         }
         else if (_ansiFont.If(out var ansiFont))
         {
            result.Append(@"\f" + ansiFont.Value);
         }

         if (_fontSize.If(out var fontSize))
         {
            result.Append($@"\fs{fontSize.PointToHalfPoint()}");
         }

         if (_foregroundColor.If(out var foregroundColor))
         {
            result.Append($@"\cf{foregroundColor.Value}");
         }

         if (_backgroundColor.If(out var backgroundColor))
         {
            result.Append($@"\chshdng0\chcbpat{backgroundColor.Value}\cb{backgroundColor.Value}");
         }

         foreach (var (fontStyleFlag, value) in fontStyleMap)
         {
            if (FontStyle.ContainsStyleAdd(fontStyleFlag))
            {
               result.Append($@"\{value}");
            }
            else if (FontStyle.ContainsStyleRemove(fontStyleFlag))
            {
               result.Append($@"\{value}0");
            }
         }

         if (twoInOneStyle != TwoInOneStyle.NotEnabled)
         {
            result.Append(@"\twoinone");
            switch (twoInOneStyle)
            {
               case TwoInOneStyle.None:
                  result.Append("0");
                  break;
               case TwoInOneStyle.Parentheses:
                  result.Append("1");
                  break;
               case TwoInOneStyle.SquareBrackets:
                  result.Append("2");
                  break;
               case TwoInOneStyle.AngledBrackets:
                  result.Append("3");
                  break;
               case TwoInOneStyle.Braces:
                  result.Append("4");
                  break;
            }
         }

         if (result.ToString().Contains(@"\"))
         {
            result.Append(" ");
         }

         if (_bookmark.If(out var bookmark))
         {
            result.Append($@"{{\*\bkmkstart {bookmark}}}");
         }

         return result.ToString();
      }

      public string RenderTail()
      {
         var result = new StringBuilder(string.Empty);

         if (_bookmark.If(out var bookmark))
         {
            result.Append($@"{{\*\bkmkend {bookmark}}}");
         }

         if (_localHyperlink.IsSome)
         {
            result.Append(@"}}}");
         }

         result.Append("}");

         return result.ToString();
      }
   }
}
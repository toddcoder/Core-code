using System.Collections.Generic;
using System.Text;
using Core.Assertions;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class CharFormat
{
   protected static IEnumerable<FontStyleFlag> fontStyleFlags()
   {
      yield return FontStyleFlag.Bold;
      yield return FontStyleFlag.Italic;
      yield return FontStyleFlag.Scaps;
      yield return FontStyleFlag.Strike;
      yield return FontStyleFlag.Sub;
      yield return FontStyleFlag.Super;
      yield return FontStyleFlag.Underline;
   }

   protected static Maybe<string> fontStyleKeyword(FontStyleFlag fontStyleFlag) => fontStyleFlag switch
   {
      FontStyleFlag.Bold => "b",
      FontStyleFlag.Italic => "i",
      FontStyleFlag.Scaps => "scaps",
      FontStyleFlag.Strike => "strike",
      FontStyleFlag.Sub => "sub",
      FontStyleFlag.Super => "super",
      FontStyleFlag.Underline => "ul",
      _ => nil
   };

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
   protected Maybe<string> _hyperlink;
   protected Maybe<string> _hyperlinkTip;

   public CharFormat()
   {
      _begin = nil;
      _end = nil;
      _font = nil;
      _ansiFont = nil;
      _fontSize = nil;
      fontStyle = new FontStyle();
      _backgroundColor = nil;
      _foregroundColor = nil;
      twoInOneStyle = TwoInOneStyle.NotEnabled;
      _bookmark = nil;
      _hyperlink = nil;
      _hyperlinkTip = nil;
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
      _font = nil;
      _ansiFont = nil;
      _fontSize = nil;
      fontStyle = new FontStyle();
      _backgroundColor = nil;
      _foregroundColor = nil;
      twoInOneStyle = TwoInOneStyle.NotEnabled;
      _bookmark = nil;
      _hyperlink = nil;
      _hyperlinkTip = nil;
   }

   public void CopyFrom(CharFormat sourceFormat)
   {
      if (!_begin && sourceFormat.Begin)
      {
         _begin = sourceFormat.Begin;
      }

      if (!_end && sourceFormat.End)
      {
         _end = sourceFormat.End;
      }

      if (!_font && sourceFormat.Font)
      {
         _font = sourceFormat.Font.Map(font => new FontDescriptor(font.Value));
      }

      if (!_ansiFont && sourceFormat.AnsiFont)
      {
         _ansiFont = sourceFormat.AnsiFont.Map(font => new FontDescriptor(font.Value));
      }

      if (!_fontSize && sourceFormat.FontSize)
      {
         _fontSize = sourceFormat.FontSize;
      }

      if (fontStyle.IsEmpty && !sourceFormat.fontStyle.IsEmpty)
      {
         fontStyle = new FontStyle(sourceFormat.fontStyle);
      }

      if (!_backgroundColor && sourceFormat.BackgroundColor)
      {
         _backgroundColor = sourceFormat.BackgroundColor.Map(color => new ColorDescriptor(color.Value));
      }

      if (!_foregroundColor && sourceFormat.ForegroundColor)
      {
         _foregroundColor = sourceFormat.ForegroundColor.Map(color => new ColorDescriptor(color.Value));
      }
   }

   public Maybe<int> Begin => _begin;

   public Maybe<int> End => _end;

   public Maybe<string> Bookmark
   {
      get => _bookmark;
      set => _bookmark = value;
   }

   public Maybe<string> Hyperlink
   {
      get => _hyperlink;
      set => _hyperlink = value;
   }

   public Maybe<string> HyperlinkTip
   {
      get => _hyperlinkTip;
      set => _hyperlinkTip = value;
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

   public Style Style
   {
      set => value.SetCharFormat(this);
   }

   public string RenderHead()
   {
      var result = new StringBuilder("{");

      if (_hyperlink)
      {
         var localHyperlinkTip = _hyperlinkTip | ~_hyperlink;
         result.Append($@"{{\field{{\*\fldinst HYPERLINK ""{_hyperlink}""}}{{\fldrslt{{\ul\cf1 {localHyperlinkTip}}}}}}}");
      }

      if (_font)
      {
         var fontValue = (~_font).Value;
         if (_ansiFont)
         {
            var ansiFontValue = (~_ansiFont).Value;
            result.Append($@"\loch\af{ansiFontValue}\hich\af{ansiFontValue}\dbch\af{fontValue}");
         }
         else
         {
            result.Append($@"\f{fontValue}");
         }
      }
      else if (_ansiFont)
      {
         result.Append(@"\f" + (~_ansiFont).Value);
      }

      if (_fontSize)
      {
         result.Append($@"\fs{(~_fontSize).PointToHalfPoint()}");
      }

      if (_foregroundColor)
      {
         result.Append($@"\cf{(~_foregroundColor).Value}");
      }

      if (_backgroundColor)
      {
         var backgroundColorValue = (~_backgroundColor).Value;
         result.Append($@"\chshdng0\chcbpat{backgroundColorValue}\cb{backgroundColorValue}");
      }

      foreach (var fontStyleFlag in fontStyleFlags())
      {
         var _keyword = fontStyleKeyword(fontStyleFlag);
         if (_keyword)
         {
            if (FontStyle.ContainsStyleAdd(fontStyleFlag))
            {
               result.Append($@"\{_keyword}");
            }
            else if (FontStyle.ContainsStyleRemove(fontStyleFlag))
            {
               result.Append($@"\{_keyword}0");
            }
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

      if (_bookmark)
      {
         result.Append($@"{{\*\bkmkstart {_bookmark}}}");
      }

      return result.ToString();
   }

   public string RenderTail()
   {
      var result = new StringBuilder(string.Empty);

      if (_bookmark)
      {
         result.Append($@"{{\*\bkmkend {_bookmark}}}");
      }

      result.Append("}");

      return result.ToString();
   }
}
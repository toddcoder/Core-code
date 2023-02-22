using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.Lazy.LazyMonads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiActionWriter
{
   protected const string CHECK_MARK = "\u2713";

   public static TextFormatFlags GetFlags(bool center)
   {
      var flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
      if (center)
      {
         flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
      }
      else
      {
         flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding;
      }

      return flags;
   }

   protected CheckStyle checkStyle;
   protected Maybe<string> _emptyTextTitle;
   protected Result<Rectangle> _rectangle;
   protected Result<Font> _font;
   protected Result<Color> _color;

   public UiActionWriter(bool center, CheckStyle checkStyle, Maybe<string> emptyTextTitle)
   {
      Center(center || checkStyle != CheckStyle.None);
      this.checkStyle = checkStyle;
      _emptyTextTitle = emptyTextTitle;

      _rectangle = fail("Rectangle not set");
      _font = fail("Font not set");
      _color = fail("Color not set");
   }

   public void Center(bool center) => Flags = GetFlags(center);

   public Rectangle Rectangle
   {
      set => _rectangle = value;
   }

   public Font Font
   {
      set => _font = value;
   }

   public Color Color
   {
      set => _color = value;
   }

   public TextFormatFlags Flags { get; set; }

   protected static string substitutions(string text)
   {
      var _result = text.Matches("-(< '//') /('//' /([/w '-']+) '.'?); f");
      if (_result is (true, var result))
      {
         foreach (var match in result)
         {
            Maybe<string> _replacement = match.SecondGroup switch
            {
               "arrow" => "⇒",
               "check" => "✔",
               "x" => "✘",
               "dot" => "•",
               "degree" => "°",
               "copyright" => "©",
               "pilcrow" => "¶",
               "diamond" => "♦",
               "double-left" => "«",
               "double-right" => "»",
               "times" => "×",
               "divide" => "÷",
               "paws-left" => "„",
               "paws-right" => "“",
               "not-equal" => "≠",
               _ => nil
            };
            if (_replacement is (true, var replacement))
            {
               match.FirstGroup = replacement;
            }
         }

         return result.ToString().Replace("//", "/");
      }
      else
      {
         return text;
      }
   }

   public Result<Unit> Write(string text, Graphics graphics)
   {
      text = substitutions(text);

      var _existingRectangle = lazy.result(_rectangle);
      var _existingFont = _existingRectangle.Then(_font);
      var _existingColor = _existingFont.Then(_color);
      if (_existingColor)
      {
         Rectangle rectangle = _existingRectangle;
         Font font = _existingFont;
         Color color = _existingColor;

         try
         {
            graphics.HighQuality();
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            var isReplaced = text.IsEmpty() && _emptyTextTitle;
            if (isReplaced)
            {
               text = _emptyTextTitle;
               font = new Font(font, FontStyle.Italic);
            }

            TextRenderer.DrawText(graphics, text, font, rectangle, color, Flags);

            if (checkStyle != CheckStyle.None)
            {
               using var pen = new Pen(color, 1);
               var location = new Point(2, 2);
               var size = new Size(12, 12);
               var boxRectangle = new Rectangle(location, size);
               graphics.DrawRectangle(pen, boxRectangle);

               if (checkStyle == CheckStyle.Checked)
               {
                  boxRectangle.Offset(1, 0);
                  boxRectangle.Inflate(8, 8);
                  using var checkFont = new Font("Consolas", 8, FontStyle.Bold);
                  TextRenderer.DrawText(graphics, CHECK_MARK, font, boxRectangle, color, Flags);
               }
            }

            return unit;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
      else
      {
         return _existingColor.Exception;
      }
   }
}
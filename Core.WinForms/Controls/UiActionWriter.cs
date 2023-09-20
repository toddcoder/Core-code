using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using Core.Strings.Emojis;
using Core.WinForms.Drawing;
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

   protected TextFormatFlags getFlags(CardinalAlignment alignment)
   {
      Bits32<TextFormatFlags> flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
      flags[TextFormatFlags.PathEllipsis] = isFile;
      switch (alignment)
      {
         case CardinalAlignment.NorthWest:
            break;
         case CardinalAlignment.North:
            flags[TextFormatFlags.Top] = true;
            flags[TextFormatFlags.HorizontalCenter] = true;
            break;
         case CardinalAlignment.NorthEast:
            flags[TextFormatFlags.Top] = true;
            flags[TextFormatFlags.Right] = true;
            break;
         case CardinalAlignment.East:
            flags[TextFormatFlags.Right] = true;
            flags[TextFormatFlags.VerticalCenter] = true;
            break;
         case CardinalAlignment.SouthEast:
            flags[TextFormatFlags.Bottom] = true;
            flags[TextFormatFlags.Right] = true;
            break;
         case CardinalAlignment.South:
            flags[TextFormatFlags.Bottom] = true;
            flags[TextFormatFlags.HorizontalCenter] = true;
            break;
         case CardinalAlignment.SouthWest:
            flags[TextFormatFlags.Bottom] = true;
            flags[TextFormatFlags.Left] = true;
            break;
         case CardinalAlignment.West:
            flags[TextFormatFlags.Left] = true;
            flags[TextFormatFlags.VerticalCenter] = true;
            break;
         case CardinalAlignment.Center:
            flags[TextFormatFlags.HorizontalCenter] = true;
            flags[TextFormatFlags.VerticalCenter] = true;
            break;
      }

      return flags;
   }

   protected bool autoSizeText;
   protected Maybe<int> _floor;
   protected Maybe<int> _ceiling;
   protected CheckStyle checkStyle;
   protected Maybe<string> _emptyTextTitle;
   protected bool isFile;
   protected Result<Rectangle> _rectangle;
   protected Result<Font> _font;
   protected Result<Color> _color;
   protected UiActionButtonType buttonType;

   public UiActionWriter(CardinalAlignment messageAlignment, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling, UiActionButtonType buttonType)
   {
      Align(messageAlignment);
      this.autoSizeText = autoSizeText;
      this._floor = _floor;
      this._ceiling = _ceiling;
      this.buttonType = buttonType;

      isFile = false;
      checkStyle = CheckStyle.None;
      _emptyTextTitle = nil;

      _rectangle = fail("Rectangle not set");
      _font = fail("Font not set");
      _color = fail("Color not set");
   }

   public void Center(bool center) => Flags = GetFlags(center);

   public void Align(CardinalAlignment messageAlignment) => Flags = getFlags(messageAlignment);

   public CheckStyle CheckStyle
   {
      get => checkStyle;
      set => checkStyle = value;
   }

   public bool IsFile
   {
      get => isFile;
      set => isFile = value;
   }

   public Maybe<string> EmptyTextTitle
   {
      set => _emptyTextTitle = value;
   }

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

   public bool AutoSizeText
   {
      get => autoSizeText;
      set => autoSizeText = value;
   }

   public Size TextSize(Graphics g, string text)
   {
      var font = _font | (() => new Font("Consolas", 12f));
      return TextSize(g, text, font, Flags);
   }

   public static Size TextSize(Graphics g, string text, Font font, TextFormatFlags flags)
   {
      var proposedSize = new Size(int.MaxValue, int.MaxValue);
      return TextRenderer.MeasureText(g, text.EmojiSubstitutions(), font, proposedSize, flags);
   }

   public Rectangle TextRectangle(string text, Graphics graphics, Maybe<Rectangle> _rectangleToUse)
   {
      Rectangle rectangle;
      if (_rectangleToUse is (true, var rectangleToUse))
      {
         rectangle = rectangleToUse;
      }
      else if (_rectangle is (true, var currentRectangle))
      {
         rectangle = currentRectangle;
      }
      else
      {
         rectangle = graphics.ClipBounds.ToRectangle();
      }

      var textSize = TextSize(graphics, text);
      textSize = textSize with { Height = textSize.Height + 8, Width = textSize.Width + 8 };
      var x = rectangle.X + (rectangle.Width - textSize.Width) / 2;
      var y = rectangle.Y + (rectangle.Height - textSize.Height) / 2;

      return new Rectangle(x, y, textSize.Width, textSize.Height);
   }

   public Rectangle TextRectangle(string text, Graphics graphics) => TextRectangle(text, graphics, nil);

   protected void drawButtonType(Graphics g, Rectangle rectangle, Color foreColor)
   {
      switch (buttonType)
      {
         case UiActionButtonType.Default:
         {
            var upper = rectangle.Reposition(1, 1).Location;
            var lower = upper with { Y = rectangle.Bottom - 1 };
            using var pen = new Pen(foreColor, 2);
            g.DrawLine(pen, upper, lower);
            break;
         }
         case UiActionButtonType.Cancel:
         {
            var upper = rectangle.Reposition(1, 1).Location;
            var lower = upper with { Y = rectangle.Bottom - 1 };
            using var pen = new Pen(Color.FromArgb(128, foreColor), 2);
            pen.DashStyle = DashStyle.Dot;
            g.DrawLine(pen, upper, lower);
            break;
         }
      }
   }

   public Result<Unit> Write(Graphics g, string text)
   {
      text = text.EmojiSubstitutions();

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
            g.HighQuality();
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            var isReplaced = text.IsEmpty() && _emptyTextTitle;
            if (isReplaced)
            {
               text = _emptyTextTitle;
               font = new Font(font, FontStyle.Italic);
            }

            if (autoSizeText)
            {
               rectangle = AutoSizingWriter.NarrowRectangle(rectangle, _floor, _ceiling);
               drawButtonType(g, rectangle, color);
               var writer = new AutoSizingWriter(text, rectangle, color, font, isFile);
               writer.Write(g);
            }
            else
            {
               drawButtonType(g, rectangle, color);
               TextRenderer.DrawText(g, text, font, rectangle, color, Flags);
            }

            if (checkStyle is not CheckStyle.None)
            {
               using var pen = new Pen(color, 1);
               var location = new Point(2, 2);
               var size = new Size(12, 12);
               var boxRectangle = new Rectangle(location, size);
               g.DrawRectangle(pen, boxRectangle);

               if (checkStyle is CheckStyle.Checked)
               {
                  boxRectangle.Offset(1, 0);
                  boxRectangle.Inflate(8, 8);
                  using var checkFont = new Font("Consolas", 8, FontStyle.Bold);
                  TextRenderer.DrawText(g, CHECK_MARK, font, boxRectangle, color, Flags);
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
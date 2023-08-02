using System;
using System.Drawing;
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

   protected bool autoSize;
   protected Maybe<int> _floor;
   protected Maybe<int> _ceiling;
   protected CheckStyle checkStyle;
   protected Maybe<string> _emptyTextTitle;
   protected bool isFile;
   protected Result<Rectangle> _rectangle;
   protected Result<Font> _font;
   protected Result<Color> _color;

   public UiActionWriter(CardinalAlignment messageAlignment, bool autoSize, Maybe<int> _floor, Maybe<int> _ceiling)
   {
      Align(messageAlignment);
      this.autoSize = autoSize;
      this._floor = _floor;
      this._ceiling = _ceiling;

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

   public Size TextSize(string text, Graphics graphics)
   {
      var font = _font | (() => new Font("Consolas", 12f));
      var proposedSize = new Size(int.MaxValue, int.MaxValue);
      return TextRenderer.MeasureText(graphics, text.EmojiSubstitutions(), font, proposedSize, Flags);
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

      var textSize = TextSize(text, graphics);
      textSize = textSize with { Height = textSize.Height + 8, Width = textSize.Width + 8 };
      var x = (rectangle.Width - textSize.Width) / 2;
      var y = (rectangle.Height - textSize.Height) / 2;

      return new Rectangle(x, y, textSize.Width, textSize.Height);
   }

   public Rectangle TextRectangle(string text, Graphics graphics) => TextRectangle(text, graphics, nil);

   public Result<Unit> Write(string text, Graphics graphics)
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
            graphics.HighQuality();
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            var isReplaced = text.IsEmpty() && _emptyTextTitle;
            if (isReplaced)
            {
               text = _emptyTextTitle;
               font = new Font(font, FontStyle.Italic);
            }

            if (autoSize)
            {
               if (_floor is (true, var floor and >= 0))
               {
                  rectangle = rectangle with { X = floor, Width = rectangle.Width - floor };
               }

               if (_ceiling is (true, var ceiling))
               {
                  rectangle = rectangle with { Width = ceiling - rectangle.X };
               }

               var writer = new AutoSizingWriter(text, rectangle, color, font);
               writer.Write(graphics);
            }
            else
            {
               TextRenderer.DrawText(graphics, text, font, rectangle, color, Flags);
            }

            if (checkStyle is not CheckStyle.None)
            {
               using var pen = new Pen(color, 1);
               var location = new Point(2, 2);
               var size = new Size(12, 12);
               var boxRectangle = new Rectangle(location, size);
               graphics.DrawRectangle(pen, boxRectangle);

               if (checkStyle is CheckStyle.Checked)
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
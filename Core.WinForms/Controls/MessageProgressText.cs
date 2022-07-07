﻿using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls
{
   public class MessageProgressText
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
      protected Result<Rectangle> _rectangle;
      protected Result<Font> _font;
      protected Result<Color> _color;

      public MessageProgressText(bool center, CheckStyle checkStyle)
      {
         Center(center || checkStyle != CheckStyle.None);
         this.checkStyle = checkStyle;

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

      public Result<Unit> Write(string text, Graphics graphics)
      {
         var _arguments =
            from existingRectangle in _rectangle
            from existingFont in _font
            from existingColor in _color
            select (existingRectangle, existingFont, existingColor);
         if (_arguments.Map(out var rectangle, out var font, out var color, out var exception))
         {
            try
            {
               graphics.HighQuality();
               graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
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
            catch (Exception exception1)
            {
               return exception1;
            }
         }
         else
         {
            return exception;
         }
      }
   }
}
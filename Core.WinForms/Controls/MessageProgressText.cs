using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls
{
   public class MessageProgressText
   {
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

      protected Result<Rectangle> _rectangle;
      protected Result<Font> _font;
      protected Result<Color> _color;

      public MessageProgressText(bool center)
      {
         Center(center);
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
               TextRenderer.DrawText(graphics, text, font, rectangle, color, Flags);
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
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Core.WinForms.Controls
{
   public class ProgressDefiniteProcessor
   {
      protected Rectangle percentRectangle;
      protected Rectangle textRectangle;
      protected Font font;

      public ProgressDefiniteProcessor(Font font, Graphics graphics, Rectangle clientRectangle)
      {
         this.font = font;

         percentRectangle = getPercentRectangle(graphics, clientRectangle);
         textRectangle = getTextRectangle(clientRectangle);
      }

      public Rectangle PercentRectangle => percentRectangle;

      public Rectangle TextRectangle => textRectangle;

      protected Rectangle getPercentRectangle(Graphics graphics, Rectangle clientRectangle)
      {
         var size = TextRenderer.MeasureText(graphics, "100%", font);
         size = new Size(size.Width, clientRectangle.Height);
         return new Rectangle(clientRectangle.Location, size);
      }

      protected Rectangle getTextRectangle(Rectangle clientRectangle)
      {
         return new Rectangle(clientRectangle.X + percentRectangle.Width, clientRectangle.Y, clientRectangle.Width - percentRectangle.Width,
            clientRectangle.Height);
      }

      public void OnPaint(Graphics graphics)
      {
         using var percentBrush = new SolidBrush(Color.LightSteelBlue);
         graphics.FillRectangle(percentBrush, percentRectangle);
      }
   }
}
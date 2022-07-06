using System.Drawing;
using System.Drawing.Text;

namespace Core.WinForms.Controls;

public class BusyProcessor
{
   protected int width;
   protected int height;
   protected int x;
   protected int speed;

   public BusyProcessor(Rectangle clientRectangle)
   {
      width = clientRectangle.Width;
      height = clientRectangle.Height;
      x = 0;
      speed = 0;
   }

   public void Advance()
   {
      if (speed++ >= 3)
      {
         if (x < width)
         {
            x += 20;
         }
         else
         {
            x = 0;
         }

         speed = 0;
      }
   }

   public void OnPaint(Graphics graphics)
   {
      graphics.HighQuality();
      graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
      using var whitePen = new Pen(Color.White, 5);
      graphics.DrawLine(whitePen, x + 1, 1, x + 1, height - 1);
      using var greenPen = new Pen(Color.Green, 5);
      graphics.DrawLine(greenPen, x, 0, x, height);
   }
}
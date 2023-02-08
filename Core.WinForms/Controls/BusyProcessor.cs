using System.Drawing;
using System.Drawing.Text;

namespace Core.WinForms.Controls;

public class BusyProcessor
{
   protected int width;
   protected int height;
   protected int x;
   protected int speed;
   protected int currentX;

   public BusyProcessor(Rectangle clientRectangle)
   {
      width = clientRectangle.Width;
      height = clientRectangle.Height;
      x = clientRectangle.X;
      speed = 0;
      currentX = x;
   }

   public void Advance()
   {
      if (speed++ >= 3)
      {
         if (currentX < width)
         {
            currentX += 20;
         }
         else
         {
            currentX = x;
         }

         speed = 0;
      }
   }

   public void OnPaint(Graphics graphics)
   {
      graphics.HighQuality();
      graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
      using var whitePen = new Pen(Color.White, 5);
      graphics.DrawLine(whitePen, currentX + 1, 1, currentX + 1, height - 1);
      using var greenPen = new Pen(Color.Green, 5);
      graphics.DrawLine(greenPen, currentX, 0, currentX, height);
   }
}
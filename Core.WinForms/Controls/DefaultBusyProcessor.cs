using System.Drawing;
using System.Drawing.Text;

namespace Core.WinForms.Controls;

public class DefaultBusyProcessor : BusyProcessor
{
   protected int width;
   protected int height;
   protected int x;
   protected int speed;
   protected int currentX;

   public DefaultBusyProcessor(Rectangle clientRectangle) : base(clientRectangle)
   {
      width = clientRectangle.Width;
      height = clientRectangle.Height;
      x = clientRectangle.X;
      speed = 0;
      currentX = x;
   }

   public override void Advance()
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

   public override void OnPaint(Graphics g)
   {
      g.HighQuality();
      g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
      using var whitePen = new Pen(Color.White, 5);
      g.DrawLine(whitePen, currentX + 1, 1, currentX + 1, height - 1);
      using var greenPen = new Pen(Color.Green, 5);
      g.DrawLine(greenPen, currentX, 0, currentX, height);
   }
}
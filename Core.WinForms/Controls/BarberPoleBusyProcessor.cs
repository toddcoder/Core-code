using System.Drawing;

namespace Core.WinForms.Controls;

public class BarberPoleBusyProcessor : BusyProcessor
{
   protected const int SKEW = 16;
   protected const int INCREMENT = 8;

   protected int width;
   protected int left;
   protected int rightCheck;
   protected int top;
   protected int bottom;

   public BarberPoleBusyProcessor(Rectangle clientRectangle) : base(clientRectangle)
   {
      width = clientRectangle.Width / SKEW;
      left = 0;
      rightCheck = width * 2;
      top = -2 * INCREMENT;
      bottom = clientRectangle.Bottom + 2 * INCREMENT;
   }

   public override void Advance()
   {
      if (left + INCREMENT < rightCheck)
      {
         left += INCREMENT;
      }
      else
      {
         left = 0;
      }
   }

   public override void OnPaint(Graphics g)
   {
      g.HighQuality();

      using var brush = new SolidBrush(Color.LightGreen);
      g.FillRectangle(brush, clientRectangle);

      using var pen = new Pen(Color.Blue, width);
      var actualLeft = left - 2 * width;
      for (var i = 0; i < 10; i++)
      {
         g.DrawLine(pen, actualLeft, bottom, actualLeft + SKEW, top);
         actualLeft += 2 * width;
      }
   }
}
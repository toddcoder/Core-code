using System.Drawing;
using Core.Numbers;

namespace Core.WinForms.Controls;

public class RectangleBusyProcessor : BusyProcessor
{
   protected Rectangle[] rectangles;
   protected int activeIndex;

   public RectangleBusyProcessor(Rectangle clientRectangle) : base(clientRectangle)
   {
      var width = (clientRectangle.Width - 20) / 4;
      var height = clientRectangle.Height - 8;
      var top = clientRectangle.Top + 4;
      rectangles = new Rectangle[4];
      var offset = width + 4;
      for (var i = 0; i < 4; i++)
      {
         rectangles[i] = new Rectangle(4 + i * offset, top, width, height);
      }

      activeIndex = 4.nextRandom();
   }

   public override void Advance()
   {
      if (activeIndex >= 4)
      {
         activeIndex = 0;
      }
      else
      {
         activeIndex++;
      }
   }

   public override void OnPaint(Graphics g)
   {
      for (var i = 0; i < 4; i++)
      {
         if (i == activeIndex)
         {
            g.FillRectangle(Brushes.White, rectangles[i]);
         }
         else
         {
            g.DrawRectangle(Pens.White, rectangles[i]);
         }
      }
   }
}
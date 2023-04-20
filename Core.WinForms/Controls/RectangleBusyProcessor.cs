using System.Drawing;
using Core.Numbers;

namespace Core.WinForms.Controls;

public class RectangleBusyProcessor : BusyProcessor
{
   protected const int NUM_RECTANGLES = 10;
   protected const int RECTANGLE_MARGIN = 4;

   protected Rectangle[] rectangles;
   protected int activeIndex;

   public RectangleBusyProcessor(Rectangle clientRectangle) : base(clientRectangle)
   {
      var width = (clientRectangle.Width - (NUM_RECTANGLES + 1) * RECTANGLE_MARGIN) / NUM_RECTANGLES;
      var height = clientRectangle.Height - 2 * RECTANGLE_MARGIN;
      var top = clientRectangle.Top + RECTANGLE_MARGIN;
      rectangles = new Rectangle[NUM_RECTANGLES];
      var offset = width + RECTANGLE_MARGIN;
      for (var i = 0; i < NUM_RECTANGLES; i++)
      {
         rectangles[i] = new Rectangle(4 + i * offset, top, width, height);
      }

      activeIndex = NUM_RECTANGLES.nextRandom();
   }

   public override void Advance()
   {
      if (activeIndex >= NUM_RECTANGLES)
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
      for (var i = 0; i < NUM_RECTANGLES; i++)
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
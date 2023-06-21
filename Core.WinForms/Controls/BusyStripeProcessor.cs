using System.Drawing;
using System.Drawing.Text;

namespace Core.WinForms.Controls;

public class BusyStripeProcessor : BusyProcessor
{
   protected int width;
   protected int lineWidth;
   protected int height;
   protected int top;
   protected int x;
   protected int speed;
   protected int currentX;
   protected int clickGlyphWidth;
   protected Color color;

   public BusyStripeProcessor(Rectangle clientRectangle, bool clickable, Color color) : base(clientRectangle)
   {
      height = 4;
      x = clientRectangle.X;
      currentX = x;
      speed = 0;
      clickGlyphWidth = clickable ? 4 : 0;
      width = clientRectangle.Width - clickGlyphWidth;
      lineWidth = width / 10;
      top = clientRectangle.Bottom - 4;
      this.color = color;
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

   public bool Enabled { get; set; }

   public override void OnPaint(Graphics g)
   {
      g.HighQuality();
      g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
      using var backPen = new Pen(color, 4);
      if (!Enabled)
      {
         backPen.DashPattern = new[] { 3.0f, 1.0f };
      }

      g.DrawLine(backPen, currentX, top, currentX + lineWidth, top);
   }
}
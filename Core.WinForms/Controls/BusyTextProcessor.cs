using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Core.WinForms.Controls;

public class BusyTextProcessor
{
   protected const double TOTAL_CIRCLE = 360;
   protected const double HALF_CIRCLE = TOTAL_CIRCLE / 2;
   protected const int INNER_RADIUS = 5;
   protected const int OUTER_RADIUS = 11;
   protected const int SPOKE_COUNT = 12;
   protected const int SPOKE_THICKNESS = 2;

   protected PointF center;
   protected Color color;
   protected Color[] colors;
   protected double[] angles;
   protected Rectangle drawRectangle;
   protected Rectangle textRectangle;
   protected int progressValue;

   protected static Color[] generatePalette(Color color)
   {
      static Color darken(Color spokeColor, int percent) => Color.FromArgb(percent, spokeColor.R, spokeColor.G, spokeColor.B);

      var colors = new Color[SPOKE_COUNT];
      var increment = (byte)(byte.MaxValue / SPOKE_COUNT);
      var percentDarkened = 0;

      colors[0] = color;

      for (var i = 1; i < SPOKE_COUNT; i++)
      {
         percentDarkened += increment;
         if (percentDarkened > byte.MaxValue)
         {
            percentDarkened = byte.MaxValue;
         }

         colors[i] = darken(color, percentDarkened);
      }

      return colors;
   }

   public Rectangle TextRectangle => textRectangle;

   protected static double[] generateAngles()
   {
      var angles = new double[SPOKE_COUNT];
      var angle = TOTAL_CIRCLE / SPOKE_COUNT;
      angles[0] = angle;

      for (var i = 1; i < SPOKE_COUNT; i++)
      {
         angles[i] = angles[i - 1] + angle;
      }

      return angles;
   }

   protected static Rectangle getDrawRectangle(Rectangle clientRectangle)
   {
      var side = clientRectangle.Height;
      return clientRectangle with { Width = side, Height = side };
   }

   protected static Rectangle getTextRectangle(Rectangle drawRectangle, Rectangle clientRectangle)
   {
      return clientRectangle with { X = clientRectangle.X + drawRectangle.Width, Width = clientRectangle.Width - drawRectangle.Width };
   }

   protected static PointF getCenter(Rectangle drawRectangle)
   {
      return new PointF(drawRectangle.X + drawRectangle.Height / 2, drawRectangle.Y + drawRectangle.Height / 2);
   }

   public BusyTextProcessor(Color color, Rectangle clientRectangle)
   {
      this.color = color;

      colors = generatePalette(this.color);
      angles = generateAngles();
      drawRectangle = getDrawRectangle(clientRectangle);
      textRectangle = getTextRectangle(drawRectangle, clientRectangle);
      center = getCenter(drawRectangle);
   }

   protected static void drawLine(Graphics graphics, PointF startPoint, PointF endPoint, Color color)
   {
      using var pen = new Pen(color, SPOKE_THICKNESS)
      {
         StartCap = LineCap.Round,
         EndCap = LineCap.Round
      };
      graphics.DrawLine(pen, startPoint, endPoint);
   }

   protected static PointF getCoordinate(PointF center, int radius, double angle)
   {
      var angleInRadians = Math.PI * angle / HALF_CIRCLE;
      return new PointF(center.X + radius * (float)Math.Cos(angleInRadians), center.Y + radius * (float)Math.Sin(angleInRadians));
   }

   public void OnTick() => progressValue = ++progressValue % SPOKE_COUNT;

   public void OnPaint(PaintEventArgs e)
   {
      e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
      var position = progressValue;
      for (var i = 0; i < SPOKE_COUNT; i++)
      {
         position %= SPOKE_COUNT;
         var startPoint = getCoordinate(center, INNER_RADIUS, angles[position]);
         var endPoint = getCoordinate(center, OUTER_RADIUS, angles[position]);
         drawLine(e.Graphics, startPoint, endPoint, colors[i]);
         position++;
      }
   }
}
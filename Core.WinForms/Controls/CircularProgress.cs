using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Core.WinForms.Controls
{
   public partial class CircularProgress : UserControl
   {
      const float MAX_ANGLE = 360.0f;

      float angle;
      int color;
      Rectangle inside;
      Stopwatch stopwatch;

      public event EventHandler<ProgressEventArgs> Tick;

      public CircularProgress()
      {
         InitializeComponent();

         angle = 0;
         color = 0;
         inside = ClientRectangle;
         inside.Inflate(-2, -2);
         stopwatch = new Stopwatch();
      }

      Color getColor()
      {
         switch (color)
         {
            case 0:
               return Color.Red;
            case 1:
               return Color.Blue;
            case 2:
               return Color.Green;
            default:
               return Color.Black;
         }
      }

      void timer_Tick(object sender, EventArgs e)
      {
         angle += 10;
         if (angle > MAX_ANGLE)
         {
            angle = 0;
            color++;
            if (color > 2)
            {
               color = 0;
            }
         }

         using (var g = CreateGraphics())
         {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.Bicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.FillRectangle(Brushes.LightSkyBlue, ClientRectangle);
            using (var pen = new Pen(getColor(), 3f))
            {
               g.CompositingQuality = CompositingQuality.HighQuality;
               g.DrawArc(pen, inside, 0, angle);
            }
         }

         Tick?.Invoke(this, new ProgressEventArgs(stopwatch.Elapsed));
      }

      void CircularProgress_VisibleChanged(object sender, EventArgs e)
      {
         timer.Enabled = Visible;
         if (timer.Enabled)
         {
            stopwatch.Reset();
            stopwatch.Start();
         }
         else
         {
            stopwatch.Stop();
         }
      }
   }
}

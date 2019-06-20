using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Core.WinForms.Controls
{
   public partial class WebProgress : UserControl
   {
      const int NUMBER_OF_CIRCLES = 8;
      const float ANGLE = 360.0f / NUMBER_OF_CIRCLES;
      const int NUMBER_OF_VISIBLE_CIRCLES = 8;
      const int CIRCLE_SIZE = 1;

      int value;
      Stopwatch stopwatch;

      public event EventHandler<ProgressEventArgs> Tick;

      public WebProgress()
      {
         InitializeComponent();

         value = 1;
         stopwatch = new Stopwatch();
      }

      void timer_Tick(object sender, EventArgs e)
      {
         if (Visible)
         {
            Tick?.Invoke(this, new ProgressEventArgs(stopwatch.Elapsed));
            if (value + 1 <= NUMBER_OF_CIRCLES)
            {
               value++;
            }
            else
            {
               value = 1;
            }

            Invalidate();
         }
      }

      void WebProgress_VisibleChanged(object sender, EventArgs e)
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

      void WebProgress_Paint(object sender, PaintEventArgs e)
      {
         var state = e.Graphics.Save();

         e.Graphics.TranslateTransform(Width / 2.0F, Height / 2.0F);
         e.Graphics.RotateTransform(ANGLE * value);
         e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
         e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
         e.Graphics.CompositingMode = CompositingMode.SourceOver;

         for (var i = 1; i <= NUMBER_OF_CIRCLES; i++)
         {
            var alpha = 255.0F * (i / (float)NUMBER_OF_VISIBLE_CIRCLES) > 255.0 ? 0 :
               (int)(255.0F * (i / (float)NUMBER_OF_VISIBLE_CIRCLES));

            var drawColor = Color.FromArgb(alpha, Color);

            using (var brush = new SolidBrush(drawColor))
            {
               var sizeRate = 4.5F / CIRCLE_SIZE;
               var size = Width / sizeRate;

               var diff = Width / 4.5F - size;

               var x = Width / 9.0F + diff;
               var y = Height / 9.0F + diff;
               e.Graphics.FillEllipse(brush, x, y, size, size);
               e.Graphics.RotateTransform(ANGLE);
            }
         }

         e.Graphics.Restore(state);
      }

      public Color Color { get; set; } = Color.Black;
   }
}

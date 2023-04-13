using System;
using System.Drawing;
using System.Drawing.Text;
using Core.Collections;
using static Core.Numbers.NumberExtensions;

namespace Core.WinForms.Controls;

public class SineBusyProcessor : BusyProcessor
{
   protected const double MAXIMUM_VALUE = 720.0;

   protected int width;
   protected int height;
   protected int margin;
   protected double start;
   protected Hash<double, double> sineValues;
   protected Hash<double, int> yValues;
   protected Hash<double, int> xValues;

   public SineBusyProcessor(Rectangle clientRectangle) : base(clientRectangle)
   {
      width = clientRectangle.Width;
      height = clientRectangle.Height;
      margin = 2;

      start = 0.nextRandom(740, 20);
      sineValues = new Hash<double, double>();
      yValues = new Hash<double, int>();
      xValues = new Hash<double, int>();
   }

   public override void Advance()
   {
      if (start < MAXIMUM_VALUE)
      {
         start += 20;
      }
      else
      {
         start = 0;
      }
   }

   public override void OnPaint(Graphics graphics)
   {
      void draw(double i)
      {
         var value = sineValues.Memoize(i, d => Math.Sin(d * Math.PI / 180) + 1);
         var y = yValues.Memoize(value, d => (int)(d / 2.0 * height + margin / 2));
         var x = xValues.Memoize(i - start, d => (int)(d / MAXIMUM_VALUE * width + margin / 2));
         graphics.SetPixel(x, y, Color.White);
      }

      graphics.HighQuality();
      graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

      var right = MAXIMUM_VALUE + start;
      for (var i = start; i <= right; i += 0.1)
      {
         draw(i);
      }
   }
}
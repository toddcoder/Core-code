using System.Drawing;
using System.Drawing.Drawing2D;

namespace Core.WinForms;

public static class GraphicsExtensions
{
   public static GraphicsPath Rounded(this Rectangle rectangle, int radius)
   {
      var diameter = radius * 2;
      var size = new Size(diameter, diameter);
      var arc = new Rectangle(rectangle.Location, size);
      var path = new GraphicsPath();

      if (radius == 0)
      {
         path.AddRectangle(rectangle);
         return path;
      }

      path.AddArc(arc, 180, 90);

      arc.X = rectangle.Right - diameter;
      path.AddArc(arc, 270, 90);

      arc.Y = rectangle.Bottom - diameter;
      path.AddArc(arc, 0, 90);

      arc.X = rectangle.Left;
      path.AddArc(arc, 90, 90);

      path.CloseFigure();
      return path;
   }

   public static void HighQuality(this Graphics graphics)
   {
      graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      graphics.SmoothingMode = SmoothingMode.AntiAlias;
   }

   public static void SetPixel(this Graphics graphics, int x, int y, Color color)
   {
      using var bitmap = new Bitmap(1, 1);
      bitmap.SetPixel(0, 0, color);
      graphics.DrawImageUnscaled(bitmap, x, y);
   }

   public static Size Resize(this Size size, int widthAmount, int heightAmount) => new(size.Width + widthAmount, size.Height + heightAmount);

   public static Rectangle Resize(this Rectangle rectangle, int widthAmount, int heightAmount)
   {
      return rectangle with { Size = rectangle.Size.Resize(widthAmount, heightAmount) };
   }

   public static Rectangle Reposition(this Rectangle rectangle, int xOffset, int yOffset)
   {
      return rectangle with { Location = rectangle.Location, X = rectangle.Left + xOffset, Y = rectangle.Top + yOffset };
   }
}
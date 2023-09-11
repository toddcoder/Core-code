﻿using System.Drawing;
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

   public static Size OffsetWidth(this Size size, int amount) => size with { Width = size.Width + amount };

   public static Size OffsetHeight(this Size size, int amount) => size with { Height = size.Height + amount };

   public static Rectangle Resize(this Rectangle rectangle, int widthAmount, int heightAmount)
   {
      return rectangle with { Size = rectangle.Size.Resize(widthAmount, heightAmount) };
   }

   public static Rectangle OffsetWidth(this Rectangle rectangle, int amount) => rectangle with { Size = OffsetWidth(rectangle.Size, amount) };

   public static Rectangle OffsetHeight(this Rectangle rectangle, int amount) => rectangle with { Size = OffsetHeight(rectangle.Size, amount) };

   public static Rectangle Reposition(this Rectangle rectangle, int xOffset, int yOffset)
   {
      return rectangle with { Location = rectangle.Location, X = rectangle.Left + xOffset, Y = rectangle.Top + yOffset };
   }

   public static Point Reposition(this Point point, int xOffset, int yOffset) => new(point.X + xOffset, point.Y + yOffset);

   public static Point OffsetX(this Point point, int xOffset) => point with { X = point.X + xOffset };

   public static Rectangle OffsetX(this Rectangle rectangle, int xOffset) => rectangle with { Location = rectangle.Location.OffsetX(xOffset) };

   public static Rectangle OffsetY(this Rectangle rectangle, int yOffset) => rectangle with { Location = rectangle.Location.OffsetY(yOffset) };

   public static Point OffsetY(this Point point, int yOffset) => point with { Y = point.Y + yOffset };

   public static Point North(this Rectangle rectangle, int xOffset = 0) => new(rectangle.Left + rectangle.Width / 2, rectangle.Top + xOffset);

   public static Point North(this Rectangle rectangle, int xOffset, int yOffset) =>
      new(rectangle.Left + rectangle.Width / 2 + xOffset, rectangle.Top + yOffset);

   public static Point NorthEast(this Rectangle rectangle, int xOffset = 0) => new(rectangle.Right - xOffset, rectangle.Top + xOffset);

   public static Point NorthEast(this Rectangle rectangle, int xOffset, int yOffset) => new(rectangle.Right + xOffset, rectangle.Top + yOffset);

   public static Point East(this Rectangle rectangle, int xOffset = 0) => new(rectangle.Right - xOffset, rectangle.Top + rectangle.Height / 2);

   public static Point East(this Rectangle rectangle, int xOffset, int yOffset) =>
      new(rectangle.Right + xOffset, rectangle.Top + rectangle.Height / 2 + yOffset);

   public static Point SouthEast(this Rectangle rectangle, int xOffset = 0) => new(rectangle.Right - xOffset, rectangle.Bottom - xOffset);

   public static Point SouthEast(this Rectangle rectangle, int xOffset, int yOffset) => new(rectangle.Right + xOffset, rectangle.Bottom + yOffset);

   public static Point South(this Rectangle rectangle, int xOffset = 0) => new(rectangle.Left + rectangle.Width / 2, rectangle.Bottom - xOffset);

   public static Point South(this Rectangle rectangle, int xOffset, int yOffset)
   {
      return new Point(rectangle.Left + rectangle.Width / 2 + xOffset, rectangle.Bottom + yOffset);
   }

   public static Point SouthWest(this Rectangle rectangle, int xOffset = 0) => new(rectangle.Left + xOffset, rectangle.Bottom - xOffset);

   public static Point SouthWest(this Rectangle rectangle, int xOffset, int yOffset) => new(rectangle.Left + xOffset, rectangle.Bottom + yOffset);

   public static Point West(this Rectangle rectangle, int xOffset = 0) => new(rectangle.Left + xOffset, rectangle.Top + rectangle.Height / 2);

   public static Point West(this Rectangle rectangle, int xOffset, int yOffset)
   {
      return new Point(rectangle.Left + xOffset, rectangle.Top + rectangle.Height / 2 + yOffset);
   }

   public static Point NorthWest(this Rectangle rectangle, int xOffset = 0) => new(rectangle.Left + xOffset, rectangle.Top + xOffset);

   public static Point NorthWest(this Rectangle rectangle, int xOffset, int yOffset) => new(rectangle.Left + xOffset, rectangle.Top + yOffset);

   public static Rectangle Shrink(this Rectangle rectangle, int offset = 0)
   {
      return rectangle.Reposition(offset, offset).Resize(-2 * offset, -2 * offset);
   }

   public static Rectangle Expand(this Rectangle rectangle, int offset = 0)
   {
      return rectangle.Reposition(-offset, -offset).Resize(2 * offset, 2 * offset);
   }

   public static void FillRectangle(this Graphics g, Color color, Rectangle rectangle)
   {
      using var brush = new SolidBrush(color);
      g.FillRectangle(brush, rectangle);
   }

   public static void DrawRectangle(this Graphics g, Color color, Rectangle rectangle, DashStyle dashStyle = DashStyle.Solid, float width = 1f)
   {
      using var pen = new Pen(color, width);
      pen.DashStyle = dashStyle;
      g.DrawRectangle(pen, rectangle);
   }
}
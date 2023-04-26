using System.Drawing;

namespace Core.WinForms;

public static class RectangleExtensions
{
   public static RectangleF ToRectangleF(this Rectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

   public static Rectangle ToRectangle(this RectangleF rectangle)
   {
      return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
   }
}
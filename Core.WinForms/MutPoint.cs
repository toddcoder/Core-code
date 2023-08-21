using System.Drawing;

namespace Core.WinForms;

public class MutPoint
{
   public static implicit operator MutPoint(Point point) => new(point);

   public static implicit operator MutPoint((int x, int y) tuple) => new(tuple.x, tuple.y);

   public static implicit operator Point(MutPoint mutPoint) => mutPoint.toPoint();

   public static MutRectangle operator +(MutPoint location, MutSize size) => new Rectangle(location, size);

   protected int x;
   protected int y;

   protected MutPoint(Point point)
   {
      x = point.X;
      y = point.Y;
   }

   protected MutPoint(int x, int y)
   {
      this.x = x;
      this.y = y;
   }

   public int X
   {
      get => x;
      set => x = value;
   }

   public int Y
   {
      get => y;
      set => y = value;
   }

   protected Point toPoint() => new(x, y);
}
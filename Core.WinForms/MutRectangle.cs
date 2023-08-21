using System.Drawing;

namespace Core.WinForms;

public class MutRectangle
{
   public static implicit operator MutRectangle(Rectangle rectangle) => new(rectangle);

   public static implicit operator Rectangle(MutRectangle mutRectangle) => mutRectangle.toRectangle();

   public static implicit operator MutRectangle((int x, int y, int width, int height) tuple) => new(tuple.x, tuple.y, tuple.width, tuple.height);

   protected int x;
   protected int y;
   protected int width;
   protected int height;

   protected MutRectangle(Rectangle rectangle)
   {
      x = rectangle.X;
      y = rectangle.Y;
      width = rectangle.Width;
      height = rectangle.Height;
   }

   protected MutRectangle(int x, int y, int width, int height)
   {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
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

   public int Width
   {
      get => width;
      set => width = value;
   }

   public int Height
   {
      get => height;
      set => height = value;
   }

   protected Rectangle toRectangle() => new(x, y, width, height);
}
using System.Drawing;

namespace Core.WinForms;

public class MutSize
{
   public static implicit operator MutSize(Size size) => new(size);

   public static implicit operator Size(MutSize mutSize) => mutSize.toSize();

   public static implicit operator MutSize((int width, int height) tuple) => new(tuple.width, tuple.height);

   public static MutRectangle operator +(MutSize size, MutPoint location) => new Rectangle(location, size);

   protected int width;
   protected int height;

   protected MutSize(Size size)
   {
      width = size.Width;
      height = size.Height;
   }

   protected MutSize(int width, int height)
   {
      this.width = width;
      this.height = height;
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

   protected Size toSize() => new(width, height);
}
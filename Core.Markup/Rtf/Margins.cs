using Core.Assertions;

namespace Core.Markup.Rtf;

public class Margins
{
   protected float[] margins;

   public Margins()
   {
      margins = new float[4];
   }

   public Margins(float top, float right, float bottom, float left) : this()
   {
      margins[(int)Direction.Top] = top;
      margins[(int)Direction.Right] = right;
      margins[(int)Direction.Bottom] = bottom;
      margins[(int)Direction.Left] = left;
   }

   public float this[Direction direction]
   {
      get
      {
         var index = (int)direction;
         index.Must().BeBetween(0).Until(margins.Length).OrThrow("Not a valid direction.");

         return margins[index];
      }
      set
      {
         var index = (int)direction;
         index.Must().BeBetween(0).Until(margins.Length).OrThrow("Not a valid direction.");

         margins[index] = value;
      }
   }
}
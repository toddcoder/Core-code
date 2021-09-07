using System;
using Core.Assertions;

namespace Core.Markup.Rtf
{
   public class Margins : IEquatable<Margins>
   {
      public static bool operator ==(Margins left, Margins right) => left.Equals(right);

      public static bool operator !=(Margins left, Margins right) => !left.Equals(right);

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

      public bool Equals(Margins margins)
      {
         return margins.margins[(int)Direction.Bottom] == this.margins[(int)Direction.Bottom] &&
            margins.margins[(int)Direction.Left] == this.margins[(int)Direction.Left] &&
            margins.margins[(int)Direction.Right] == this.margins[(int)Direction.Right] &&
            margins.margins[(int)Direction.Top] == this.margins[(int)Direction.Top];
      }

      public override bool Equals(object obj) => obj is Margins otherMargins && Equals(otherMargins);

      public override int GetHashCode() => margins.GetHashCode();
   }
}
using System.Linq;
using Core.Assertions;

namespace Core.Markup.Rtf
{
   public class Borders
   {
      protected Border[] borders;

      public Borders()
      {
         borders = Enumerable.Range(0, 4).Select(_ => new Border()).ToArray();
      }

      public Border this[Direction direction]
      {
         get
         {
            var index = (int)direction;
            index.Must().BeBetween(0).Until(borders.Length).OrThrow("Not a valid direction.");

            return borders[index];
         }
      }
   }
}
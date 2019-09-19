using System.Collections.Generic;
using System.Linq;
using Core.Assertions;

namespace Core.Enumerables
{
   public static class TupleExtensions
   {
      public static (T, T) ToTuple2<T>(this IEnumerable<T> enumerable)
      {
         var list = enumerable.ToList().Must().HaveCountOf(2).Ensure("Enumerable too small");

         return (list[0], list[1]);
      }

      public static (T, T, T) ToTuple3<T>(this IEnumerable<T> enumerable)
      {
         var list = enumerable.ToList().Must().HaveCountOf(3).Ensure("Enumerable too small");

         return (list[0], list[1], list[2]);
      }

      public static (T, T, T, T) ToTuple4<T>(this IEnumerable<T> enumerable)
      {
         var list = enumerable.ToList().Must().HaveCountOf(4).Ensure("Enumerable too small");

         return (list[0], list[1], list[2], list[3]);
      }
   }
}
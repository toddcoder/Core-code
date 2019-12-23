using System.Collections.Generic;
using System.Linq;
using Core.Assertions;
using static Core.Assertions.AssertionFunctions;

namespace Core.Enumerables
{
   public static class TupleExtensions
   {
      public static (T, T) ToTuple2<T>(this IEnumerable<T> enumerable)
      {
         var list = enumerable.ToList();
         assert(() => list).Must().HaveCountOf(2).OrThrow();

         return (list[0], list[1]);
      }

      public static (T, T, T) ToTuple3<T>(this IEnumerable<T> enumerable)
      {
         var list = enumerable.ToList();
         assert(() => list).Must().HaveCountOf(3).OrThrow();

         return (list[0], list[1], list[2]);
      }

      public static (T, T, T, T) ToTuple4<T>(this IEnumerable<T> enumerable)
      {
         var list = enumerable.ToList();
         assert(() => list).Must().HaveCountOf(4).OrThrow();

         return (list[0], list[1], list[2], list[3]);
      }
   }
}
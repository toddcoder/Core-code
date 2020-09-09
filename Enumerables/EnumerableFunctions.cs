using System;
using System.Collections.Generic;
using Core.Assertions;
using static Core.Assertions.AssertionFunctions;

namespace Core.Enumerables
{
   public static class EnumerableFunctions
   {
      public static IEnumerable<T> enumEnumerable<T>() where T : Enum
      {
         var type = typeof(T);

         assert(() => type).Must().BeEnumeration().OrThrow(() => "Type $name must be an enum");

         foreach (var value in (T[])Enum.GetValues(type))
         {
            yield return value;
         }
      }
   }
}
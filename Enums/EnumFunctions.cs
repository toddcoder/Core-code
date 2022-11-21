using System;
using System.Collections.Generic;
using Core.Assertions;

namespace Core.Enums;

public static class EnumFunctions
{
   public static IEnumerable<T> enumEnumerable<T>() where T : Enum
   {
      var type = typeof(T);

      type.Must().BeEnumeration().OrThrow(() => $"Type {type.Name} must be an enum");

      foreach (var value in (T[])Enum.GetValues(type))
      {
         yield return value;
      }
   }
}
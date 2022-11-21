using System;
using System.Collections.Generic;
using Core.Assertions;

namespace Core.Lists;

public static class ListExtensions
{
   public static bool AllEqualTo<T>(this List<T> left, List<T> right) where T : IEquatable<T>
   {
      left.Must().Not.BeNullOrEmpty().OrThrow();
      right.Must().Not.BeNullOrEmpty().OrThrow();

      if (left.Count != right.Count)
      {
         return false;
      }
      else
      {
         for (var i = 0; i < left.Count; i++)
         {
            if (!left[i].Equals(right[i]))
            {
               return false;
            }
         }

         return true;
      }
   }
}
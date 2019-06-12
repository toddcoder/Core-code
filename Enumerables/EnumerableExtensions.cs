using System;
using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.AttemptFunctions;

namespace Core.Enumerables
{
   public static class EnumerableExtensions
   {
      public static string Stringify<T>(this IEnumerable<T> enumerable, string connector = ", ")
      {
	      return string.Join(connector ?? "", enumerable);
      }

      public static IEnumerable<IEnumerable<T>> Pivot<T>(this IEnumerable<IEnumerable<T>> source, Func<T> defaultValue)
      {
         var array = source.Select(row => row.ToArray()).ToArray();
         if (array.Length != 0)
         {
            var maxRowLen = array.Select(a => a.Length).Max();
            var minRowLen = array.Select(a => a.Length).Min();
            var squared = array;
            if (minRowLen != maxRowLen)
               squared = array.Select(row => row.Pad(maxRowLen, defaultValue())).ToArray();

            return 0.Until(maxRowLen).Select(i => squared.Select(row => row[i]).ToArray());
         }
         else
            return array;
      }

      public static IEnumerable<IEnumerable<T>> Pivot<T>(this IEnumerable<IEnumerable<T>> source) => source.Pivot(() => default);

      public static IResult<T[]> ToResultOfArray<T>(this IEnumerable<T> enumerable) => tryTo(enumerable.ToArray);

      public static IResult<List<T>> ToResultOfList<T>(this IEnumerable<T> enumerable) => tryTo(enumerable.ToList);

      public static IEnumerable<int> UpTo(this int from, int to, int by = 1)
      {
         for (var i = from; i <= to; i += by)
            yield return i;
      }

      public static IEnumerable<int> UpUntil(this int from, int to, int by = 1)
      {
         for (var i = from; i < to; i += by)
            yield return i;
      }

      public static IEnumerable<int> DownTo(this int from, int to, int by = -1)
      {
         for (var i = from; i >= to; i += by)
            yield return i;
      }

      public static IEnumerable<int> DownUntil(this int from, int to, int by = -1)
      {
         for (var i = from; i > to; i += by)
            yield return i;
      }

      public static IEnumerable<T> Then<T>(this T seed, Func<T, T> next, Predicate<T> stop)
      {
         var current = seed;

         yield return current;

         while (!stop(current))
         {
            current = next(current);
            yield return current;
         }
      }

      public static IEnumerable<T> Then<T>(this T seed, Func<T, T> next) => seed.Then(next, v => false);

	   public static IEnumerable<(int, T)> Indexed<T>(this IEnumerable<T> enumerable)
	   {
		   var index = 0;
		   foreach (var item in enumerable)
			   yield return (index++, item);
	   }
   }
}
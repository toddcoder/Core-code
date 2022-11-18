using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Data;

public static class DataExtensions
{
   public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
   {
      return items.GroupBy(property).Select(x => x.First());
   }

   public static ParallelQuery<T> DistinctBy<T, TKey>(this ParallelQuery<T> items, Func<T, TKey> property)
   {
      return items.GroupBy(property).Select(x => x.First());
   }
}
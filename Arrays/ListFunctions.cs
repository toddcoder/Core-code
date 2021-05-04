using System.Collections.Generic;

namespace Core.Arrays
{
   public static class ListFunctions
   {
      public static List<T> list<T>() => new();

      public static List<T> list<T>(params T[] items)
      {
         var list = new List<T>();
         list.AddRange(items);

         return list;
      }

      public static List<T> list<T>(List<T> items, params T[] newItems)
      {
         var list = new List<T>();
         list.AddRange(items);
         list.AddRange(newItems);

         return list;
      }
   }
}
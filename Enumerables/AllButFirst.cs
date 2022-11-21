using System;
using System.Collections.Generic;

namespace Core.Enumerables;

public class AllButFirst<T>
{
   protected IEnumerable<T> enumerable;

   public AllButFirst(IEnumerable<T> enumerable) => this.enumerable = enumerable;

   public void For(Action<T> firstAction, Action<T> restAction)
   {
      var first = true;

      foreach (var item in enumerable)
      {
         if (first)
         {
            firstAction(item);
            first = false;
         }
         else
         {
            restAction(item);
         }
      }
   }
}
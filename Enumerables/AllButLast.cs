using System;
using System.Collections.Generic;

namespace Core.Enumerables;

public class AllButLast<T>
{
   protected IEnumerable<T> enumerable;

   public AllButLast(IEnumerable<T> enumerable) => this.enumerable = enumerable;

   public void For(Action<T> restAction, Action<T> lastAction)
   {
      using var enumerator = enumerable.GetEnumerator();
      var last = !enumerator.MoveNext();

      while (!last)
      {
         var current = enumerator.Current;
         last = !enumerator.MoveNext();
         if (last)
         {
            lastAction(current);
         }
         else
         {
            restAction(current);
         }
      }
   }
}
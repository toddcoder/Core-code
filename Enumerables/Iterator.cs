using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Enumerables;

public class Iterator<T> : IEnumerable<T>
{
   protected T seed;
   protected Func<T, T> next;
   protected Predicate<T> condition;

   public Iterator(T seed)
   {
      this.seed = seed;

      next = i => i;
      condition = _ => false;
   }

   public Iterator<T> By(Func<T, T> next)
   {
      this.next = next;
      return this;
   }

   public Iterator<T> While(Predicate<T> condition)
   {
      this.condition = condition;
      return this;
   }

   public IEnumerator<T> GetEnumerator()
   {
      var current = seed;
      yield return current;

      while (condition(current))
      {
         current = next(current);
         yield return current;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
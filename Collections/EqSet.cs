using System;
using System.Collections.Generic;

namespace Core.Collections
{
   public class EqSet<T> : Set<T>
   {
      protected Func<T, T, bool> equals;

      public EqSet(Func<T, T, bool> equals)
      {
         this.equals = equals;
      }

      public EqSet(IEnumerable<T> items, Func<T, T, bool> equals) : base(items)
      {
         this.equals = equals;
      }

      public EqSet(Set<T> other, Func<T, T, bool> equals) : base(other)
      {
         this.equals = equals;
      }

      public override bool Contains(T item)
      {
         for (var i = 0; i < Count; i++)
         {
            if (equals(content[i], item))
            {
               return true;
            }
         }

         return false;
      }
   }
}
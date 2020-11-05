using System.Collections.Generic;
using Core.Monads;

namespace Core.Collections
{
   public class Exister<T>
   {
      protected Hash<T, Unit> content;

      public Exister()
      {
         content = new Hash<T, Unit>();
      }

      public Exister(IEnumerable<T> enumerable) : this()
      {
         foreach (var item in enumerable)
         {
            Add(item);
         }
      }

      public Exister(params T[] args) : this()
      {
         foreach (var arg in args)
         {
            Add(arg);
         }
      }

      public void Add(T item) => content[item] = Unit.Value;

      public bool Contains(T item) => content.ContainsKey(item);
   }
}
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class Exister<T>
{
   protected Hash<T, Unit> content;

   public Exister()
   {
      content = new Hash<T, Unit>();
   }

   public Exister(IEqualityComparer<T> comparer)
   {
      content = new Hash<T, Unit>(comparer);
   }

   public Exister(IEnumerable<T> enumerable) : this()
   {
      foreach (var item in enumerable)
      {
         Add(item);
      }
   }

   public Exister(IEnumerable<T> enumerable, IEqualityComparer<T> comparer) : this(comparer)
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

   public Exister(IEqualityComparer<T> comparer, params T[] args) : this(comparer)
   {
      foreach (var arg in args)
      {
         Add(arg);
      }
   }

   public void Add(T item) => content[item] = unit;

   public bool Contains(T item) => content.ContainsKey(item);
}
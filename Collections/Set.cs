using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections
{
   public class Set<T> : IEnumerable<T>
   {
      public static Set<T> operator |(Set<T> set1, IEnumerable<T> set2) => set1.Union(set2);

      public static Set<T> operator &(Set<T> set1, IEnumerable<T> set2) => set1.Intersection(set2);

      public static Set<T> operator ^(Set<T> set1, IEnumerable<T> set2) => set1.Complement(set2);

      public static Set<T> operator -(Set<T> set1, IEnumerable<T> set2) => set1.Except(set2);

      protected HashSet<T> content;
      protected IMaybe<IEqualityComparer<T>> _equalityComparer;

      public Set()
      {
         content = new HashSet<T>();
         _equalityComparer = none<IEqualityComparer<T>>();
      }

      public Set(IEqualityComparer<T> equalityComparer)
      {
         content = new HashSet<T>(equalityComparer);
         _equalityComparer = equalityComparer.Some();
      }

      public Set(IEnumerable<T> items)
      {
         content = new HashSet<T>(items);
         _equalityComparer = none<IEqualityComparer<T>>();
      }

      public Set(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
      {
         content = new HashSet<T>(items, equalityComparer);
         _equalityComparer = equalityComparer.Some();
      }

      public Set(Set<T> other)
      {
         content = new HashSet<T>(other);
         _equalityComparer = none<IEqualityComparer<T>>();
      }

      public Set(Set<T> other, IEqualityComparer<T> equalityComparer)
      {
         content = new HashSet<T>(other, equalityComparer);
         _equalityComparer = equalityComparer.Some();
      }

      public Set(params T[] items)
      {
         content = new HashSet<T>(items);
         _equalityComparer = none<IEqualityComparer<T>>();
      }

      public int Count => content.Count;

      public virtual void Add(T item) => content.Add(item);

      public void AddRange(IEnumerable<T> enumerable)
      {
         foreach (var item in enumerable)
         {
            Add(item);
         }
      }

      public virtual void Remove(T item) => content.Remove(item);

      public virtual void Clear() => content.Clear();

      public virtual bool Contains(T item) => content.Contains(item);

      public Set<T> Clone() => _equalityComparer.Map(ec => new Set<T>(this, ec)).DefaultTo(() => new Set<T>(this));

      public Set<T> Union(IEnumerable<T> enumerable)
      {
         var clone = Clone();
         clone.content.UnionWith(enumerable);

         return clone;
      }

      public Set<T> Intersection(IEnumerable<T> enumerable)
      {
         var clone = Clone();
         clone.content.IntersectWith(enumerable);

         return clone;
      }

      public Set<T> Except(IEnumerable<T> enumerable, bool symmetric = false)
      {
         var clone = Clone();
         if (symmetric)
         {
            clone.content.SymmetricExceptWith(enumerable);
         }
         else
         {
            clone.content.ExceptWith(enumerable);
         }

         return clone;
      }

      public Set<T> Complement(IEnumerable<T> set)
      {
         var clone = Clone();

         foreach (var item in this.Where(i => !set.Contains(i)))
         {
            clone.Add(item);
         }

         return clone;
      }

      public bool Overlaps(IEnumerable<T> set) => content.Overlaps(set);

      public bool IsSubsetOf(IEnumerable<T> set) => content.IsSubsetOf(set);

      public bool IsProperSubsetOf(IEnumerable<T> set) => content.IsProperSubsetOf(set);

      public bool IsSupersetOf(IEnumerable<T> set) => content.IsSupersetOf(set);

      public bool IsProperSupersetOf(IEnumerable<T> set) => content.IsProperSupersetOf(set);

      public bool EqualsEnumerable(IEnumerable<T> set) => content.SetEquals(set);

      IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)content).GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => content.GetEnumerator();

      public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)content).GetEnumerator();
   }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections
{
   public class Set<T> : IEnumerable<T>, IEquatable<Set<T>>
   {
      public static Set<T> Of(params T[] items) => new Set<T>(items);

      public static Set<T> Of(IEnumerable<T> items) => new Set<T>(items);

      public static Set<T> operator |(Set<T> set1, IEnumerable<T> set2) => set1.Union(set2);

      public static Set<T> operator &(Set<T> set1, IEnumerable<T> set2) => set1.Intersection(set2);

      public static Set<T> operator -(Set<T> set1, IEnumerable<T> set2) => set1.Except(set2);

      public static Set<T> operator ^(Set<T> set1, IEnumerable<T> set2) => set1.Except(set2, true);

      protected HashSet<T> content;
      protected Maybe<IEqualityComparer<T>> _equalityComparer;

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

      public Set<T> Clone() => _equalityComparer.Map(ec => new Set<T>(content.ToArray(), ec)).DefaultTo(() => new Set<T>(content.ToArray()));

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

         if (set is Set<T> otherSet)
         {
            foreach (var item in this.Where(i => !otherSet.Contains(i)))
            {
               clone.Add(item);
            }
         }
         else
         {
            foreach (var item in this.Where(i => !set.Contains(i)))
            {
               clone.Add(item);
            }
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

      public bool Equals(Set<T> other)
      {
         if (other is null)
         {
            return false;
         }

         if (ReferenceEquals(this, other))
         {
            return true;
         }

         return content.SetEquals(other.content) && Equals(_equalityComparer, other._equalityComparer);
      }

      public override bool Equals(object obj) => obj is Set<T> set && Equals(set);

      public override int GetHashCode()
      {
         unchecked
         {
            return (content != null ? content.GetHashCode() : 0) * 397 ^ (_equalityComparer != null ? _equalityComparer.GetHashCode() : 0);
         }
      }

      public HashSet<T> ToHashSet() => new HashSet<T>(this);
   }
}
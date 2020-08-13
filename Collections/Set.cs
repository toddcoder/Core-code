using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Monads;

namespace Core.Collections
{
   public class Set<T> : IEnumerable<T>
   {
      public static Set<T> operator |(Set<T> set1, Set<T> set2) => set1.Union(set2);

      public static Set<T> operator &(Set<T> set1, Set<T> set2) => set1.Intersection(set2);

      public static Set<T> operator ^(Set<T> set1, Set<T> set2) => set1.Complement(set2);

      protected List<T> content;

      public Set() => content = new List<T>();

      public Set(IEnumerable<T> items) : this()
      {
         foreach (var item in items)
         {
            add(item);
         }
      }

      public Set(Set<T> other) : this()
      {
         foreach (var item in other)
         {
            add(item);
         }
      }

      public Set(params T[] items) : this()
      {
         foreach (var item in items)
         {
            add(item);
         }
      }

      public T this[int index] => content[index];

      public int Count => content.Count;

      public virtual void Add(T item) => add(item);

      protected void add(T item)
      {
         if (!content.Contains(item))
         {
            content.Add(item);
         }
      }

      public void AddRange(IEnumerable<T> enumerable)
      {
         foreach (var item in enumerable)
         {
            Add(item);
         }
      }

      public virtual void Remove(T item)
      {
         if (content.Contains(item))
         {
            content.Remove(item);
         }
      }

      public virtual void Clear() => content.Clear();

      public virtual bool Contains(T item) => content.Contains(item);

      public virtual IMaybe<T> Find(Predicate<T> predicate) => content.FirstOrNone(i => predicate(i));

      public virtual IEnumerable<T> FindAll(Predicate<T> predicate) => content.Where(i => predicate(i));

      public int IndexOf(T item) => content.IndexOf(item);

      public Set<T> Clone()
      {
         var set = new Set<T>();

         foreach (var item in this)
         {
            set.Add(item);
         }

         return set;
      }

      public Set<T> Union(Set<T> set)
      {
         var copy = Clone();

         foreach (var item in set)
         {
            copy.Add(item);
         }

         return copy;
      }

      public Set<T> Intersection(Set<T> set)
      {
         var newSet = new Set<T>();

         foreach (var item in this.Where(set.Contains))
         {
            newSet.Add(item);
         }

         foreach (var item in set.Where(Contains))
         {
            newSet.Add(item);
         }

         return newSet;
      }

      public Set<T> Complement(Set<T> set)
      {
         var newSet = new Set<T>();

         foreach (var item in this.Where(i => !set.Contains(i)))
         {
            newSet.Add(item);
         }

         return newSet;
      }

      public bool IsSubsetOf(Set<T> set) => this.All(set.Contains);

      public bool IsStrictSubsetOf(Set<T> set) => Count != set.Count && IsSubsetOf(set);

      IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)content).GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => content.GetEnumerator();

      public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)content).GetEnumerator();
   }
}
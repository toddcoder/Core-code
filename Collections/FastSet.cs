using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.Collections;

[Obsolete("Use Set<T>")]
public class FastSet<T> : IEnumerable<T>
{
   public static FastSet<T> operator |(FastSet<T> set1, FastSet<T> set2) => set1.Union(set2);

   public static FastSet<T> operator &(FastSet<T> set1, FastSet<T> set2) => set1.Intersection(set2);

   public static FastSet<T> operator ^(FastSet<T> set1, FastSet<T> set2) => set1.Complement(set2);

   protected Hash<T, int> content;

   public FastSet() => content = new Hash<T, int>();

   public FastSet(IEqualityComparer<T> comparer) => content = new Hash<T, int>(comparer);

   public FastSet(IEnumerable<T> items) : this()
   {
      AddRange(items);
   }

   public FastSet(IEqualityComparer<T> comparer, IEnumerable<T> items) : this(comparer)
   {
      AddRange(items);
   }

   public FastSet(params T[] items) : this()
   {
      AddRange(items);
   }

   public FastSet(IEqualityComparer<T> comparer, params T[] items) : this(comparer)
   {
      AddRange(items);
   }

   public virtual bool Contains(T item) => content.ContainsKey(item);

   public virtual void Add(T item) => add(item);

   public void AddRange(IEnumerable<T> enumerable)
   {
      foreach (var item in enumerable)
      {
         Add(item);
      }
   }

   public virtual void Remove(T item)
   {
      if (Contains(item))
      {
         content.Remove(item);
      }
   }

   protected void add(T item)
   {
      if (!Contains(item))
      {
         content[item] = content.Count;
      }
   }

   public int Count => content.Count;

   public void Clear() => content.Clear();

   public FastSet<T> Union(FastSet<T> set)
   {
      var copy = new FastSet<T>(this);
      copy.AddRange(set);

      return copy;
   }

   public FastSet<T> Intersection(FastSet<T> set)
   {
      var newSet = new FastSet<T>();
      newSet.AddRange(this.Where(set.Contains));
      newSet.AddRange(set.Where(Contains));

      return newSet;
   }

   public FastSet<T> Complement(FastSet<T> set) => new(this.Where(i => !set.Contains(i)));

   public bool IsSubsetOf(FastSet<T> set) => this.All(set.Contains);

   public bool IsStrictSubsetOf(FastSet<T> set) => Count != set.Count && IsSubsetOf(set);

   public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)content.Keys).GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
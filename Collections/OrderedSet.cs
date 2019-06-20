using System.Collections;
using System.Collections.Generic;

namespace Core.Collections
{
   public class OrderedSet<T> : ICollection<T>
   {
      Hash<T, LinkedListNode<T>> hash;
      LinkedList<T> list;

      public OrderedSet(IEqualityComparer<T> comparer)
      {
         hash = new Hash<T, LinkedListNode<T>>(comparer);
         list = new LinkedList<T>();
      }

      public OrderedSet() : this(EqualityComparer<T>.Default) { }

      public OrderedSet(IEnumerable<T> enumerable, IEqualityComparer<T> comparer) : this(comparer)
      {
         foreach (var item in enumerable)
         {
            Add(item);
         }
      }

      public OrderedSet(IEnumerable<T> enumerable) : this(enumerable, EqualityComparer<T>.Default) { }

      public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public void Add(T item)
      {
         if (!hash.ContainsKey(item))
         {
            var node = list.AddLast(item);
            hash[item] = node;
         }
      }

      public void Clear()
      {
         list.Clear();
         hash.Clear();
      }

      public bool Contains(T item) => hash.ContainsKey(item);

      public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

      public bool Remove(T item)
      {
         if (hash.If(item, out var node))
         {
            hash.Remove(item);
            list.Remove(node.Value);

            return true;
         }
         else
         {
            return false;
         }
      }

      public int Count => hash.Count;

      public bool IsReadOnly => false;
   }
}
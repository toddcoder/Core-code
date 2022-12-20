using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Enumerables;

public class LazyList<T> : IList<T>
{
   protected List<IEnumerable<T>> enumerables;
   protected Maybe<T[]> _flattened;

   public event EventHandler Flattened;
   public event EventHandler Unflattened;

   public LazyList()
   {
      enumerables = new List<IEnumerable<T>>();
      _flattened = nil;
   }

   public void Flatten()
   {
      if (!_flattened)
      {
         _flattened = enumerables.Flatten().ToArray();
         Flattened?.Invoke(this, EventArgs.Empty);
      }
   }

   public IEnumerator<T> GetEnumerator()
   {
      Flatten();
      foreach (var item in ~_flattened)
      {
         yield return item;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Add(T item)
   {
      if (_flattened)
      {
         enumerables.Clear();
         enumerables.Add(~_flattened);
         _flattened = nil;
         Unflattened?.Invoke(this, EventArgs.Empty);
      }

      enumerables.Add(new List<T> { item });
   }

   public void Add(IEnumerable<T> enumerable)
   {
      if (_flattened)
      {
         enumerables.Clear();
         enumerables.Add(~_flattened);
         _flattened = nil;
         Unflattened?.Invoke(this, EventArgs.Empty);
      }

      enumerables.Add(enumerable);
   }

   public void Clear()
   {
      enumerables.Clear();
      _flattened = nil;
      Unflattened?.Invoke(this, EventArgs.Empty);
   }

   public bool Contains(T item)
   {
      Flatten();
      return (~_flattened).Contains(item);
   }

   public void CopyTo(T[] array, int arrayIndex)
   {
      Flatten();
      var flattened = ~_flattened;
      var length = Math.Min(flattened.Length, array.Length);
      Array.Copy(flattened, arrayIndex, array, 0, length);
   }

   public bool Remove(T item)
   {
      Flatten();
      var newEnumerable = (~_flattened).Where(i => !i.Equals(item));
      Clear();
      Add(newEnumerable);
      return true;
   }

   public int Count
   {
      get
      {
         Flatten();
         return (~_flattened).Length;
      }
   }

   public bool IsReadOnly => false;

   public int IndexOf(T item)
   {
      Flatten();
      return Array.IndexOf(_flattened, item);
   }

   public void Insert(int index, T item)
   {
      Flatten();
      var flattenedList = (~_flattened).ToList();
      flattenedList.Insert(0, item);
      Clear();
      Add(flattenedList);
   }

   public void RemoveAt(int index)
   {
      Flatten();
      var flattenedList = (~_flattened).ToList();
      flattenedList.RemoveAt(index);
      Clear();
      Add(flattenedList);
   }

   public T this[int index]
   {
      get
      {
         Flatten();
         return (~_flattened)[index];
      }
      set
      {
         Flatten();
         (~_flattened)[index] = value;
      }
   }
}
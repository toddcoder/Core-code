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
   protected Optional<T[]> _flattened;

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
      if (_flattened is (true, var flattened))
      {
         foreach (var item in flattened)
         {
            yield return item;
         }
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Add(T item)
   {
      if (_flattened is (true, var flattened))
      {
         enumerables.Clear();
         enumerables.Add(flattened);
         _flattened = nil;
         Unflattened?.Invoke(this, EventArgs.Empty);
      }

      enumerables.Add(new List<T> { item });
   }

   public void Add(IEnumerable<T> enumerable)
   {
      if (_flattened is (true, var flattened))
      {
         enumerables.Clear();
         enumerables.Add(flattened);
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
      return _flattened is (true, var flattened) && flattened.Contains(item);
   }

   public void CopyTo(T[] array, int arrayIndex)
   {
      Flatten();
      if (_flattened is (true, var flattened))
      {
         var length = Math.Min(flattened.Length, array.Length);
         Array.Copy(flattened, arrayIndex, array, 0, length);
      }
   }

   public bool Remove(T item)
   {
      Flatten();
      if (_flattened is (true, var flattened))
      {
         var newEnumerable = flattened.Where(i => !i.Equals(item));
         Clear();
         Add(newEnumerable);

         return true;
      }
      else
      {
         return false;
      }
   }

   public int Count
   {
      get
      {
         Flatten();
         return _flattened is (true, var flattened) ? flattened.Length : 0;
      }
   }

   public bool IsReadOnly => false;

   public int IndexOf(T item)
   {
      Flatten();
      return _flattened is (true, var flattened) ? Array.IndexOf(flattened, item) : -1;
   }

   public void Insert(int index, T item)
   {
      Flatten();
      if (_flattened is (true, var flattened))
      {
         var flattenedList = flattened.ToList();
         flattenedList.Insert(0, item);
         Clear();
         Add(flattenedList);
      }
   }

   public void RemoveAt(int index)
   {
      Flatten();
      if (_flattened is (true, var flattened))
      {
         var flattenedList = flattened.ToList();
         flattenedList.RemoveAt(index);
         Clear();
         Add(flattenedList);
      }
   }

   public T this[int index]
   {
      get
      {
         Flatten();
         return ((T[])_flattened)[index];
      }
      set
      {
         Flatten();
         if (_flattened is (true, var flattened))
         {
            flattened[index] = value;
         }
      }
   }
}
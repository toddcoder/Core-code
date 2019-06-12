using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.DataStructures
{
   public class ResettableQueue<T> : IEnumerable<T>
   {
      const int MAX_SIZE = 1024;

      int topIndex;
      int bottomIndex;
      T[] items;

      public ResettableQueue()
      {
         items = new T[MAX_SIZE];
         topIndex = 0;
         bottomIndex = -1;
      }

      public void Enqueue(T item) => items[++bottomIndex] = item;

      public T Dequeue() => items[topIndex++];

      public T Peek() => items[topIndex];

      public void Reset() => topIndex = 0;

      public void Clear()
      {
         topIndex = 0;
         bottomIndex = -1;
      }

      public int Count => bottomIndex - topIndex + 1;

      public bool IsEmpty => Count == 0;

      public bool IsFull => Count >= MAX_SIZE;

      public bool Contains(T item)
      {
         for (var i = topIndex; i <= bottomIndex; i++)
            if (items.Equals(item))
               return true;

         return false;
      }

      public void CopyTo(T[] array, int arrayIndex) => Array.Copy(items, topIndex, array, arrayIndex, Count);

      public IEnumerator<T> GetEnumerator()
      {
         for (var i = topIndex; i <= bottomIndex; i++)
            yield return items[i];
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public T[] ToArray()
      {
         var array = new T[Count];
         CopyTo(array, 0);

         return array;
      }

      public IEither<T, T> DequeueEither()
      {
         if (Count == 1)
            return Dequeue().RightHand<T, T>();
         else
            return Dequeue().LeftHand<T, T>();
      }

      public ResettableQueueTrying<T> TryTo => new ResettableQueueTrying<T>(this);
   }
}
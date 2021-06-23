using System;
using System.Collections;
using System.Collections.Generic;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.DataStructures
{
   public class PriorityQueue<T> : IQueue<T>, IEnumerable<T>, IEquatable<PriorityQueue<T>> where T : IComparable<T>
   {
      protected List<T> list;

      public PriorityQueue()
      {
         list = new List<T>();
      }

      public PriorityQueue(IEnumerable<T> enumerable)
      {
         foreach (var item in enumerable)
         {
            Enqueue(item);
         }
      }

      public PriorityQueue(params T[] args)
      {
         foreach (var item in args)
         {
            Enqueue(item);
         }
      }

      public int Count => list.Count;

      public void Clear() => list.Clear();

      public bool Contains(T item) => list.Contains(item);

      public IResult<T[]> ToArray(int arrayIndex = 0)
      {
         return
            from assertion in arrayIndex.Must().BeBetween(0).Until(Count).OrFailure()
            from array in tryTo(() =>
            {
               var result = new T[Count];
               list.CopyTo(result, arrayIndex);

               return result;
            })
            select array;
      }

      public void Enqueue(T item)
      {
         list.Add(item);
         var compareIndex = list.Count - 1;
         while (compareIndex > 0)
         {
            var pivotIndex = (compareIndex - 1) / 2;
            if (list[compareIndex].CompareTo(list[pivotIndex]) >= 0)
            {
               break;
            }

            var temp = list[compareIndex];
            list[compareIndex] = list[pivotIndex];
            list[pivotIndex] = temp;

            compareIndex = pivotIndex;
         }
      }

      public Maybe<T> Peek() => maybe(IsNotEmpty, () => list[0]);

      public Maybe<T> Dequeue()
      {
         if (list.Count == 0)
         {
            return none<T>();
         }

         var lastIndex = list.Count - 1;
         var frontItem = list[0];
         list[0] = list[lastIndex];
         list.RemoveAt(lastIndex);

         lastIndex--;
         var pivotIndex = 0;

         while (true)
         {
            var currentIndex = pivotIndex * 2 + 1;
            if (currentIndex > lastIndex)
            {
               break;
            }

            var nextIndex = currentIndex + 1;
            if (nextIndex <= lastIndex && list[nextIndex].CompareTo(list[currentIndex]) < 0)
            {
               currentIndex = nextIndex;
            }

            if (list[pivotIndex].CompareTo(list[currentIndex]) <= 0)
            {
               break;
            }

            var temp = list[pivotIndex];
            list[pivotIndex] = list[currentIndex];
            list[currentIndex] = temp;

            pivotIndex = currentIndex;
         }

         return frontItem.Some();
      }

      public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public bool Equals(PriorityQueue<T> other) => Equals(list, other.list);

      public override bool Equals(object obj) => obj is PriorityQueue<T> other && Equals(other);

      public override int GetHashCode() => list.GetHashCode();

      public bool IsEmpty => list.Count == 0;

      public bool IsNotEmpty => list.Count > 0;
   }
}
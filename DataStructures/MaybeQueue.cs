﻿using System.Collections;
using System.Collections.Generic;
using Core.Assertions;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.DataStructures
{
   public class MaybeQueue<T> : IQueue<T>, IEnumerable<T>
   {
      protected Queue<T> queue;

      public MaybeQueue() => queue = new Queue<T>();

      public MaybeQueue(IEnumerable<T> collection) => queue = new Queue<T>(collection);

      public MaybeQueue(int capacity) => queue = new Queue<T>(capacity);

      public int Count => queue.Count;

      public void Clear() => queue.Clear();

      public bool Contains(T item) => queue.Contains(item);

      public IResult<T[]> ToArray(int arrayIndex = 0)
      {
         return
            from assertion in assert(() => arrayIndex).Must().BeBetween(0).Until(Count).OrFailure()
            from array in tryTo(() =>
            {
               var result = new T[Count];
               queue.CopyTo(result, arrayIndex);

               return result;
            })
            select array;
      }

      public IMaybe<T> Dequeue() => maybe(IsNotEmpty, () => queue.Dequeue());

      public void Enqueue(T item) => queue.Enqueue(item);

      public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();

      public override bool Equals(object obj) => obj is MaybeQueue<T> q && q == this;

      public override int GetHashCode() => queue.GetHashCode();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public IMaybe<T> Peek() => maybe(IsNotEmpty, () => queue.Peek());

      public T[] ToArray() => queue.ToArray();

      public void TrimExcess() => queue.TrimExcess();

      public bool IsEmpty => queue.Count == 0;

      public bool IsNotEmpty => queue.Count > 0;
   }
}
using System.Collections;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.DataStructures
{
   public class MaybeQueue<T> : IEnumerable<T>
   {
      protected Queue<T> queue;

      public MaybeQueue() => queue = new Queue<T>();

      public MaybeQueue(IEnumerable<T> collection) => queue = new Queue<T>(collection);

      public MaybeQueue(int capacity) => queue = new Queue<T>(capacity);

      public int Count => queue.Count;

      public void Clear() => queue.Clear();

      public bool Contains(T item) => queue.Contains(item);

      public void CopyTo(T[] array, int arrayIndex) => queue.CopyTo(array, arrayIndex);

      public IMaybe<T> Dequeue() => when(queue.Count > 0, () => queue.Dequeue());

      public void Enqueue(T item) => queue.Enqueue(item);

      public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();

      public override bool Equals(object obj) => obj is MaybeQueue<T> q && q == this;

      public override int GetHashCode() => queue.GetHashCode();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public IMaybe<T> Peek() => when(queue.Count > 0, () => queue.Peek());

      public T[] ToArray() => queue.ToArray();

      public void TrimExcess() => queue.TrimExcess();
   }
}
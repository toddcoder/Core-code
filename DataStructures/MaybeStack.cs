using System.Collections;
using System.Collections.Generic;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.DataStructures
{
   public class MaybeStack<T> : IEnumerable<T>
   {
      protected Stack<T> stack;

      public MaybeStack() => stack = new Stack<T>();

      public MaybeStack(IEnumerable<T> collection) => stack = new Stack<T>(collection);

      public MaybeStack(int capacity) => stack = new Stack<T>(capacity);

      public int Count => stack.Count;

      public void Clear() => stack.Clear();

      public bool Contains(T item) => stack.Contains(item);

      public void CopyTo(T[] array, int arrayIndex) => stack.CopyTo(array, arrayIndex);

      public IMaybe<T> Peek() => when(stack.Count > 0, () => stack.Peek());

      public IMaybe<T> Pop() => when(stack.Count > 0, () => stack.Pop());

      public void Push(T item) => stack.Push(item);

      public T[] ToArray() => stack.ToArray();

      public IEnumerator<T> GetEnumerator() => stack.GetEnumerator();

      public override string ToString() => stack.Stringify();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public void TrimExcess() => stack.TrimExcess();
   }
}
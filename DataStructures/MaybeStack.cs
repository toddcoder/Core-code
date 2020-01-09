using System.Collections;
using System.Collections.Generic;
using Core.Assertions;
using Core.Enumerables;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.AttemptFunctions;
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

      public IResult<T[]> ToArray(int arrayIndex = 0)
      {
         return
            from assertion in assert(() => arrayIndex).Must().BeBetween(0).Until(Count).OrFailure()
            from array in tryTo(() =>
            {
               var result = new T[Count];
               stack.CopyTo(result, arrayIndex);

               return result;
            })
            select array;
      }

      public IMaybe<T> Peek() => maybe(stack.Count > 0, () => stack.Peek());

      public IMaybe<T> Pop() => maybe(stack.Count > 0, () => stack.Pop());

      public void Push(T item) => stack.Push(item);

      public T[] ToArray() => stack.ToArray();

      public IEnumerator<T> GetEnumerator() => stack.GetEnumerator();

      public override string ToString() => stack.Stringify();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public void TrimExcess() => stack.TrimExcess();
   }
}
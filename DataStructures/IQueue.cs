using Core.Monads;

namespace Core.DataStructures
{
   public interface IQueue<T>
   {
      int Count { get; }

      void Clear();

      bool Contains(T item);

      Result<T[]> ToArray(int arrayIndex = 0);

      Maybe<T> Dequeue();

      void Enqueue(T item);

      Maybe<T> Peek();

      bool IsEmpty { get; }

      bool IsNotEmpty { get; }
   }
}
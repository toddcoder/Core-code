using Core.Monads;

namespace Core.DataStructures;

public interface IQueue<T>
{
   int Count { get; }

   void Clear();

   bool Contains(T item);

   Optional<T[]> ToArray(int arrayIndex = 0);

   Optional<T> Dequeue();

   void Enqueue(T item);

   Optional<T> Peek();

   bool IsEmpty { get; }

   bool IsNotEmpty { get; }
}
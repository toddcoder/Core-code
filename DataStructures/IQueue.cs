using Core.Monads;

namespace Core.DataStructures
{
   public interface IQueue<T>
   {
      int Count { get; }

      void Clear();

      bool Contains(T item);

      IResult<T[]> ToArray(int arrayIndex = 0);

      IMaybe<T> Dequeue();

      void Enqueue(T item);

      IMaybe<T> Peek();

      bool IsEmpty { get; }

      bool IsNotEmpty { get; }
   }
}
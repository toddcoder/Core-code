using Core.Monads;
using static Core.Monads.AttemptFunctions;

namespace Core.DataStructures
{
   public class ResettableQueueTrying<T>
   {
      ResettableQueue<T> queue;

      public ResettableQueueTrying(ResettableQueue<T> queue) => this.queue = queue;

      public IResult<Unit> Enqueue(T item) => reject(queue.IsFull, () =>
      {
         queue.Enqueue(item);
         return Unit.Value;
      }, "Queue is full");

      public IResult<T> Dequeue() => reject(queue.IsEmpty, () => queue.Dequeue(), "Queue is empty");

      public IResult<T> Peek() => reject(queue.IsEmpty, () => queue.Peek(), "Queue is empty");
   }
}
using Core.Monads;

namespace Core.DataStructures
{
   public class ResultQueue<T>
   {
      protected MaybeQueue<T> queue;

      internal ResultQueue(MaybeQueue<T> queue)
      {
         this.queue = queue;
      }

      public IResult<T> Dequeue(string message) => queue.Dequeue().Result(message);

      public IResult<T> Dequeue() => Dequeue("Empty queue");

      public IResult<T> Peek(string message) => queue.Peek().Result(message);

      public IResult<T> Peek() => Peek("Empty queue");
   }
}
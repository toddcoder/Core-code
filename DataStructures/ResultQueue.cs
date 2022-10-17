using Core.Monads;

namespace Core.DataStructures;

public class ResultQueue<T>
{
   protected MaybeQueue<T> queue;

   internal ResultQueue(MaybeQueue<T> queue)
   {
      this.queue = queue;
   }

   public Result<T> Dequeue(string message) => queue.Dequeue().Result(message);

   public Result<T> Dequeue() => Dequeue("Empty queue");

   public Result<T> Peek(string message) => queue.Peek().Result(message);

   public Result<T> Peek() => Peek("Empty queue");
}
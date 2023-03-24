using Core.Monads;

namespace Core.DataStructures;

public class ResultQueue<T>
{
   protected MaybeQueue<T> queue;

   internal ResultQueue(MaybeQueue<T> queue)
   {
      this.queue = queue;
   }

   public Optional<T> Dequeue(string message) => queue.Dequeue().Result(message);

   public Optional<T> Dequeue() => Dequeue("Empty queue");

   public Optional<T> Peek(string message) => queue.Peek().Result(message);

   public Optional<T> Peek() => Peek("Empty queue");
}
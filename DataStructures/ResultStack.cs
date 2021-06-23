using Core.Monads;

namespace Core.DataStructures
{
   public class ResultStack<T>
   {
      protected MaybeStack<T> stack;

      internal ResultStack(MaybeStack<T> stack) => this.stack = stack;

      public Result<T> Peek(string message) => stack.Peek().Result(message);

      public Result<T> Peek() => Peek("Empty stack");

      public Result<T> Pop(string message) => stack.Pop().Result(message);

      public Result<T> Pop() => Pop("Empty stack");
   }
}
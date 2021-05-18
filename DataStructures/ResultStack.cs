using Core.Monads;

namespace Core.DataStructures
{
   public class ResultStack<T>
   {
      protected MaybeStack<T> stack;

      internal ResultStack(MaybeStack<T> stack) => this.stack = stack;

      public IResult<T> Peek(string message) => stack.Peek().Result(message);

      public IResult<T> Peek() => Peek("Empty stack");

      public IResult<T> Pop(string message) => stack.Pop().Result(message);

      public IResult<T> Pop() => Pop("Empty stack");
   }
}
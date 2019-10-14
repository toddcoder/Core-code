using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections
{
   public static class StackExtensions
   {
      public static IMaybe<T> PopIf<T>(this Stack<T> stack) => maybe(stack.IsNotEmpty(), stack.Pop);

      public static IMaybe<T> PeekIf<T>(this Stack<T> stack) => maybe(stack.IsNotEmpty(), stack.Peek);

      public static bool IsEmpty<T>(this Stack<T> stack) => stack.Count == 0;

      public static bool IsNotEmpty<T>(this Stack<T> stack) => stack.Count > 0;
   }
}
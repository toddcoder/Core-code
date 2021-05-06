using Core.Exceptions;

namespace Core.Monads
{
   public static class EitherExtensions
   {
      public static Either<TLeft, TRight> Left<TLeft, TRight>(this TLeft value) => new Left<TLeft, TRight>(value);

      public static Either<TLeft, TRight> Right<TLeft, TRight>(this TRight value) => new Right<TLeft, TRight>(value);

      public static Either<TLeft, TRight> Either<TLeft, TRight>(this (IMaybe<TLeft>, IMaybe<TRight>) tuple)
      {
         var (_left, _right) = tuple;
         if (_left.If(out var left))
         {
            return left.Left<TLeft, TRight>();
         }
         else if (_right.If(out var right))
         {
            return right.Right<TLeft, TRight>();
         }
         else
         {
            throw "Either left or right must have a value".Throws();
         }
      }

      public static Either<TLeft, TRight> Either<TLeft, TRight>(this TLeft left) => left.Left<TLeft, TRight>();

      public static Either<TLeft, TRight> Either<TLeft, TRight>(this TRight right) => right.Right<TLeft, TRight>();
   }
}
namespace Core.Monads
{
   public static class EitherExtensions
   {
      public static Either<TLeft, TRight> Left<TLeft, TRight>(this TLeft value) => new Left<TLeft, TRight>(value);

      public static Either<TLeft, TRight> Right<TLeft, TRight>(this TRight value) => new Right<TLeft, TRight>(value);

      public static Either<TLeft, TRight> Either<TLeft, TRight>(this TLeft left) => left.Left<TLeft, TRight>();

      public static Either<TLeft, TRight> Either<TLeft, TRight>(this TRight right) => right.Right<TLeft, TRight>();

      public static LeftHand<T> Left<T>(this T value) => new(value);

      public static RightHand<T> Right<T>(this T value) => new(value);
   }
}
namespace Core.Monads
{
   public static class EitherExtensions
   {
      public static Either<TLeft, TRight> Either<TLeft, TRight>(this TLeft left) => new Left<TLeft, TRight>(left);

      public static Either<TLeft, TRight> Either<TLeft, TRight>(this TRight right) => new Right<TLeft, TRight>(right);

      public static LeftHand<T> Left<T>(this T value) => new(value);

      public static RightHand<T> Right<T>(this T value) => new(value);
   }
}
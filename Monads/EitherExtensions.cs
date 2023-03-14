namespace Core.Monads;

public static class EitherExtensions
{
   public static Either<TLeft, TRight> Either<TLeft, TRight>(this TLeft left) => new Left<TLeft, TRight>(left);

   public static Either<TLeft, TRight> Either<TLeft, TRight>(this TRight right) => new Right<TLeft, TRight>(right);
}
namespace Core.Monads
{
   public class EitherMaybe<TLeft, TRight>
   {
      protected Either<TLeft, TRight> _either;

      public EitherMaybe(Either<TLeft, TRight> either)
      {
         _either = either;
      }

      public Maybe<TLeft> Left => _either.MaybeFromLeft();

      public Maybe<TRight> Right => _either.MaybeFromRight();
   }
}
using System;

namespace Core.Monads
{
   public abstract class Either<TLeft, TRight>
   {
      public static implicit operator Either<TLeft, TRight>(TLeft value) => new Left<TLeft, TRight>(value);

      public static implicit operator Either<TLeft, TRight>(TRight value) => new Right<TLeft, TRight>(value);

      public abstract bool IfLeft(out TLeft value);

      public abstract bool IfLeft(out TLeft left, out TRight right);

      public abstract bool IfRight(out TRight value);

      public abstract bool IfRight(out TRight right, out TLeft left);

      public abstract bool IsLeft { get; }

      public abstract bool IsRight { get; }

      public abstract Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, TLeftResult> leftMap,
         Func<TRight, TRightResult> rightMap);

      public abstract Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, Either<TLeftResult, TRightResult>> leftMap,
         Func<TRight, Either<TLeftResult, TRightResult>> rightMap);

      public abstract void Deconstruct(out Maybe<TLeft> left, out Maybe<TRight> right);

      public abstract Maybe<TLeft> LeftValue { get; }

      public abstract Maybe<TRight> RightValue { get; }

      [Obsolete("Use LeftValue")]
      public abstract Maybe<TLeft> MaybeFromLeft();

      [Obsolete("Use RightValue")]
      public abstract Maybe<TRight> MaybeFromRight();

      public abstract Result<TLeft> ResultFromLeft(string exceptionMessage);

      public abstract Result<TLeft> ResultFromLeft(Func<TRight, string> exceptionMessage);

      public abstract Result<TRight> ResultFromRight(string exceptionMessage);

      public abstract Result<TRight> ResultFromRight(Func<TLeft, string> exceptionMessage);

      public abstract Either<TLeft, TRight> OnLeft(Action<TLeft> action);

      public abstract Either<TLeft, TRight> OnRight(Action<TRight> action);

      public abstract TLeft DefaultToLeft(Func<TLeft> map);

      public abstract TRight DefaultToRight(Func<TRight> map);
   }
}
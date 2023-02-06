using System;

namespace Core.Monads;

public abstract class Either<TLeft, TRight>
{
   public static implicit operator Either<TLeft, TRight>(TLeft value) => new Left<TLeft, TRight>(value);

   public static implicit operator Either<TLeft, TRight>(TRight value) => new Right<TLeft, TRight>(value);

   public static TLeft operator |(Either<TLeft, TRight> either, TLeft defaultValue) => either is ((true, var left), _) ? left : defaultValue;

   public static TLeft operator |(Either<TLeft, TRight> either, Func<TLeft> defaultValue) => either is ((true, var left), _) ? left : defaultValue();

   public static TRight operator |(Either<TLeft, TRight> either, TRight defaultValue) => either is (_, (true, var right)) ? right : defaultValue;

   public static TRight operator |(Either<TLeft, TRight> either, Func<TRight> defaultValue)
   {
      return either is (_, (true, var right)) ? right : defaultValue();
   }

   [Obsolete("Use deconstruction")]
   public abstract bool IfLeft(out TLeft value);

   [Obsolete("Use deconstruction")]
   public abstract bool IfLeft(out TLeft left, out TRight right);

   [Obsolete("Use deconstruction")]
   public abstract bool IfRight(out TRight value);

   [Obsolete("Use deconstruction")]
   public abstract bool IfRight(out TRight right, out TLeft left);

   [Obsolete("Use deconstruction")]
   public abstract bool IsLeft { get; }

   [Obsolete("Use deconstruction")]
   public abstract bool IsRight { get; }

   public abstract Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, TLeftResult> leftMap,
      Func<TRight, TRightResult> rightMap);

   public abstract Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, Either<TLeftResult, TRightResult>> leftMap,
      Func<TRight, Either<TLeftResult, TRightResult>> rightMap);

   [Obsolete("Use deconstruction")]
   public abstract Maybe<TLeft> LeftValue { get; }

   [Obsolete("Use deconstruction")]
   public abstract Maybe<TRight> RightValue { get; }

   [Obsolete("Use deconstruction")]
   public abstract Maybe<TLeft> MaybeFromLeft();

   [Obsolete("Use deconstruction")]
   public abstract Maybe<TRight> MaybeFromRight();

   [Obsolete("Use deconstruction")]
   public abstract Result<TLeft> ResultFromLeft(string exceptionMessage);

   [Obsolete("Use deconstruction")]
   public abstract Result<TLeft> ResultFromLeft(Func<TRight, string> exceptionMessage);

   [Obsolete("Use deconstruction")]
   public abstract Result<TRight> ResultFromRight(string exceptionMessage);

   [Obsolete("Use deconstruction")]
   public abstract Result<TRight> ResultFromRight(Func<TLeft, string> exceptionMessage);

   [Obsolete("Use deconstruction")]
   public abstract Either<TLeft, TRight> OnLeft(Action<TLeft> action);

   [Obsolete("Use deconstruction")]
   public abstract Either<TLeft, TRight> OnRight(Action<TRight> action);

   [Obsolete("Use deconstruction")]
   public abstract TLeft DefaultToLeft(Func<TLeft> map);

   [Obsolete("Use deconstruction")]
   public abstract TRight DefaultToRight(Func<TRight> map);

   public abstract void Deconstruct(out Maybe<TLeft> left, out Maybe<TRight> right);
}
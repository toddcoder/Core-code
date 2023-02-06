using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Right<TLeft, TRight> : Either<TLeft, TRight>, IEquatable<Right<TLeft, TRight>>
{
   protected TRight value;

   internal Right(TRight value) => this.value = value;

   public TRight Value => value;

   [Obsolete("Use deconstruction")]
   public override bool IfLeft(out TLeft value)
   {
      value = default;
      return false;
   }

   [Obsolete("Use deconstruction")]
   public override bool IfLeft(out TLeft left, out TRight right)
   {
      left = default;
      right = value;

      return false;
   }

   [Obsolete("Use deconstruction")]
   public override bool IfRight(out TRight value)
   {
      value = this.value;
      return true;
   }

   [Obsolete("Use deconstruction")]
   public override bool IfRight(out TRight right, out TLeft left)
   {
      right = value;
      left = default;

      return true;
   }

   [Obsolete("Use deconstruction")]
   public override bool IsLeft => false;

   [Obsolete("Use deconstruction")]
   public override bool IsRight => true;

   public override Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, TLeftResult> leftMap,
      Func<TRight, TRightResult> rightMap)
   {
      return new Right<TLeftResult, TRightResult>(rightMap(value));
   }

   public override Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, Either<TLeftResult, TRightResult>> leftMap,
      Func<TRight, Either<TLeftResult, TRightResult>> rightMap)
   {
      return rightMap(value);
   }

   [Obsolete("Use deconstruction")]
   public override Maybe<TLeft> LeftValue => nil;

   [Obsolete("Use deconstruction")]
   public override Maybe<TRight> RightValue => value;

   [Obsolete("Use LeftValue")]
   public override Maybe<TLeft> MaybeFromLeft() => nil;

   [Obsolete("Use RightValue")]
   public override Maybe<TRight> MaybeFromRight() => value;

   [Obsolete("Use deconstruction")]
   public override Result<TLeft> ResultFromLeft(string exceptionMessage) => fail(exceptionMessage);

   [Obsolete("Use deconstruction")]
   public override Result<TLeft> ResultFromLeft(Func<TRight, string> exceptionMessage) => fail(exceptionMessage(value));

   [Obsolete("Use deconstruction")]
   public override Result<TRight> ResultFromRight(string exceptionMessage) => value;

   [Obsolete("Use deconstruction")]
   public override Result<TRight> ResultFromRight(Func<TLeft, string> exceptionMessage) => value;

   [Obsolete("Use deconstruction")]
   public override Either<TLeft, TRight> OnLeft(Action<TLeft> action) => this;

   [Obsolete("Use deconstruction")]
   public override Either<TLeft, TRight> OnRight(Action<TRight> action)
   {
      action(value);
      return this;
   }

   [Obsolete("Use deconstruction")]
   public override TLeft DefaultToLeft(Func<TLeft> map) => map();

   [Obsolete("Use deconstruction")]
   public override TRight DefaultToRight(Func<TRight> map) => value;

   public override void Deconstruct(out Maybe<TLeft> left, out Maybe<TRight> right)
   {
      left = nil;
      right = value;
   }

   public bool Equals(Right<TLeft, TRight> other)
   {
      return other is not null && (ReferenceEquals(this, other) || EqualityComparer<TRight>.Default.Equals(value, other.value));
   }

   public override bool Equals(object obj) => obj is Right<TLeft, TRight> other && Equals(other);

   public override int GetHashCode() => EqualityComparer<TRight>.Default.GetHashCode(value);

   public override string ToString() => value.ToString();
}
using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Right<TLeft, TRight> : Either<TLeft, TRight>, IEquatable<Right<TLeft, TRight>>
   {
      protected TRight value;

      internal Right(TRight value) => this.value = value;

      public TRight Value => value;

      public override bool IfLeft(out TLeft value)
      {
         value = default;
         return false;
      }

      public override bool IfLeft(out TLeft left, out TRight right)
      {
         left = default;
         right = value;

         return false;
      }

      public override bool IfRight(out TRight value)
      {
         value = this.value;
         return true;
      }

      public override bool IfRight(out TRight right, out TLeft left)
      {
         right = value;
         left = default;

         return true;
      }

      public override bool IsLeft => false;

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

      public override void Deconstruct(out Maybe<TLeft> left, out Maybe<TRight> right)
      {
         left = nil;
         right = value;
      }

      public override Maybe<TLeft> MaybeFromLeft() => nil;

      public override Maybe<TRight> MaybeFromRight() => value;

      public override Result<TLeft> ResultFromLeft(string exceptionMessage) => fail(exceptionMessage);

      public override Result<TLeft> ResultFromLeft(Func<TRight, string> exceptionMessage) => fail(exceptionMessage(value));

      public override Result<TRight> ResultFromRight(string exceptionMessage) => value;

      public override Result<TRight> ResultFromRight(Func<TLeft, string> exceptionMessage) => value;

      public override Either<TLeft, TRight> OnLeft(Action<TLeft> action) => this;

      public override Either<TLeft, TRight> OnRight(Action<TRight> action)
      {
         action(value);
         return this;
      }

      public override TLeft DefaultToLeft(Func<TLeft> map) => map();

      public override TRight DefaultToRight(Func<TRight> map) => value;

      public bool Equals(Right<TLeft, TRight> other)
      {
         return other is not null && (ReferenceEquals(this, other) || EqualityComparer<TRight>.Default.Equals(value, other.value));
      }

      public override bool Equals(object obj) => obj is Right<TLeft, TRight> other && Equals(other);

      public override int GetHashCode() => EqualityComparer<TRight>.Default.GetHashCode(value);

      public override string ToString() => value.ToString();
   }
}
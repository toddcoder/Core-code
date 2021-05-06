using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Right<TLeft, TRight> : Either<TLeft, TRight>, IEquatable<Right<TLeft, TRight>>
   {
      protected TRight value;

      internal Right(TRight value) => this.value = value;

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

      public override void Deconstruct(out IMaybe<TLeft> left, out IMaybe<TRight> right)
      {
         left = none<TLeft>();
         right = value.Some();
      }

      public override IMaybe<TLeft> MaybeFromLeft() => none<TLeft>();

      public override IMaybe<TRight> MaybeFromRight() => value.Some();

      public override IResult<TLeft> ResultFromLeft(string exceptionMessage) => exceptionMessage.Failure<TLeft>();

      public override IResult<TLeft> ResultFromLeft(Func<TRight, string> exceptionMessage) => exceptionMessage(value).Failure<TLeft>();

      public override IResult<TRight> ResultFromRight(string exceptionMessage) => value.Success();

      public override IResult<TRight> ResultFromRight(Func<TLeft, string> exceptionMessage) => value.Success();

      public override Either<TLeft, TRight> OnLeft(Action<TLeft> action) => this;

      public override Either<TLeft, TRight> OnRight(Action<TRight> action)
      {
         action(value);
         return this;
      }

      public bool Equals(Right<TLeft, TRight> other)
      {
         return other is not null && (ReferenceEquals(this, other) || EqualityComparer<TRight>.Default.Equals(value, other.value));
      }

      public override bool Equals(object obj) => obj is Right<TLeft, TRight> other && Equals(other);

      public override int GetHashCode() => EqualityComparer<TRight>.Default.GetHashCode(value);

      public override string ToString() => $"Right({value})";
   }
}
using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Left<TLeft, TRight> : Either<TLeft, TRight>, IEquatable<Left<TLeft, TRight>>
   {
      protected TLeft value;

      internal Left(TLeft value) => this.value = value;

      public TLeft Value => value;

      public override bool IfLeft(out TLeft value)
      {
         value = this.value;
         return true;
      }

      public override bool IfLeft(out TLeft left, out TRight right)
      {
         left = value;
         right = default;

         return true;
      }

      public override bool IfRight(out TRight value)
      {
         value = default;
         return false;
      }

      public override bool IfRight(out TRight right, out TLeft left)
      {
         right = default;
         left = value;

         return false;
      }

      public override bool IsLeft => true;

      public override bool IsRight => false;

      public override Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, TLeftResult> leftMap,
         Func<TRight, TRightResult> rightMap)
      {
         return new Left<TLeftResult, TRightResult>(leftMap(value));
      }

      public override Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, Either<TLeftResult, TRightResult>> leftMap,
         Func<TRight, Either<TLeftResult, TRightResult>> rightMap)
      {
         return leftMap(value);
      }

      public override void Deconstruct(out Maybe<TLeft> left, out Maybe<TRight> right)
      {
         left = value.Some();
         right = none<TRight>();
      }

      public override Maybe<TLeft> MaybeFromLeft() => value.Some();

      public override Maybe<TRight> MaybeFromRight() => none<TRight>();

      public override Result<TLeft> ResultFromLeft(string exceptionMessage) => value.Success();

      public override Result<TLeft> ResultFromLeft(Func<TRight, string> exceptionMessage) => value.Success();

      public override Result<TRight> ResultFromRight(string exceptionMessage) => exceptionMessage.Failure<TRight>();

      public override Result<TRight> ResultFromRight(Func<TLeft, string> exceptionMessage) => exceptionMessage(value).Failure<TRight>();

      public override Either<TLeft, TRight> OnLeft(Action<TLeft> action)
      {
         action(value);
         return this;
      }

      public override Either<TLeft, TRight> OnRight(Action<TRight> action) => this;

      public override TLeft DefaultToLeft(Func<TLeft> map) => value;

      public override TRight DefaultToRight(Func<TRight> map) => map();

      public bool Equals(Left<TLeft, TRight> other)
      {
         return other is not null && (ReferenceEquals(this, other) || EqualityComparer<TLeft>.Default.Equals(value, other.value));
      }

      public override bool Equals(object obj) => obj is Left<TLeft, TRight> other && Equals(other);

      public override int GetHashCode() => EqualityComparer<TLeft>.Default.GetHashCode(value);

      public override string ToString() => $"Left({value})";
   }
}
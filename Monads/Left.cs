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

      [Obsolete("Use deconstruction")]
      public override bool IfLeft(out TLeft value)
      {
         value = this.value;
         return true;
      }

      [Obsolete("Use deconstruction")]
      public override bool IfLeft(out TLeft left, out TRight right)
      {
         left = value;
         right = default;

         return true;
      }

      [Obsolete("Use deconstruction")]
      public override bool IfRight(out TRight value)
      {
         value = default;
         return false;
      }

      [Obsolete("Use deconstruction")]
      public override bool IfRight(out TRight right, out TLeft left)
      {
         right = default;
         left = value;

         return false;
      }

      [Obsolete("Use deconstruction")]
      public override bool IsLeft => true;

      [Obsolete("Use deconstruction")]
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


      [Obsolete("Use deconstruction")]
      public override Maybe<TLeft> LeftValue => value;

      [Obsolete("Use deconstruction")]
      public override Maybe<TRight> RightValue => nil;

      [Obsolete("Use LeftValue")]
      public override Maybe<TLeft> MaybeFromLeft() => value;

      [Obsolete("Use RightValue")]
      public override Maybe<TRight> MaybeFromRight() => nil;

      [Obsolete("Use deconstruction")]
      public override Result<TLeft> ResultFromLeft(string exceptionMessage) => value;

      [Obsolete("Use deconstruction")]
      public override Result<TLeft> ResultFromLeft(Func<TRight, string> exceptionMessage) => value;

      [Obsolete("Use deconstruction")]
      public override Result<TRight> ResultFromRight(string exceptionMessage) => fail(exceptionMessage);

      [Obsolete("Use deconstruction")]
      public override Result<TRight> ResultFromRight(Func<TLeft, string> exceptionMessage) => fail(exceptionMessage(value));

      [Obsolete("Use deconstruction")]
      public override Either<TLeft, TRight> OnLeft(Action<TLeft> action)
      {
         action(value);
         return this;
      }

      [Obsolete("Use deconstruction")]
      public override Either<TLeft, TRight> OnRight(Action<TRight> action) => this;

      [Obsolete("Use deconstruction")]
      public override TLeft DefaultToLeft(Func<TLeft> map) => value;

      [Obsolete("Use deconstruction")]
      public override TRight DefaultToRight(Func<TRight> map) => map();

      public override void Deconstruct(out Maybe<TLeft> left, out Maybe<TRight> right)
      {
         left = value;
         right = nil;
      }

      public bool Equals(Left<TLeft, TRight> other)
      {
         return other is not null && (ReferenceEquals(this, other) || EqualityComparer<TLeft>.Default.Equals(value, other.value));
      }

      public override bool Equals(object obj) => obj is Left<TLeft, TRight> other && Equals(other);

      public override int GetHashCode() => EqualityComparer<TLeft>.Default.GetHashCode(value);

      public override string ToString() => value.ToString();
   }
}
﻿using System;

namespace Core.Monads
{
   public abstract class Either<TLeft, TRight>
   {
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

      public abstract void Deconstruct(out IMaybe<TLeft> left, out IMaybe<TRight> right);

      public abstract IMaybe<TLeft> MaybeFromLeft();

      public abstract IMaybe<TRight> MaybeFromRight();

      public abstract IResult<TLeft> ResultFromLeft(string exceptionMessage);

      public abstract IResult<TLeft> ResultFromLeft(Func<TRight, string> exceptionMessage);

      public abstract IResult<TRight> ResultFromRight(string exceptionMessage);

      public abstract IResult<TRight> ResultFromRight(Func<TLeft, string> exceptionMessage);

      public abstract Either<TLeft, TRight> OnLeft(Action<TLeft> action);

      public abstract Either<TLeft, TRight> OnRight(Action<TRight> action);

      public abstract TLeft DefaultToLeft(Func<TLeft> map);

      public abstract TRight DefaultToRight(Func<TRight> map);
   }
}
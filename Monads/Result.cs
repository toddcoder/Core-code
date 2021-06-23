using System;
using static Core.Monads.AttemptFunctions;

namespace Core.Monads
{
   public abstract class Result<T>
   {
      public abstract bool If(out T value, out Exception exception);

      public abstract bool ValueOrOriginal(out T value, out Result<T> original);

      public abstract bool ValueOrCast<TResult>(out T value, out Result<TResult> result);

      public abstract bool IsSuccessful { get; }

      public abstract bool IsFailed { get; }

      public abstract Result<TOther> ExceptionAs<TOther>();

      public abstract Result<TResult> Map<TResult>(Func<T, Result<TResult>> ifSuccessful);

      public abstract Result<TResult> Map<TResult>(Func<T, TResult> ifSuccessful);

      public abstract Result<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection);

      public abstract Result<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection);

      public abstract Result<TResult> SelectMany<TResult>(Func<T, TResult> func);

      public abstract T Recover(Func<Exception, T> recovery);

      public abstract Result<T> Or(Result<T> other);

      public abstract Result<T> Or(Func<Result<T>> other);

      public abstract Result<T> Or(T other);

      public abstract Result<T> Or(Func<T> other);

      public abstract Result<Unit> Unit { get; }

      public abstract Result<T> Always(Action action);

      public abstract Matched<T> Match();

      public abstract bool If(out T value);

      public abstract bool IfNot(out Exception exception);

      public abstract bool IfNot(out T value, out Exception exception);

      public abstract void Force();

      public abstract T ForceValue();

      public abstract Result<T> OnSuccess(Action<T> action);

      public abstract Result<T> OnFailure(Action<Exception> action);

      public abstract void Deconstruct(out Maybe<T> value, out Exception exception);

      public abstract Result<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage);

      public abstract Maybe<T> Maybe();

      public abstract bool EqualToValueOf(Result<T> otherResult);

      public abstract bool ValueEqualTo(T otherValue);

      public abstract Result<T> Otherwise(Func<Exception, T> func);

      public abstract Result<T> Otherwise(Func<Exception, Result<T>> func);

      public abstract Result<TResult> CastAs<TResult>();

      public abstract Result<T> Where(Predicate<T> predicate, string exceptionMessage);

      public abstract Result<T> Where(Predicate<T> predicate, Func<string> exceptionMessage);

      public abstract Result<T> ExceptionMessage(string message);

      public abstract Result<T> ExceptionMessage(Func<Exception, string> message);

      public Result<T> Tap(Action<Result<T>> action) => tryTo(() =>
      {
         action(this);
         return this;
      });
   }
}
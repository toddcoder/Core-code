using System;
using static Core.Monads.AttemptFunctions;

namespace Core.Monads
{
   public abstract class Result<T>
   {
      public static Result<T> operator |(Result<T> left, Result<T> right)
      {
         if (left)
         {
            return left;
         }
         else if (right)
         {
            return right;
         }
         else
         {
            return left;
         }
      }

      public static Result<T> operator |(Result<T> left, Func<Result<T>> rightFunc)
      {
         if (left)
         {
            return left;
         }
         else
         {
            var right = rightFunc();
            if (right)
            {
               return right;
            }
            else
            {
               return left;
            }
         }
      }

      public static implicit operator Result<T>(T value) => value.Success();

      public static implicit operator Result<T>(Exception exception) => new Failure<T>(exception);

      public static bool operator true(Result<T> value) => value is Success<T>;

      public static bool operator false(Result<T> value) => value is Failure<T>;

      public static bool operator !(Result<T> value) => value is Failure<T>;

      public static implicit operator bool(Result<T> value) => value is Success<T>;

      public static Result<T> Nil(string message) => new Failure<T>(new Exception(message));

      public static implicit operator T(Result<T> result) => result switch
      {
         Success<T> success => success.Value,
         Failure<T> failure => throw failure.Exception,
         _ => throw new InvalidCastException("Must be a Success to return a value")
      };

      public abstract bool Map(out T value, out Exception exception);

      [Obsolete("Use If()")]
      public abstract bool ValueOrOriginal(out T value, out Result<T> original);

      [Obsolete("Use If()")]
      public abstract bool ValueOrCast<TResult>(out T value, out Result<TResult> result);

      [Obsolete("Use bool implicit cast")]
      public abstract bool IsSuccessful { get; }

      [Obsolete("Use bool implicit cast")]
      public abstract bool IsFailed { get; }

      [Obsolete("Use exception")]
      public abstract Result<TOther> ExceptionAs<TOther>();

      public abstract Result<TResult> Map<TResult>(Func<T, Result<TResult>> ifSuccessful);

      public abstract Result<TResult> Map<TResult>(Func<T, TResult> ifSuccessful);

      public abstract Result<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection);

      public abstract Result<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection);

      public abstract Result<TResult> SelectMany<TResult>(Func<T, TResult> func);

      public abstract T Recover(Func<Exception, T> recovery);

      [Obsolete("Use |")]
      public abstract Result<T> Or(Result<T> other);

      [Obsolete("Use |")]
      public abstract Result<T> Or(Func<Result<T>> other);

      [Obsolete("Use |")]
      public abstract Result<T> Or(T other);

      [Obsolete("Use |")]
      public abstract Result<T> Or(Func<T> other);

      public abstract Result<Unit> Unit { get; }

      public abstract Result<T> Always(Action action);

      public abstract Matched<T> Match();

      public abstract bool Map(out T value);

      public abstract bool UnMap(out Exception exception);

      public abstract bool UnMap(out T value, out Exception exception);

      public abstract void Force();

      public abstract T ForceValue();

      public abstract Result<T> OnSuccess(Action<T> action);

      public abstract Result<T> OnFailure(Action<Exception> action);

      public abstract void Deconstruct(out Maybe<T> value, out Exception exception);

      public abstract Result<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage);

      public abstract Maybe<T> Maybe();

      public abstract Responding<T> Responding();

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
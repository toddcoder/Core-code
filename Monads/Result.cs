using System;
using static Core.Lambdas.LambdaFunctions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public abstract class Result<T>
   {
      public class If
      {
         public static If operator &(If @if, bool test) => @if.test && test ? new If(test, nil, nil) : new If(false, nil, nil);

         public static If operator &(If @if, T value) => @if.test ? new If(@if.test, func(() => value), nil) : @if;

         public static If operator &(If @if, Func<T> value) => @if.test ? new If(@if.test, value, nil) : @if;

         public static Result<T> operator &(If @if, Exception exception)
         {
            if (@if.test)
            {
               return @if._value.Map(v => v().Success()) | exception;
            }
            else
            {
               return exception;
            }
         }

         protected bool test;
         protected Maybe<Func<T>> _value;
         protected Maybe<Exception> _exception;

         internal If(bool test, Maybe<Func<T>> _value, Maybe<Exception> exception)
         {
            this.test = test;
            this._value = _value;
            _exception = exception;
         }

         public bool Test => test;

         public Maybe<Func<T>> Value => _value;

         public Maybe<Exception> Exception => _exception;
      }

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

      public static T operator |(Result<T> result, T defaultValue) => result ? result : defaultValue;

      public static T operator |(Result<T> result, Func<T> defaultValue) => result ? result : defaultValue();

      public static T operator |(Result<T> result, Func<Exception, T> recoveryFunc) => result.Recover(recoveryFunc);

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

      [Obsolete("Use Exception property")]
      public static implicit operator Exception(Result<T> result) => result switch
      {
         Failure<T> failure => failure.Exception,
         _ => throw new InvalidCastException("Must be a Failure to return a value")
      };

      public static T operator ~(Result<T> result) => result.Value;

      public abstract bool Map(out T value, out Exception exception);

      public abstract Result<TResult> Map<TResult>(Func<T, Result<TResult>> ifSuccessful);

      public abstract Result<TResult> Map<TResult>(Func<T, TResult> ifSuccessful);

      public abstract Result<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection);

      public abstract Result<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection);

      public abstract Result<TResult> SelectMany<TResult>(Func<T, TResult> func);

      public abstract T Recover(Func<Exception, T> recovery);

      public abstract Result<Unit> Unit { get; }

      public abstract T Value { get; }

      public abstract Exception Exception { get; }

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
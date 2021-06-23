using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Exceptions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Success<T> : Result<T>, IEquatable<Success<T>>
   {
      public static implicit operator bool(Success<T> _) => true;

      protected T value;

      internal Success(T value) => this.value = value;

      public T Value => value;

      public override bool If(out T value, out Exception exception)
      {
         value = this.value;
         exception = null;

         return true;
      }

      public override bool ValueOrOriginal(out T value, out Result<T> original)
      {
         value = this.value;
         original = this;

         return true;
      }

      public override bool ValueOrCast<TResult>(out T value, out Result<TResult> result)
      {
         value = this.value;
         result = "Do not use this".Failure<TResult>();

         return true;
      }

      public override bool IsSuccessful => true;

      public override bool IsFailed => false;

      public override Result<TOther> ExceptionAs<TOther>() => throw "There is no exception".Throws();

      [DebuggerStepThrough]
      public override Result<TResult> Map<TResult>(Func<T, Result<TResult>> ifSuccessful)
      {
         try
         {
            return ifSuccessful(value);
         }
         catch (Exception exception)
         {
            return failure<TResult>(exception);
         }
      }

      [DebuggerStepThrough]
      public override Result<TResult> Map<TResult>(Func<T, TResult> ifSuccessful)
      {
         try
         {
            return ifSuccessful(value).Success();
         }
         catch (Exception exception)
         {
            return failure<TResult>(exception);
         }
      }

      [DebuggerStepThrough]
      public override Result<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection)
      {
         try
         {
            return projection(value);
         }
         catch (Exception exception)
         {
            return failure<TResult>(exception);
         }
      }

      [DebuggerStepThrough]
      public override Result<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection)
      {
         return func(value).Map(t1 => projection(value, t1));
      }

      [DebuggerStepThrough]
      public override Result<TResult> SelectMany<TResult>(Func<T, TResult> func)
      {
         try
         {
            return func(value).Success();
         }
         catch (Exception exception)
         {
            return failure<TResult>(exception);
         }
      }

      [DebuggerStepThrough]
      public override T Recover(Func<Exception, T> recovery) => value;

      [DebuggerStepThrough]
      public override Result<T> Or(Result<T> other) => this;

      [DebuggerStepThrough]
      public override Result<T> Or(Func<Result<T>> other) => this;

      [DebuggerStepThrough]
      public override Result<T> Or(T other) => this;

      [DebuggerStepThrough]
      public override Result<T> Or(Func<T> other) => this;

      public override Result<Unit> Unit => Monads.Unit.Success();

      public override Result<T> Always(Action action)
      {
         tryTo(action);
         return this;
      }

      public override IMatched<T> Match() => value.Match();

      public override bool If(out T value)
      {
         value = this.value;
         return true;
      }

      public override bool IfNot(out Exception exception)
      {
         exception = null;
         return false;
      }

      public override bool IfNot(out T value, out Exception exception)
      {
         value = this.value;
         exception = null;

         return false;
      }

      public override void Force()
      {
      }

      public override T ForceValue() => value;

      public override Result<T> OnSuccess(Action<T> action)
      {
         try
         {
            action(value);
         }
         catch
         {
         }

         return this;
      }

      public override Result<T> OnFailure(Action<Exception> action) => this;

      public override void Deconstruct(out Maybe<T> value, out Exception exception)
      {
         value = this.value.Some();
         exception = default;
      }

      public override Result<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage)
      {
         return predicate(value) ? this : exceptionMessage().Failure<T>();
      }

      public override Maybe<T> Maybe() => value.Some();

      public override bool EqualToValueOf(Result<T> otherResult) => otherResult.If(out var otherValue) && ValueEqualTo(otherValue);

      public override bool ValueEqualTo(T otherValue) => value.Equals(otherValue);

      public override Result<T> Otherwise(Func<Exception, T> func) => this;

      public override Result<T> Otherwise(Func<Exception, Result<T>> func) => this;

      public override Result<TResult> CastAs<TResult>()
      {
         if (value is TResult result)
         {
            return result.Success();
         }
         else
         {
            return $"Invalid cast from {typeof(T).Name} to {typeof(TResult).Name}".Failure<TResult>();
         }
      }

      public override Result<T> Where(Predicate<T> predicate, string exceptionMessage) => predicate(value) ? this : exceptionMessage.Failure<T>();

      public override Result<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) =>
         predicate(value) ? this : exceptionMessage().Failure<T>();

      public override Result<T> ExceptionMessage(string message) => this;

      public override Result<T> ExceptionMessage(Func<Exception, string> message) => this;

      public bool Equals(Success<T> other)
      {
         return other is not null && ReferenceEquals(this, other) || EqualityComparer<T>.Default.Equals(value, other.value);
      }

      public override bool Equals(object obj) => obj is Success<T> other && Equals(other);

      public override int GetHashCode() => value.GetHashCode();

      public override string ToString() => $"Success({value})";
   }
}
using System;
using System.Diagnostics;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Failure<T> : Result<T>, IEquatable<Failure<T>>
   {
      public static implicit operator bool(Failure<T> _) => false;

      protected Exception exception;

      internal Failure(Exception exception)
      {
         this.exception = exception is FullStackException ? exception : new FullStackException(exception);
      }

      public override bool Map(out T value, out Exception exception)
      {
         value = default;
         exception = this.exception;

         return false;
      }

      [DebuggerStepThrough]
      public override Result<TResult> Map<TResult>(Func<T, Result<TResult>> ifSuccessful) => exception;

      [DebuggerStepThrough]
      public override Result<TResult> Map<TResult>(Func<T, TResult> ifSuccessful) => exception;

      [DebuggerStepThrough]
      public override Result<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection) => exception;

      [DebuggerStepThrough]
      public override Result<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection)
      {
         return exception;
      }

      [DebuggerStepThrough]
      public override Result<TResult> SelectMany<TResult>(Func<T, TResult> func) => exception;

      [DebuggerStepThrough]
      public override T Recover(Func<Exception, T> recovery) => recovery(exception);

      public override Result<Unit> Unit => exception;

      public override T Value => throw exception;

      public override Exception Exception => exception;

      public override Result<T> Always(Action action)
      {
         tryTo(action);
         return this;
      }

      public override bool Map(out T value)
      {
         value = default;
         return false;
      }

      public override bool UnMap(out Exception exception)
      {
         exception = this.exception;
         return true;
      }

      public override bool UnMap(out T value, out Exception exception)
      {
         value = default;
         exception = this.exception;

         return true;
      }

      public override void Force() => throw exception;

      public override T ForceValue() => throw exception;

      public override Result<T> OnSuccess(Action<T> action) => this;

      public override Result<T> OnFailure(Action<Exception> action)
      {
         try
         {
            action(exception);
         }
         catch
         {
         }

         return this;
      }

      public override void Deconstruct(out Maybe<T> value, out Exception exception)
      {
         value = nil;
         exception = this.exception;
      }

      public override Result<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public override Maybe<T> Maybe() => nil;

      public override Responding<T> Responding() => Exception;

      public override bool EqualToValueOf(Result<T> otherResult) => false;

      public override bool ValueEqualTo(T otherValue) => false;

      public override Result<T> Otherwise(Func<Exception, T> func)
      {
         try
         {
            return func(exception);
         }
         catch (Exception otherException)
         {
            return otherException;
         }
      }

      public override Result<T> Otherwise(Func<Exception, Result<T>> func)
      {
         try
         {
            return func(exception);
         }
         catch (Exception otherException)
         {
            return otherException;
         }
      }

      public override Result<TResult> CastAs<TResult>() => exception;

      public override Result<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public override Result<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public override Result<T> ExceptionMessage(string message) => new Failure<T>(new FullStackException(message, exception));

      public override Result<T> ExceptionMessage(Func<Exception, string> message)
      {
         return new FullStackException(message(exception), exception);
      }

      public bool Equals(Failure<T> other)
      {
         return other is not null && ReferenceEquals(this, other) || Equals(exception, other.exception);
      }

      public override bool Equals(object obj) => obj is Failure<T> other && Equals(other);

      public override int GetHashCode() => exception?.GetHashCode() ?? 0;

      public override string ToString() => $"Failure({exception.Message.Elliptical(60, ' ')})";
   }
}
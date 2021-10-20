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

      public Exception Exception => exception;

      public override bool If(out T value, out Exception exception)
      {
         value = default;
         exception = this.exception;

         return false;
      }

      [Obsolete("Use If()")]
      public override bool ValueOrOriginal(out T value, out Result<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      [Obsolete("Use If()")]
      public override bool ValueOrCast<TResult>(out T value, out Result<TResult> result)
      {
         value = default;
         result = failure<TResult>(exception);

         return false;
      }

      public override bool IsSuccessful => false;

      public override bool IsFailed => true;

      [Obsolete("Use exception")]
      public override Result<TOther> ExceptionAs<TOther>() => failure<TOther>(exception);

      [DebuggerStepThrough]
      public override Result<TResult> Map<TResult>(Func<T, Result<TResult>> ifSuccessful) => failure<TResult>(exception);

      [DebuggerStepThrough]
      public override Result<TResult> Map<TResult>(Func<T, TResult> ifSuccessful) => failure<TResult>(exception);

      [DebuggerStepThrough]
      public override Result<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection) => failure<TResult>(exception);

      [DebuggerStepThrough]
      public override Result<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection)
      {
         return failure<T2>(exception);
      }

      [DebuggerStepThrough]
      public override Result<TResult> SelectMany<TResult>(Func<T, TResult> func) => failure<TResult>(exception);

      [DebuggerStepThrough]
      public override T Recover(Func<Exception, T> recovery) => recovery(exception);

      [DebuggerStepThrough, Obsolete("Use |")]
      public override Result<T> Or(Result<T> other) => other;

      [DebuggerStepThrough, Obsolete("Use |")]
      public override Result<T> Or(Func<Result<T>> other) => tryTo(other);

      [DebuggerStepThrough, Obsolete("Use |")]
      public override Result<T> Or(T other) => other.Success();

      [DebuggerStepThrough, Obsolete("Use |")]
      public override Result<T> Or(Func<T> other) => tryTo(other);

      public override Result<Unit> Unit => failure<Unit>(exception);

      public override Result<T> Always(Action action)
      {
         tryTo(action);
         return this;
      }

      public override Matched<T> Match() => failedMatch<T>(exception);

      public override bool If(out T value)
      {
         value = default;
         return false;
      }

      public override bool IfNot(out Exception exception)
      {
         exception = this.exception;
         return true;
      }

      public override bool IfNot(out T value, out Exception exception)
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
         value = none<T>();
         exception = this.exception;
      }

      public override Result<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public override Maybe<T> Maybe() => Maybe<T>.nil;

      public override Responding<T> Responding() => new FailedResponse<T>(Exception);

      public override bool EqualToValueOf(Result<T> otherResult) => false;

      public override bool ValueEqualTo(T otherValue) => false;

      public override Result<T> Otherwise(Func<Exception, T> func)
      {
         try
         {
            return func(exception).Success();
         }
         catch (Exception otherException)
         {
            return failure<T>(otherException);
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
            return failure<T>(otherException);
         }
      }

      public override Result<TResult> CastAs<TResult>() => failure<TResult>(exception);

      public override Result<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public override Result<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public override Result<T> ExceptionMessage(string message) => new Failure<T>(new FullStackException(message, exception));

      public override Result<T> ExceptionMessage(Func<Exception, string> message)
      {
         return new Failure<T>(new FullStackException(message(exception), exception));
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
using System;
using System.Diagnostics;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class Failure<T> : IResult<T>, IEquatable<Failure<T>>
   {
      public static implicit operator bool(Failure<T> _) => false;

      protected Exception exception;

      internal Failure(Exception exception)
      {
         this.exception = exception is FullStackException ? exception : new FullStackException(exception);
      }

      public Exception Exception => exception;

      public bool If(out T value, out Exception exception)
      {
         value = default;
         exception = this.exception;

         return false;
      }

      public bool ValueOrOriginal(out T value, out IResult<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      public bool ValueOrCast<TResult>(out T value, out IResult<TResult> result)
      {
         value = default;
         result = failure<TResult>(exception);

         return false;
      }

      public bool IsSuccessful => false;

      public bool IsFailed => true;

      public IResult<TOther> ExceptionAs<TOther>() => failure<TOther>(exception);

      [DebuggerStepThrough]
      public IResult<TResult> Map<TResult>(Func<T, IResult<TResult>> ifSuccessful) => failure<TResult>(exception);

      [DebuggerStepThrough]
      public IResult<TResult> Map<TResult>(Func<T, TResult> ifSuccessful) => failure<TResult>(exception);

      [DebuggerStepThrough]
      public IResult<TResult> SelectMany<TResult>(Func<T, IResult<TResult>> projection) => failure<TResult>(exception);

      [DebuggerStepThrough]
      public IResult<T2> SelectMany<T1, T2>(Func<T, IResult<T1>> func, Func<T, T1, T2> projection)
      {
         return failure<T2>(exception);
      }

      [DebuggerStepThrough]
      public IResult<TResult> SelectMany<TResult>(Func<T, TResult> func) => failure<TResult>(exception);

      [DebuggerStepThrough]
      public T Recover(Func<Exception, T> recovery) => recovery(exception);

      [DebuggerStepThrough]
      public IResult<T> Or(IResult<T> other) => other;

      [DebuggerStepThrough]
      public IResult<T> Or(Func<IResult<T>> other) => tryTo(other);

      [DebuggerStepThrough]
      public IResult<T> Or(T other) => other.Success();

      [DebuggerStepThrough]
      public IResult<T> Or(Func<T> other) => tryTo(other);

      public IResult<Unit> Unit => failure<Unit>(exception);

      public IResult<T> Always(Action action)
      {
         tryTo(action);
         return this;
      }

      public IMatched<T> Match() => failedMatch<T>(exception);

      public bool If(out T value)
      {
         value = default;
         return false;
      }

      public bool IfNot(out Exception exception)
      {
         exception = this.exception;
         return true;
      }

      public bool IfNot(out T value, out Exception exception)
      {
         value = default;
         exception = this.exception;

         return true;
      }

      public void Force() => throw exception;

      public T ForceValue() => throw exception;

      public IResult<T> OnSuccess(Action<T> action) => this;

      public IResult<T> OnFailure(Action<Exception> action)
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

      public void Deconstruct(out IMaybe<T> value, out Exception exception)
      {
         value = none<T>();
         exception = this.exception;
      }

      public IResult<T> Assert(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public IMaybe<T> Maybe() => none<T>();

      public bool EqualToValueOf(IResult<T> otherResult) => false;

      public bool ValueEqualTo(T otherValue) => false;

      public IResult<T> Otherwise(Func<Exception, T> func)
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

      public IResult<T> Otherwise(Func<Exception, IResult<T>> func)
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

      public IResult<TResult> CastAs<TResult>() => failure<TResult>(exception);

      public IResult<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public IResult<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public IResult<T> ExceptionMessage(string message) => new Failure<T>(new FullStackException(message, exception));

      public IResult<T> ExceptionMessage(Func<Exception, string> message) => new Failure<T>(new FullStackException(message(exception), exception));

      public bool Equals(Failure<T> other)
      {
         return other is not null && ReferenceEquals(this, other) || Equals(exception, other.exception);
      }

      public override bool Equals(object obj) => obj is Failure<T> other && Equals(other);

      public override int GetHashCode() => exception?.GetHashCode() ?? 0;

      public override string ToString() => $"Failure({exception.Message.Elliptical(60, ' ')})";
   }
}
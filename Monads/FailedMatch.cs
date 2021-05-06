using System;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Monads
{
   public class FailedMatch<T> : IMatched<T>, IEquatable<FailedMatch<T>>
   {
      public static implicit operator bool(FailedMatch<T> _) => false;

      protected Exception exception;

      internal FailedMatch(Exception exception)
      {
         this.exception = exception is FullStackException ? exception : new FullStackException(exception);
      }

      public Exception Exception => exception;

      public IMatched<T> Do(Action<T> ifMatched, Action ifNotOrFailed)
      {
         ifNotOrFailed();
         return this;
      }

      public IMatched<T> Do(Action<T> ifMatched, Action ifNotMatched, Action<Exception> ifFailedMatch)
      {
         ifFailedMatch(exception);
         return this;
      }

      public IMatched<TOther> ExceptionAs<TOther>() => failedMatch<TOther>(exception);

      public IMatched<T> Or(IMatched<T> other) => other;

      public IMatched<T> Or(Func<IMatched<T>> other) => other();

      public IMatched<TResult> SelectMany<TResult>(Func<T, IMatched<TResult>> projection)
      {
         return failedMatch<TResult>(exception);
      }

      public IMatched<T2> SelectMany<T1, T2>(Func<T, IMatched<T1>> func, Func<T, T1, T2> projection)
      {
         return failedMatch<T2>(exception);
      }

      public IMatched<TResult> SelectMany<TResult>(Func<T, TResult> func) => failedMatch<TResult>(exception);

      public IMatched<TResult> Select<TResult>(IMatched<T> result, Func<T, TResult> func)
      {
         return failedMatch<TResult>(exception);
      }

      public bool If(out T value)
      {
         value = default;
         return false;
      }

      public bool IfNotMatched() => false;

      public bool Failed(out Exception exception)
      {
         exception = this.exception;
         return true;
      }

      public bool ValueOrOriginal(out T value, out IMatched<T> original)
      {
         value = default;
         original = this;

         return false;
      }

      public bool ValueOrCast<TMatched>(out T value, out IMatched<TMatched> matched)
      {
         value = default;
         matched = failedMatch<TMatched>(exception);

         return false;
      }

      public bool If(out T value, out IMaybe<Exception> exception)
      {
         value = default;
         exception = this.exception.Some();

         return false;
      }

      public bool IfNot(out IMaybe<Exception> anyException)
      {
         anyException = exception.Some();
         return true;
      }

      public bool Else<TOther>(out IMatched<TOther> result)
      {
         result = failedMatch<TOther>(exception);
         return true;
      }

      public IMatched<TOther> Unmatched<TOther>() => failedMatch<TOther>(exception);

      public bool WasMatched(out IMatched<T> matched)
      {
         matched = this;
         return false;
      }

      public void Force() => throw exception;

      public T ForceValue() => throw exception;

      public IMatched<T> UnmatchedOnly() => throw exception;

      public IMatched<TOther> UnmatchedOnly<TOther>() => throw exception;

      public void Deconstruct(out IMaybe<T> value, out IMaybe<Exception> exception)
      {
         value = none<T>();
         exception = this.exception.Some();
      }

      public bool EqualToValueOf(IMatched<T> otherMatched) => false;

      public bool ValueEqualTo(T otherValue) => false;

      public IMatched<TResult> CastAs<TResult>() => failedMatch<TResult>(exception);

      public IMatched<T> Where(Predicate<T> predicate) => this;

      public IMatched<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

      public IMatched<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

      public IMatched<T> ExceptionMessage(string message) => new FailedMatch<T>(new FullStackException(message, exception));

      public IMatched<T> ExceptionMessage(Func<Exception, string> message)
      {
         return new FailedMatch<T>(new FullStackException(message(exception), exception));
      }

      public bool IsMatched => false;

      public bool IsNotMatched => false;

      public bool IsFailedMatch => true;

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched) => failedMatch<TResult>(exception);

      public IMatched<TResult> Map<TResult>(Func<T, TResult> ifMatched) => failedMatch<TResult>(exception);

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<IMatched<TResult>> ifNotMatched)
      {
         return failedMatch<TResult>(exception);
      }

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched, Func<Exception,
         IMatched<TResult>> ifFailedMatch)
      {
         return ifFailedMatch(exception);
      }

      public IMatched<TResult> Map<TResult>(Func<T, IMatched<TResult>> ifMatched,
         Func<IMatched<TResult>> ifNotMatched, Func<Exception, IMatched<TResult>> ifFailedMatch)
      {
         return ifFailedMatch(exception);
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotMatched,
         Func<Exception, TResult> ifFailedMatch)
      {
         return ifFailedMatch(exception);
      }

      public TResult FlatMap<TResult>(Func<T, TResult> ifMatched, Func<TResult> ifNotOrFailed) => ifNotOrFailed();

      public IMatched<T> If(Action<T> action) => this;

      public IMatched<T> Else(Action action) => this;

      public IMatched<T> Else(Action<Exception> action)
      {
         action(exception);
         return this;
      }

      public bool Equals(FailedMatch<T> other)
      {
         return other is not null && ReferenceEquals(this, other) || Equals(exception, other.exception);
      }

      public override bool Equals(object obj) => obj is FailedMatch<T> other && Equals(other);

      public override int GetHashCode() => exception?.GetHashCode() ?? 0;

      public override string ToString() => $"FailedMatch({exception.Message.Elliptical(60, ' ')})";
   }
}